using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Collections
{
    public class DelegateComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _delegateComparer;

        public DelegateComparer(Func<T, T, int> delegateComparer)
        {
            if (delegateComparer == null)
            {
                throw new ArgumentNullException(nameof(delegateComparer));
            }

            _delegateComparer = delegateComparer;
        }

        public int Compare(T x, T y)
        {
            return _delegateComparer(x, y);
        }
    }
}
