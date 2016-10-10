using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Caching
{
    public class CacheItem<T>
    {
        public CacheItem(T item, CacheItemExpirationPolicy expirationPolicy)
        {
            Item = item;
            ExpirationPolicy = expirationPolicy;
            LastUpdated = DateTime.UtcNow;
        }

        public T Item { get; private set; }

        public CacheItemExpirationPolicy ExpirationPolicy { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public bool Update(CacheItem<T> updateSource)
        {
            if (ExpirationPolicy != CacheItemExpirationPolicy.Indefinite)
            {
                // Only update the expiration policy if its not indefinite.
                // If it is indefinite, its lifetime is being managed manually
                // and it should stay around until its manually removed.
                ExpirationPolicy = updateSource.ExpirationPolicy;
            }

            LastUpdated = updateSource.LastUpdated;

            var updatableItem = Item as IUpdatable<T>;
            if (updatableItem != null)
            {
                return updatableItem.Update(updateSource.Item);
            }
            else
            {
                Item = updateSource.Item;
                return true;
            }
        }

        public bool IsExpired(TimeSpan timeToLive)
        {
            bool isExpired = false;

            // If the item's expiration policy is TTL, check that it hasn't expired
            if (ExpirationPolicy == CacheItemExpirationPolicy.TimeToLive)
            {
                // Calculate the time since it was last updated
                var timeSinceLastUpdated = DateTime.UtcNow.Subtract(LastUpdated);

                // If the time since last updated is greater than the TTL, its expired
                if (timeSinceLastUpdated.CompareTo(timeToLive) > 0)
                {
                    isExpired = true;
                }
            }

            return isExpired;
        }
    }
}
