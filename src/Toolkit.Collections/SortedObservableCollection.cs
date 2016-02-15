using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Collections
{
    public class SortedObservableCollection<T> : ObservableCollection<T>
    {
        private Func<T, T, int> _comparer;

        public SortedObservableCollection(Func<T, T, int> comparer)
        {
            _comparer = comparer;
        }

        public SortedObservableCollection(IEnumerable<T> collection, Func<T, T, int> comparer)
            : base(GetSortedCollection(collection, comparer))
        {
            _comparer = comparer;
        }

        public new void Add(T item)
        {
            int count = Items.Count;
            int insertIndex = count;

            for (int i = 0; i < count; i++)
            {
                if (_comparer(Items[i], item) >= 0)
                {
                    insertIndex = i;
                    break;
                }
            }

            Insert(insertIndex, item);
        }

        private static IEnumerable<T> GetSortedCollection(IEnumerable<T> collection, Func<T, T, int> comparer)
        {
            return new SortedSet<T>(collection, new DelegateComparer<T>(comparer));
        }
    }
}
