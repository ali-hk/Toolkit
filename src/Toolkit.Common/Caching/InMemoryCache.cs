using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.Common.Threading;

namespace Toolkit.Common.Caching
{
    public class InMemoryCache<TKey, TValue>
    {
        /// <summary>
        /// Tracks which items are expirable to proivde a quick way of comparing
        /// expirable items against max items.
        /// </summary>
        private readonly HashSet<TKey> _expirableKeys;
        private readonly Dictionary<TKey, CacheItem<TValue>> _itemsCache;
        private readonly ReaderWriterLockSlim _itemsCacheLock = new ReaderWriterLockSlim();
        private readonly int _maxItems;
        private readonly TimeSpan _timeToLive;

        /// <summary>
        /// Creates an in memory cache with the specified max items and time to live (applied to
        /// items not using <see cref="CacheItemExpirationPolicy.Indefinite"/> expiration policy).
        /// Note that specifying a low maxItems will hurt performance especially if there aren't
        /// many expired items to purge.
        /// </summary>
        /// <param name="maxItems">Maximum number of expirable items to keep</param>
        /// <param name="timeToLive">How long before expirable items are considered expired</param>
        public InMemoryCache(int maxItems, TimeSpan timeToLive)
        {
            _expirableKeys = new HashSet<TKey>();
            _itemsCache = new Dictionary<TKey, CacheItem<TValue>>();
            _maxItems = maxItems;
            _timeToLive = timeToLive;
        }

        /// <summary>
        /// Adds if item doesn't exist, updates if it does.
        /// </summary>
        /// <returns><see cref="CacheTransactionType.Add"/> if the item was added,
        /// or <see cref="CacheTransactionType.Update"/> if it was updated
        /// or <see cref="CacheTransactionType.None"/> if it existed but was unchanged (already updated).</returns>
        public CacheTransactionType AddOrUpdate(TKey key, TValue item, CacheItemExpirationPolicy expirationPolicy)
        {
            return AddOrUpdateInternal(key, item, expirationPolicy);
        }

        /// <summary>
        /// Removes an item if it exists.
        /// </summary>
        /// <returns><see cref="CacheTransactionType.Remove"/> if the item was added, or <see cref="CacheTransactionType.None"/> if it wasn't in the cache.</returns>
        public CacheTransactionType Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            CacheTransactionType transactionType = CacheTransactionType.None;
            using (_itemsCacheLock.ScopedWriteLock())
            {
                if (_itemsCache.Remove(key))
                {
                    // If the item was found and removed, indicate a Remove transaction
                    // otherwise, leave it as a None transaction
                    transactionType = CacheTransactionType.Remove;

                    // Remove it from the expirable keys to ensure it doesn't count
                    // against max items anymore. If it wasn't expirable, this is a no-op
                    _expirableKeys.Remove(key);
                }
            }

            return transactionType;
        }

