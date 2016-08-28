using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Toolkit.Collections
{
    public class FilteredObservableCollectionView<T> : ICollection<T>, IList<T>, IEnumerable<T>, ICollection, IList, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableCollection<T> _collectionToFilter;
        private Predicate<T> _filter;
        private List<T> _filteredCollection;
        private bool[] _skipFlags;

        public FilteredObservableCollectionView(ObservableCollection<T> collectionToFilter, Predicate<T> filter)
        {
            if (collectionToFilter == null)
            {
                throw new ArgumentNullException(nameof(collectionToFilter));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            _collectionToFilter = collectionToFilter;
            _filter = filter;
            _skipFlags = new bool[0];
            UpdateFilteredCollection();

            _collectionToFilter.CollectionChanged += OnSourceCollectionChanged;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get
            {
                return _filteredCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return (Items as IList).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return (Items as IList).SyncRoot;
            }
        }

        protected List<T> Items
        {
            get
            {
                return _filteredCollection;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return Items[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public T this[int index]
        {
            get
            {
                return Items[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new FilteredEnumerator<T>(Items);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FilteredEnumerator<T>(Items);
        }

        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return Items.Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return Items.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index)
        {
            Items.ToArray().CopyTo(array, index);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnItemsAdded(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnItemsRemoved(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnItemsReset(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                default:
                    // ObservableCollections don't support Replace or Move
                    throw new NotSupportedException();
            }
        }

        private void OnItemsAdded(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                // Update the filtered collection based on the source collection
                UpdateFilteredCollection();

                // Based on the items that were added from the source collection,
                // gather the items that were consequently added to the filtered collection
                var list = new List<T>();
                foreach (var item in e.NewItems)
                {
                    var castItem = (T)item;
                    if (Items.Contains(castItem))
                    {
                        list.Add(castItem);
                    }
                }

                // Find the index of the first item that was added

                // Alternate approach, this doesn't work well with strings and value types
                // and any object that doesn't do reference comparison
                ////var castFirstItem = e.NewItems[0] as T;
                ////var startingIndex = Items.IndexOf(castFirstItem);

                var startingIndex = -1;
                for (int i = 0; i <= e.NewStartingIndex; i++)
                {
                    if (_skipFlags[i] == false)
                    {
                        startingIndex++;
                    }
                }

                if (startingIndex >= 0 && list.Count > 0)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, startingIndex));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }

        private void OnItemsRemoved(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                // Based on the items that were removed from the source collection,
                // gather the items that will consequently be removed from the filtered collection
                var list = new List<T>();
                foreach (var item in e.OldItems)
                {
                    var castItem = (T)item;
                    if (Items.Contains(castItem))
                    {
                        list.Add(castItem);
                    }
                }

                // If there were items removed, find the index of the first item that was removed

                // Alternate approach, this doesn't work well with strings and value types
                // and any object that doesn't do reference comparison
                ////var castFirstItem = e.OldItems[0] as T;
                ////var startingIndex = Items.IndexOf(castFirstItem);

                var startingIndex = -1;
                for (int i = 0; i <= e.OldStartingIndex; i++)
                {
                    if (_skipFlags[i] == false)
                    {
                        startingIndex++;
                    }
                }

                // Now that we've captured the necessary information for NotifyCollectionChangedEventArgs,
                // update the filtered collection, prior to firing the event. This must be done prior to
                // firing the event, otherwise consumers wouldn't be able to update correctly based on the new state
                UpdateFilteredCollection();
                if (startingIndex >= 0 && list.Count > 0)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, startingIndex));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }

        private void OnItemsReset(NotifyCollectionChangedEventArgs e)
        {
            UpdateFilteredCollection();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void UpdateFilteredCollection()
        {
            _skipFlags = new bool[_collectionToFilter.Count];
            _filteredCollection = new List<T>();
            for (int i = 0; i < _collectionToFilter.Count; i++)
            {
                if (_filter.Invoke(_collectionToFilter[i]))
                {
                    _skipFlags[i] = false;
                    _filteredCollection.Add(_collectionToFilter[i]);
                }
                else
                {
                    _skipFlags[i] = true;
                }
            }
        }
    }
}
