using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.Uwp.Collections.ItemCacheManager;
using Windows.UI.Xaml.Data;

namespace Toolkit.Uwp.Collections
{
    public delegate Task<T[]> FetchDataCallbackHandler<T>(ItemIndexRange range, CancellationToken cancellationToken);

    /// <summary>
    /// Provides extended and advanced data virtualization support for ListView/GridView usage
    /// </summary>
    public class VirtualizedDataSource<T> : INotifyCollectionChanged, IList, IItemsRangeInfo
    {
        private int _count;
        private ItemCacheManager<T> _itemCache;
        private UpdateCountCallbackHandler _updateCountCallback;

        /// <param name="fetchDataCallback">Callback used when more data is requested for the cache</param>
        /// <param name="updateCountCallback">Callback used to calculate the total number of items that exist outside the cache</param>
        /// <param name="cacheBatchSize">Cache batch size</param>
        public VirtualizedDataSource(FetchDataCallbackHandler<T> fetchDataCallback, UpdateCountCallbackHandler updateCountCallback, int cacheBatchSize = 10)
        {
            // The ItemCacheManager does most of the heavy lifting. We pass it a callback that it will use to actually fetch data, and the max size of a request
            _itemCache = new ItemCacheManager<T>(fetchDataCallback, cacheBatchSize);
            _itemCache.CacheChanged += ItemCache_CacheChanged;
            _updateCountCallback = updateCountCallback;
            UpdateCount();
        }

        public delegate int UpdateCountCallbackHandler();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count
        {
            get { return _count; }
        }

        #region Parts of IList Not Implemented

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public object this[int index]
        {
            get
            {
                // The cache will return null if it doesn't have the item. Once the item is fetched it will fire a changed event so that we can inform the list control
                return _itemCache[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void UpdateCount()
        {
            _count = _updateCountCallback();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Dispose()
        {
            _itemCache = null;
        }

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            // We know that the visible range is included in the broader range so don't need to hand it to the UpdateRanges call
            // Update the cache of items based on the new set of ranges. It will callback for additional data if required
            _itemCache.UpdateRanges(trackedItems.ToArray());
        }

        public bool Contains(object value)
        {
            return IndexOf(value) != -1;
        }

        public int IndexOf(object value)
        {
            return (value != null) ? _itemCache.IndexOf((T)value) : -1;
        }

        #region Parts of IList Not Implemented

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        // Event fired when items are inserted in the cache
        // Used to fire our collection changed event
        private void ItemCache_CacheChanged(object sender, CacheChangedEventArgs<T> args)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, args.OldItem, args.NewItem, args.ItemIndex));
        }
    }
}