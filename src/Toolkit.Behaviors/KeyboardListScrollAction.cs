using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Xaml.VisualTree;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Toolkit.Behaviors
{
    public class KeyboardListScrollAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty HorizontalPageSizeProperty =
            DependencyProperty.Register(nameof(HorizontalPageSize), typeof(int), typeof(KeyboardListScrollAction), new PropertyMetadata(0));

        public static readonly DependencyProperty VerticalPageSizeProperty =
            DependencyProperty.Register(nameof(VerticalPageSize), typeof(int), typeof(KeyboardListScrollAction), new PropertyMetadata(0));

        public static readonly DependencyProperty ScrollDirectionDelegateProperty =
            DependencyProperty.Register(nameof(ScrollDirectionDelegate), typeof(Func<VirtualKey, ScrollDirection>), typeof(KeyboardListScrollAction), new PropertyMetadata(null));

        public int HorizontalPageSize
        {
            get { return (int)GetValue(HorizontalPageSizeProperty); }
            set { SetValue(HorizontalPageSizeProperty, value); }
        }

        public int VerticalPageSize
        {
            get { return (int)GetValue(VerticalPageSizeProperty); }
            set { SetValue(VerticalPageSizeProperty, value); }
        }

        public Func<VirtualKey, ScrollDirection> ScrollDirectionDelegate
        {
            get { return (Func<VirtualKey, ScrollDirection>)GetValue(ScrollDirectionDelegateProperty); }
            set { SetValue(ScrollDirectionDelegateProperty, value); }
        }

        public object Execute(object sender, object parameter)
        {
            var eventArgs = parameter as KeyRoutedEventArgs;

            var listViewBase = sender as ListViewBase;
            if (listViewBase == null)
            {
                throw new ArgumentException($"{nameof(KeyboardListScrollAction)} can only be used with ListViewBase");
            }

            if (ScrollDirectionDelegate == null)
            {
                throw new ArgumentNullException(nameof(ScrollDirectionDelegate));
            }

            var horizontalPageSize = HorizontalPageSize;
            var verticalPageSize = VerticalPageSize;

            var scrollableSize = horizontalPageSize * verticalPageSize;

            var currentItem = eventArgs.OriginalSource as SelectorItem;
            if (currentItem == null)
            {
                currentItem = VisualTreeUtilities.GetFirstParentOfType<SelectorItem>(eventArgs.OriginalSource as DependencyObject);
            }

            if (currentItem == null)
            {
                throw new ArgumentException($"No SelectorItem found in visual tree for {eventArgs.OriginalSource.ToString()}");
            }

            var currentIndex = listViewBase.IndexFromContainer(currentItem);

            if (currentIndex < 0)
            {
                Debug.Assert(currentIndex >= 0, "Current item hasn't been realized yet");
                return null;
            }

            var direction = ScrollDirectionDelegate?.Invoke(eventArgs.Key);
            if (direction == ScrollDirection.None)
            {
                return null;
            }

            var targetIndex = currentIndex + (direction == ScrollDirection.Up || direction == ScrollDirection.Left ? -scrollableSize : scrollableSize);

            var max = listViewBase.Items.Count - 1;
            targetIndex = Math.Min(max, Math.Max(0, targetIndex));

            if (direction == ScrollDirection.Up || direction == ScrollDirection.Left)
            {
                for (var i = targetIndex; i < currentIndex; i++)
                {
                    if (FocusIndex(listViewBase, i))
                    {
                        break;
                    }
                }
            }
            else
            {
                for (var i = targetIndex; i > currentIndex; i--)
                {
                    if (FocusIndex(listViewBase, i))
                    {
                        break;
                    }
                }
            }

            return null;
        }

        private static bool FocusIndex(ListViewBase listViewBase, int i)
        {
            var newContainer = listViewBase.ContainerFromIndex(i) as Control;
            if (newContainer == null)
            {
                return false;
            }

            newContainer.Focus(FocusState.Keyboard);
            listViewBase.ScrollIntoView(newContainer);
            return true;
        }
    }
}
