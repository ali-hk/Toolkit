using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Types
{
    public static class CastingHelper
    {
        /// <summary>
        /// Casts a value to type 'TResult' and throws a Debug.Assert if the cast fails.
        /// </summary>
        public static TResult SafeCast<TResult>(this object value, string message = "SafeCast Failed") where TResult : class
        {
            TResult result = value as TResult;

            // Do not throw if 'value' itself was null
            Debug.Assert(value == null || result != null, message);

            return result;
        }
    }
}
