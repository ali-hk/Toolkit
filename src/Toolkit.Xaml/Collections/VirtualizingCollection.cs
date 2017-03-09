using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Toolkit.Xaml.Collections
{
    public class VirtualizingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading, IItemsRangeInfo, IVisibleItemsAwareCollection
    {
        private Func<VirtualizingCollection<T>, bool> _hasMoreItemsFunc;
        private Func<uint, CancellationToken, Task<IEnumerable<T>>> _loadMoreItemsFunc;
        private ItemIndexRange _visibleItems;

        public VirtualizingCollection(Func<VirtualizingCollection<T>, bool> hasMoreItemsFunc = null, Func<uint, CancellationToken, Task<IEnumerable<T>>> loadMoreItemsFunc = null)
        {
            _hasMoreItemsFunc = hasMoreItemsFunc;
            _loadMoreItemsFunc = loadMoreItemsFunc;
        }

        public event EventHandler<ItemIndexRange> VisibleItemsChanged;

        public bool HasMoreItems
        {
            get
            {
                if (_hasMoreItemsFunc == null)
                {
                    return false;
                }

                return _hasMoreItemsFunc(this);
            }
        }

        public ItemIndexRange VisibleItems
        {
            get
            {
                return _visibleItems;
            }

            private set
            {
                _visibleItems = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(VisibleItems)));
            }
        }

        public void Dispose()
        {
        }

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            VisibleItems = visibleRange;
            VisibleItemsChanged?.Invoke(this, visibleRange);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((cancellationToken) => LoadMoreItemsInternalAsync(count, cancellationToken));
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsInternalAsync(uint count, CancellationToken cancellationToken)
        {
            var newItems = await _loadMoreItemsFunc?.Invoke(count, cancellationToken);
            foreach (var item in newItems)
            {
                Add(item);
            }

            return new LoadMoreItemsResult { Count = (uint)newItems.LongCount() };
        }
    }
}