        /// <summary>
        /// Processes items in batch, adding or updating values in <paramref name="itemsToAdd"/> and removing those in <paramref name="itemKeysToRemove"/>
        /// If an item is in both <paramref name="itemsToAdd"/> and <paramref name="itemKeysToRemove"/>, the item will be removed.
        /// </summary>
        /// <param name="itemsToAdd">Items to be added or updated.</param>
        /// <param name="itemKeysToRemove">Item keys to be removed.</param>
        /// <param name="expirationPolicy">The caching policy that should be applied to these items.</param>
        /// <returns>An <see cref="CacheUpdateResult"/> with <see cref="CacheUpdateResult.TransactionTypes"/> flags set according to which transactions occured
        /// and collections of each of the items that were impaced.
        /// </returns>
        public CacheUpdateResult Update(IReadOnlyDictionary<TKey, TValue> itemsToAdd, IReadOnlyList<TKey> itemKeysToRemove, CacheItemExpirationPolicy expirationPolicy)
        {
            if (itemsToAdd == null)
            {
                throw new ArgumentNullException(nameof(itemsToAdd));
            }

            if (itemKeysToRemove == null)
            {
                throw new ArgumentNullException(nameof(itemKeysToRemove));
            }

            var resultAddedItems = new Dictionary<TKey, TValue>();
            var resultRemovedItems = new List<TKey>();
            var resultUpdatedItems = new Dictionary<TKey, TValue>();

            foreach (var addedKey in itemsToAdd.Keys)
            {
                // Add or update the cached item as appropriate
                // and add to the corresponding collection for the
                // update result. The item can't already be in one
                // of the two result lists, because it can't be
                // in itemsToAdd more than once
                var addedValue = itemsToAdd[addedKey];
                var transactionType = AddOrUpdateInternal(addedKey, addedValue, expirationPolicy, enforceCacheSize: false);
                if (transactionType == CacheTransactionType.Add)
                {
                    resultAddedItems[addedKey] = addedValue;
                }
                else if (transactionType == CacheTransactionType.Update)
                {
                    resultUpdatedItems[addedKey] = addedValue;
                }
            }

            foreach (var removedKey in itemKeysToRemove)
            {
                // Remove the cached item if it exists
                var transactionType = Remove(removedKey);
                if (transactionType == CacheTransactionType.Remove)
                {
                    // If it existed and was actually removed
                    // add it to the collection of removed items
                    // for the update result
                    resultRemovedItems.Add(removedKey);

                    // Remove it from the added or updated
                    // result collections since we will have
                    // removed any items that appeared in both
                    // itemsToAdd and itemKeysToRemove due to
                    // the order of operations here
                    resultAddedItems.Remove(removedKey);
                    resultUpdatedItems.Remove(removedKey);
                }
            }

            EnsureCacheSize();

            // Iterate over added items and find which ones have been removed
            // due to ensure cache size so we have an accurate set of resulting
            // added items
            var purgedItemKeys = new HashSet<TKey>();
            foreach (var addedItemKey in resultAddedItems.Keys)
            {
                TValue value = default(TValue);
                if (!TryGetValue(addedItemKey, out value))
                {
                    // Item is no longer there, mark for removal from resulting added items
                    purgedItemKeys.Add(addedItemKey);
                }
            }

            foreach (var itemKey in purgedItemKeys)
            {
                resultAddedItems.Remove(itemKey);
            }

            // Determine transaction types
            CacheTransactionType transactionTypes = CacheTransactionType.Indeterminate;
            if (resultAddedItems.Count > 0)
            {
                transactionTypes |= CacheTransactionType.Add;
            }

            if (resultRemovedItems.Count > 0)
            {
                transactionTypes |= CacheTransactionType.Remove;
            }

            if (resultUpdatedItems.Count > 0)
            {
                transactionTypes |= CacheTransactionType.Update;
            }

            if (resultAddedItems.Count == 0 && resultRemovedItems.Count == 0 && resultUpdatedItems.Count == 0)
            {
                transactionTypes |= CacheTransactionType.None;
            }

            return new CacheUpdateResult(transactionTypes, resultAddedItems, resultRemovedItems, resultUpdatedItems);
        }

        /// <summary>
        /// Atomically checks for the existence of a key and fetches the value if it exists
        /// and it has not expired.
        /// If the item is not in the cache or has expired, returns false; Otherwise true.
        /// </summary>
        /// <returns>If the item is not in the cache, returns false; Otherwise true.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            CacheItem<TValue> cachedItem;
            using (_itemsCacheLock.ScopedReadLock())
            {
                // Attempt to retrieve the cached item
                if (!_itemsCache.TryGetValue(key, out cachedItem))
                {
                    // If the item wasn't found, bail out
                    value = default(TValue);
                    return false;
                }
            }

            // If the item is expired, remove it and indicate that it wasn't in the cache
            if (cachedItem.IsExpired(_timeToLive))
            {
                using (_itemsCacheLock.ScopedWriteLock())
                {
                    // Attempt to remove the cached item
                    _itemsCache.Remove(key);
                    _expirableKeys.Remove(key);
                }

                value = default(TValue);
                return false;
            }

