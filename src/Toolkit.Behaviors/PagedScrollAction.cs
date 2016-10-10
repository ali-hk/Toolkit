using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Xaml.VisualTree;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Toolkit.Behaviors
{
    /// <summary>
    /// Action to allow a button click or similar event to cause a scrollable target to scroll in a given direction
    /// </summary>
    public class PagedScrollAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty ScrollTargetProperty =
            DependencyProperty.Register(nameof(ScrollTarget), typeof(DependencyObject), typeof(PagedScrollAction), new PropertyMetadata(null, OnScrollTargetChanged));

        public static readonly DependencyProperty ScrollDirectionProperty =
            DependencyProperty.Register(nameof(ScrollDirection), typeof(ScrollDirection), typeof(PagedScrollAction), new PropertyMetadata(ScrollDirection.None));

        private ScrollViewer _scrollViewer = null;

        public DependencyObject ScrollTarget
        {
            get { return (DependencyObject)GetValue(ScrollTargetProperty); }
            set { SetValue(ScrollTargetProperty, value); }
        }

        public ScrollDirection ScrollDirection
        {
            get { return (ScrollDirection)GetValue(ScrollDirectionProperty); }
            set { SetValue(ScrollDirectionProperty, value); }
        }

        public object Execute(object sender, object parameter)
        {
            Debug.Assert(ScrollDirection != ScrollDirection.None, $"{nameof(ScrollDirection)} is set to {nameof(ScrollDirection.None)}. This behavior isn't doing anything");

            if (ScrollTarget != null && ScrollDirection != ScrollDirection.None)
            {
                if (_scrollViewer == null)
                {
                    _scrollViewer = FindScrollViewer();
                }

                if (_scrollViewer != null)
                {
                    if (ScrollDirection == ScrollDirection.Left || ScrollDirection == ScrollDirection.Right)
                    {
                        ScrollPageHorizontal(_scrollViewer, ScrollDirection);
                    }
                    else
                    {
                        ScrollPageVertical(_scrollViewer, ScrollDirection);
                    }
                }
            }

            return null;
        }

        private static void ScrollPageHorizontal(ScrollViewer scrollViewer, ScrollDirection direction)
        {
            if (scrollViewer == null || direction == ScrollDirection.None)
            {
                return;
            }

            double actualWidth = scrollViewer.ActualWidth;
            double currentOffset = scrollViewer.HorizontalOffset;
            double newWidth = 0;

            if (direction == ScrollDirection.Left)
            {
                newWidth = currentOffset - actualWidth;
            }
            else if (direction == ScrollDirection.Right)
            {
                newWidth = currentOffset + actualWidth;
            }

            if (newWidth > scrollViewer.ExtentWidth)
            {
                newWidth = currentOffset;
            }

            if (newWidth < 0)
            {
                newWidth = 0;
            }

            scrollViewer.ChangeView(newWidth, null, null);
        }

        private static void ScrollPageVertical(ScrollViewer scrollViewer, ScrollDirection direction)
        {
            if (scrollViewer == null || direction == ScrollDirection.None)
            {
                return;
            }

            double actualHeight = scrollViewer.ActualHeight;
            double currentOffset = scrollViewer.VerticalOffset;
            double newHeight = 0;

            if (direction == ScrollDirection.Up)
            {
                newHeight = currentOffset - actualHeight;
            }
            else if (direction == ScrollDirection.Down)
            {
                newHeight = currentOffset + actualHeight;
            }

            if (newHeight > scrollViewer.ExtentHeight)
            {
                newHeight = currentOffset;
            }

            if (newHeight < 0)
            {
                newHeight = 0;
            }

            scrollViewer.ChangeView(null, newHeight, null);
        }

        private static void OnScrollTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pagedScrollAction = d as PagedScrollAction;
            if (pagedScrollAction != null)
            {
                pagedScrollAction._scrollViewer = pagedScrollAction.FindScrollViewer();
            }
        }

        private ScrollViewer FindScrollViewer()
        {
            if (ScrollTarget != null)
            {
                return ScrollTarget.GetChild<ScrollViewer>();
            }

            return null;
        }
    }
}
