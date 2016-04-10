using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.Collections.Extensions;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Toolkit.Collections.ItemCacheManager
{
    // Implements a relatively simple cache for items based on a set of ranges
    internal class ItemCacheManager<T>
    {
        // Callback that will be used to request data
        private readonly FetchDataCallbackHandler<T> _fetchDataCallback;

        // Maximum number of items that can be fetched in one batch
        private readonly int _maxBatchFetchSize;

        // Timer to optimize the the fetching of data so we throttle Request if the list is still changing
        private readonly DispatcherTimer _timer;

        // data structure to hold all the items that are in the ranges the cache manager is looking after
        private List<CacheEntryBlock<T>> _cacheBlocks;

        // list of ranges for items that are present in the cache
        private ItemIndexRangeList _cachedResults;

        // Used to be able to cancel outstanding requests
        private CancellationTokenSource _cancelTokenSource;

        // Range of items that is currently being requested
        private ItemIndexRange _requestInProgress;

        // List of ranges for items that are not present in the cache
        private ItemIndexRangeList _request;

        public ItemCacheManager(FetchDataCallbackHandler<T> callback, int batchsize = 50)
        {
            _cacheBlocks = new List<CacheEntryBlock<T>>();
            _request = new ItemIndexRangeList();
            _cachedResults = new ItemIndexRangeList();
            _fetchDataCallback = callback;
            _maxBatchFetchSize = batchsize;

            // set up a timer that is used to delay fetching data so that we can catch up if the list is scrolling fast
            _timer = new DispatcherTimer();
            _timer.Tick += (sender, args) => { FetchData(); };

            _timer.Interval = new TimeSpan(20 * 10000);
        }

        public event TypedEventHandler<object, CacheChangedEventArgs<T>> CacheChanged;

        /// <summary>
        ///     Indexer for access to the item cache
        /// </summary>
        /// <param name="index">Item Index</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                // iterates through the cache blocks to find the item
                foreach (CacheEntryBlock<T> block in _cacheBlocks)
                {
                    if (index >= block.FirstIndex && index <= block.LastIndex)
                    {
                        return block.Items[index - block.FirstIndex];
                    }
                }

                return default(T);
            }

            set
            {
                // iterates through the cache blocks to find the right block
                for (int i = 0; i < _cacheBlocks.Count; i++)
                {
                    CacheEntryBlock<T> block = _cacheBlocks[i];
                    if (index >= block.FirstIndex && index <= block.LastIndex)
                    {
                        block.Items[index - block.FirstIndex] = value;

                        // register that we have the result in the cache
                        if (value != null)
                        {
                            _cachedResults.Add((uint)index, 1);
                        }

                        return;
                    }

                    // We have moved past the block where the item is supposed to live
                    if (block.FirstIndex > index)
                    {
                        AddOrExtendBlock(index, value, i);
                        return;
                    }
                }

                // No blocks exist, so creating a new block
                AddOrExtendBlock(index, value, _cacheBlocks.Count);
            }
        }

        /// <summary>
        ///     Updates the desired item range of the cache, discarding items that are not needed, and figuring out which items
        ///     need to be requested. It will then kick off a fetch if required.
        /// </summary>
        /// <param name="ranges">New set of ranges the cache should hold</param>
        public void UpdateRanges(ItemIndexRange[] ranges)
        {
            // Normalize ranges to get a unique set of discontinuous ranges
            ranges = NormalizeRanges(ranges);

            // Fail fast if the ranges haven't changed
            if (!HasRangesChanged(ranges))
            {
                return;
            }

            // To make the cache update easier, we'll create a new set of CacheEntryBlocks
            List<CacheEntryBlock<T>> newCacheBlocks = new List<CacheEntryBlock<T>>();
            foreach (ItemIndexRange range in ranges)
            {
                CacheEntryBlock<T> newBlock = new CacheEntryBlock<T>
                {
                    FirstIndex = range.FirstIndex,
                    Length = range.Length,
                    Items = new T[range.Length]
                };
                newCacheBlocks.Add(newBlock);
            }

            // Copy over data to the new cache blocks from the old ones where there is overlap
            int lastTransferred = 0;
            for (int i = 0; i < ranges.Length; i++)
            {
                CacheEntryBlock<T> newBlock = newCacheBlocks[i];
                ItemIndexRange range = ranges[i];
                int j = lastTransferred;
                while (j < _cacheBlocks.Count && _cacheBlocks[j].FirstIndex <= ranges[i].LastIndex)
                {
                    ItemIndexRange overlap, oldEntryRange;
                    ItemIndexRange[] added, removed;
                    CacheEntryBlock<T> oldBlock = _cacheBlocks[j];
                    oldEntryRange = new ItemIndexRange(oldBlock.FirstIndex, oldBlock.Length);
                    bool hasOverlap = oldEntryRange.DiffRanges(range, out overlap, out removed, out added);
                    if (hasOverlap)
                    {
                        Array.Copy(oldBlock.Items, overlap.FirstIndex - oldBlock.FirstIndex, newBlock.Items, overlap.FirstIndex - range.FirstIndex, (int)overlap.Length);
                    }

                    j++;
                    if (ranges.Length > i + 1 && oldBlock.LastIndex < ranges[i + 1].FirstIndex)
                    {
                        lastTransferred = j;
                    }
                }
            }

            // swap over to the new cache
            _cacheBlocks = newCacheBlocks;

            // figure out what items need to be fetched because we don't have them in the cache
            _request = new ItemIndexRangeList(ranges);
            ItemIndexRangeList newCachedResults = new ItemIndexRangeList();

            // Use the previous knowlege of what we have cached to form the new list
            foreach (ItemIndexRange range in ranges)
            {
                foreach (ItemIndexRange cached in _cachedResults)
                {
                    ItemIndexRange overlap;
                    ItemIndexRange[] added, removed;
                    bool hasOverlap = cached.DiffRanges(range, out overlap, out removed, out added);
                    if (hasOverlap)
                    {
                        newCachedResults.Add(overlap);
                    }
                }
            }

            // remove the data we know we have cached from the results
            foreach (ItemIndexRange range in newCachedResults)
            {
                _request.Subtract(range);
            }

            _cachedResults = newCachedResults;

            StartFetchData();
        }

        // Gets the first block of items that we don't have values for
        public ItemIndexRange GetFirstRequestBlock(int maxsize = 50)
        {
            if (_request.Count > 0)
            {
                ItemIndexRange range = _request[0];
                if (range.Length > 50)
                {
                    range = new ItemIndexRange(range.FirstIndex, 50);
                }

                return range;
            }

            return null;
        }

        // Throttling function for fetching data. Forces a wait of 20ms before making the request.
        // If another fetch is requested in that time, it will reset the timer, so we don't fetch data if the view is actively scrolling
        public void StartFetchData()
        {
            // Verify if an active request is still needed
            if (_requestInProgress != null)
            {
                if (_request.Intersects(_requestInProgress))
                {
                    return;
                }

                _cancelTokenSource.Cancel();
            }

            // Using a timer to delay fetching data by 20ms, if another range comes in that time, then the timer is reset.
            _timer.Stop();
            _timer.Start();
        }

        // Sees if the value is in our cache if so it returns the index
        public int IndexOf(T value)
        {
            foreach (CacheEntryBlock<T> entry in _cacheBlocks)
            {
                int index = Array.IndexOf(entry.Items, value);
                if (index != -1)
                {
                    return index + entry.FirstIndex;
                }
            }

            return -1;
        }

        // Called by the timer to make a request for data
        public async void FetchData()
        {
            // Stop the timer so we don't get fired again unless data is requested
            _timer.Stop();
            if (_requestInProgress != null)
            {
                // Verify if an active request is still needed
                if (_request.Intersects(_requestInProgress))
                {
                    return;
                }

                // Cancel the existing request
                _cancelTokenSource.Cancel();
            }

            ItemIndexRange nextRequest = GetFirstRequestBlock(_maxBatchFetchSize);
            if (nextRequest != null)
            {
                _cancelTokenSource = new CancellationTokenSource();
                CancellationToken ct = _cancelTokenSource.Token;
                _requestInProgress = nextRequest;
                T[] data = null;
                try
                {
                    // Use the callback to get the data, passing in a cancellation token
                    data = await _fetchDataCallback(nextRequest, ct);

                    if (!ct.IsCancellationRequested)
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            int cacheIndex = nextRequest.FirstIndex + i;

                            T oldItem = this[cacheIndex];
                            T newItem = data[i];

                            if (!newItem.Equals(oldItem))
                            {
                                this[cacheIndex] = newItem;

                                // Fire CacheChanged so that the datasource can fire its INCC event, and do other work based on the item having data
                                if (CacheChanged != null)
                                {
                                    CacheChanged(this,
#pragma warning disable SA1118 // Parameter must not span multiple lines
                                        new CacheChangedEventArgs<T>
                                        {
                                            OldItem = oldItem,
                                            NewItem = newItem,
                                            ItemIndex = cacheIndex
                                        });
#pragma warning restore SA1118 // Parameter must not span multiple lines
                                }
                            }
                        }

                        _request.Subtract(new ItemIndexRange(nextRequest.FirstIndex, (uint)data.Length));
                    }
                }

                // Try/Catch is needed as cancellation is via an exception
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    _requestInProgress = null;

                    // Start another request if required
                    FetchData();
                }
            }
        }

        // Extends an existing block if the item fits at the end, or creates a new block
        private void AddOrExtendBlock(int index, T value, int insertBeforeBlock)
        {
            if (insertBeforeBlock > 0)
            {
                CacheEntryBlock<T> block = _cacheBlocks[insertBeforeBlock - 1];
                if (block.LastIndex == index - 1)
                {
                    T[] newItems = new T[block.Length + 1];
                    Array.Copy(block.Items, newItems, (int)block.Length);
                    newItems[block.Length] = value;
                    block.Length++;
                    block.Items = newItems;
                    return;
                }
            }

            CacheEntryBlock<T> newBlock = new CacheEntryBlock<T> { FirstIndex = index, Length = 1, Items = new[] { value } };
            _cacheBlocks.Insert(insertBeforeBlock, newBlock);
        }

        // Compares the new ranges against the previous ones to see if they have changed
        private bool HasRangesChanged(ItemIndexRange[] ranges)
        {
            if (ranges.Length != _cacheBlocks.Count)
            {
                return true;
            }

            for (int i = 0; i < ranges.Length; i++)
            {
                ItemIndexRange r = ranges[i];
                CacheEntryBlock<T> block = _cacheBlocks[i];
                if (r.FirstIndex != block.FirstIndex || r.LastIndex != block.LastIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Merges a set of ranges to form a new set of non-contiguous ranges
        /// </summary>
        /// <param name="ranges">The list of ranges to merge</param>
        /// <returns>A smaller set of merged ranges</returns>
        private ItemIndexRange[] NormalizeRanges(ItemIndexRange[] ranges)
        {
            List<ItemIndexRange> results = new List<ItemIndexRange>();
            foreach (ItemIndexRange range in ranges)
            {
                bool handled = false;
                for (int i = 0; i < results.Count; i++)
                {
                    ItemIndexRange existing = results[i];
                    if (range.ContiguousOrOverlaps(existing))
                    {
                        results[i] = existing.Combine(range);
                        handled = true;
                        break;
                    }

                    if (range.FirstIndex < existing.FirstIndex)
                    {
                        results.Insert(i, range);
                        handled = true;
                        break;
                    }
                }

                if (!handled)
                {
                    results.Add(range);
                }
            }

            return results.ToArray();
        }

        // Type for the cache blocks
        private class CacheEntryBlock<ITEMTYPE>
        {
            public int FirstIndex { get; set; }

            public ITEMTYPE[] Items { get; set; }

            public uint Length { get; set; }

            public int LastIndex
            {
                get { return FirstIndex + (int)Length - 1; }
            }
        }
    }
}