            value = cachedItem.Item;
            return true;
        }

        public void Clear()
        {
            using (_itemsCacheLock.ScopedWriteLock())
            {
                _itemsCache.Clear();
                _expirableKeys.Clear();
            }
        }

        private CacheTransactionType AddOrUpdateInternal(TKey key, TValue item, CacheItemExpirationPolicy expirationPolicy, bool enforceCacheSize = true)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            CacheTransactionType transactionType = CacheTransactionType.None;

            // Create the new cache item to use for adding or updating from
            var newCacheItem = new CacheItem<TValue>(item, expirationPolicy);
            using (_itemsCacheLock.ScopedWriteLock())
            {
                CacheItem<TValue> existingCachedItem;

                // Attempt to retrieve the cached item
                bool itemExistsInCache = _itemsCache.TryGetValue(key, out existingCachedItem);
                if (itemExistsInCache && existingCachedItem != null)
                {
                    // If it exists and is not null, attempt to update it
                    if (existingCachedItem.Update(newCacheItem))
                    {
                        transactionType = CacheTransactionType.Update;
                    }
                    else
                    {
                        // If it was already updated, mark the transaction as None
                        transactionType = CacheTransactionType.None;
                    }
                }
                else if (itemExistsInCache && existingCachedItem == null)
                {
                    // If the cached item exists but is somehow null
                    // remove the existing item and replace it with the new value.
                    _itemsCache[key] = newCacheItem;
                }
                else
                {
                    // If it wasn't in the cache already, add it
                    transactionType = CacheTransactionType.Add;
                    _itemsCache.Add(key, newCacheItem);
                }

                if (expirationPolicy != CacheItemExpirationPolicy.Indefinite)
                {
                    // If the item is expirable, add it to the expirable keys
                    // to ensure it counts towards max items and is available for
                    // removal if max is reached.
                    _expirableKeys.Add(key);
                }

                if (enforceCacheSize)
                {
                    EnsureCacheSize();
                }
            }

            return transactionType;
        }

        /// <summary>
        /// WARNING: This method MUST be called from within a Write lock scope. <para/>
        /// Checks the cache size and clears out expired items if the size is exceeded
        /// </summary>
        private void EnsureCacheSize()
        {
            // WARNING: This method MUST be called from within a Write lock scope
            if (_expirableKeys.Count <= _maxItems)
            {
                return;
            }

            if (_expirableKeys.Count > _maxItems)
            {
                PurgeExpiredItems();
            }

            // If the items are still more than the max, sort the items and start dropping the oldest
            if (_expirableKeys.Count > _maxItems)
            {
                Debug.WriteLine($"{nameof(InMemoryCache<TKey, TValue>)}: Expensive purge occurring because there are no more expired items to remove from cache to stay within max items ${_maxItems}. Consider increasing max items.");

                // Filter the dictionary down to only the expirable items. This adds a little more efficiency
                // since its more expensive to compare dates for all when sorting, than to check the HashSet for
                // the existence of the key and filter it out if it's not expirable. This can greatly reduce the
                // number of items that need to be sorted and compared by date.
                var itemsList = _itemsCache.Where(kvp => _expirableKeys.Contains(kvp.Key)).ToList();

                // Sort descending
                itemsList.Sort((x, y) => x.Value.LastUpdated.CompareTo(y.Value.LastUpdated));

                int indexToRemove = 0;

                // Keep removing items until we're within the max items or until there are no more to remove
                while (_expirableKeys.Count > _maxItems && indexToRemove < itemsList.Count && _itemsCache.Count > 0)
                {
                    var itemToRemove = itemsList[indexToRemove];

                    // Only remove expirable items!
                    if (itemToRemove.Value.ExpirationPolicy != CacheItemExpirationPolicy.Indefinite)
                    {
                        _itemsCache.Remove(itemToRemove.Key);
                        _expirableKeys.Remove(itemToRemove.Key);
                    }

                    indexToRemove++;
                }
            }
        }

        /// <summary>
        /// WARNING: This method MUST be called from within a Write lock scope. <para/>
        /// Removes expired items
        /// </summary>
        private void PurgeExpiredItems()
        {
            foreach (var cachedItemKey in _itemsCache.Keys.ToList())
            {
                var cachedItem = _itemsCache[cachedItemKey];
                if (cachedItem.IsExpired(_timeToLive))
                {
                    _itemsCache.Remove(cachedItemKey);
                    _expirableKeys.Remove(cachedItemKey);
                }
            }
        }

        public class CacheUpdateResult
        {
            public CacheUpdateResult(CacheTransactionType transactionTypes, IReadOnlyDictionary<TKey, TValue> addedItems, IReadOnlyList<TKey> removedItems, IReadOnlyDictionary<TKey, TValue> updatedItems)
            {
                TransactionTypes = transactionTypes;
                AddedItems = addedItems;
                RemovedItems = removedItems;
                UpdatedItems = updatedItems;
            }

            /// <summary>
            /// Gets a flags enum indicating which types of transactions occured.
            /// Use TransactionTypes.HasFlag() to check for the existence of a flag.
            /// </summary>
            public CacheTransactionType TransactionTypes { get; private set; }

            public IReadOnlyDictionary<TKey, TValue> AddedItems { get; private set; }

            public IReadOnlyList<TKey> RemovedItems { get; private set; }

            public IReadOnlyDictionary<TKey, TValue> UpdatedItems { get; private set; }
        }
    }
}
