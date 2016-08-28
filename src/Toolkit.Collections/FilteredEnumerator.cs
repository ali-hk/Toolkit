using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolkit.Collections
{
    public class FilteredEnumerator<T> : IEnumerator<T>
    {
        private IList<T> _filteredCollection;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        private int _position = -1;

        public FilteredEnumerator(IList<T> filteredCollection)
        {
            _filteredCollection = filteredCollection;
        }

        public T Current
        {
            get
            {
                try
                {
                    return _filteredCollection[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            _filteredCollection = null;
        }

        public bool MoveNext()
        {
            _position++;
            return _position < _filteredCollection.Count;
        }

        public void Reset()
        {
            _position = -1;
        }
    }
}
