using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Caching
{
    public enum CacheItemExpirationPolicy
    {
        /// <summary>
        /// Item is valid as long as it was updated
        /// within the TimeToLive period.
        /// </summary>
        TimeToLive,

        /// <summary>
        /// Item is valid indefinitely and won't expire.
        /// </summary>
        Indefinite
    }
}
