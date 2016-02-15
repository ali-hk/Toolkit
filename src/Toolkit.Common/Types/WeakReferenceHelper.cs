using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Types
{
    public static class WeakReferenceHelper
    {
        public static WeakReference<T> AsWeakRef<T>(this T obj) where T : class
        {
            return new WeakReference<T>(obj);
        }

        /// <summary>
        /// Resolves the weak reference or returns null if the reference is dead
        /// </summary>
        public static T SafeResolve<T>(this WeakReference<T> weakRef) where T : class
        {
            T value = null;
            if (!weakRef.TryGetTarget(out value))
            {
                value = null;
            }

            return value;
        }
    }
}
