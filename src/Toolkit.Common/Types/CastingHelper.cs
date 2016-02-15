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
        /// Casts a value to type 'TResult' and throws a DebugEx.Assert if the cast fails.
        /// This should allow us to more readily pinpoint previously silent functional errors when we change a dynamic type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Review unused parameters", Justification = "It is being used in Debug")]
        public static TResult SafeCast<TResult>(this object value, string message = "AssertCast Failed") where TResult : class
        {
            TResult result = value as TResult;

            // Do not throw if 'value' itself was null
            Debug.Assert(value == null || result != null, message);

            return result;
        }
    }
}
