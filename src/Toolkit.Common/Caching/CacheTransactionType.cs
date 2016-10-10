using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Caching
{
    [Flags]
    public enum CacheTransactionType
    {
        // None is intentionally not defined as 0
        // because we want None to be mutually exclusive
        // of the other values in this context. In other words,
        // None is an explicit state here, not the lack of any state
        Indeterminate = 0,
        None = 1,
        Add = 2,
        Remove = 4,
        Update = 8
    }
}
