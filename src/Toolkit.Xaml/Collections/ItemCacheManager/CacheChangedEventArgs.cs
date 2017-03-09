using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Xaml.Collections.ItemCacheManager
{
    // EventArgs class for the CacheChanged event
    internal class CacheChangedEventArgs<T> : EventArgs
    {
        public T OldItem { get; set; }

        public T NewItem { get; set; }

        public int ItemIndex { get; set; }
    }
}
