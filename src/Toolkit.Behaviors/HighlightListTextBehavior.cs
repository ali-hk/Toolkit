using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Collections;
using Toolkit.Common.Types;
using Toolkit.Xaml.VisualTree;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Toolkit.Behaviors
{
    /// <summary>
    /// Highlights all occurrences of a given search term in all TextBlocks in a List/GridView
    /// Note: This works well with virtualization enabled and disabled.
    /// However if virtualization is enabled it won't work well with small CacheLengths (ex. &lt;0.3)
    /// </summary>
    public class HighlightListTextBehavior : Behavior<ListViewBase>
    {
        public static readonly DependencyProperty SearchTermProperty =
            DependencyProperty.Register("SearchTerm", typeof(string), typeof(HighlightListTextBehavior), new PropertyMetadata(null, OnSearchTermChanged));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(HighlightListTextBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        private ItemIndexRange _previousVisibleRange = new ItemIndexRange(0, 0);
        private WeakReference<IVisibleItemsAwareCollection> _collectionWeakRef;

        public string SearchTerm
        {
            get { return (string)GetValue(SearchTermProperty); }
            set { SetValue(SearchTermProperty, value); }
        }

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        protected override void OnDetaching()
        {
            if (_collectionWeakRef?.SafeResolve() != null)
            {
                _collectionWeakRef.SafeResolve().VisibleItemsChanged -= Collection_VisibleItemsChanged;
            }

            AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
        }

        private static void OnSearchTermChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            var behavior = d as HighlightListTextBehavior;
            var textBlocks = VisualTreeUtilities.GetChildrenOfType<TextBlock>(behavior.AssociatedObject);
            behavior.HighlightText(textBlocks);
        }

        private static void OnVisibleRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void AssociatedObject_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (_collectionWeakRef?.SafeResolve() != null)
            {
                _collectionWeakRef.SafeResolve().VisibleItemsChanged -= Collection_VisibleItemsChanged;
            }

            var collection = AssociatedObject.ItemsSource as IVisibleItemsAwareCollection;
            _collectionWeakRef = collection.AsWeakRef();
            collection.VisibleItemsChanged += Collection_VisibleItemsChanged;
        }

        private void Collection_VisibleItemsChanged(object sender, ItemIndexRange newVisibleRange)
        {
            if (SearchTerm == null)
            {
                return;
            }

            var textBlocks = GetTextBlocksFromRange(newVisibleRange);
            HighlightText(textBlocks);
        }

        private IEnumerable<TextBlock> GetTextBlocksFromRange(ItemIndexRange visibleRange)
        {
            var newVisibleRange = visibleRange as ItemIndexRange;

            var containers = new List<DependencyObject>();
            for (int i = newVisibleRange.FirstIndex; i < _previousVisibleRange.FirstIndex; i++)
            {
                var container = AssociatedObject.ContainerFromIndex(i);
                containers.Add(container);
            }

            for (int i = _previousVisibleRange.LastIndex + 1; i <= newVisibleRange.LastIndex; i++)
            {
                var container = AssociatedObject.ContainerFromIndex(i);
                containers.Add(container);
            }

            _previousVisibleRange = newVisibleRange;

            return containers.SelectMany(item => VisualTreeUtilities.GetChildrenOfType<TextBlock>(item as DependencyObject)).ToList();
        }

        private void HighlightText(IEnumerable<TextBlock> textBlocks)
        {
            var searchTerm = SearchTerm;
            if (searchTerm == null)
            {
                // Don't check for whitespace and return early, because we need to clear out any existing inlines in the textblocks still
                return;
            }

            foreach (var textBlock in textBlocks)
            {
                var originalText = textBlock.Text;
                if (string.IsNullOrWhiteSpace(originalText))
                {
                    continue;
                }

                if (searchTerm.Length == 0)
                {
                    textBlock.Text = originalText;
                    continue;
                }

                // TODO: This doesn't handle multiple occurences
                textBlock.Inlines.Clear();
                var currentIndex = 0;
                var searchTermLength = searchTerm.Length;
                int index = originalText.IndexOf(searchTerm, 0, StringComparison.CurrentCultureIgnoreCase);
                while (index > -1)
                {
                    textBlock.Inlines.Add(new Run() { Text = originalText.Substring(currentIndex, index - currentIndex) });
                    currentIndex = index + searchTermLength;
                    textBlock.Inlines.Add(new Run() { Text = originalText.Substring(index, searchTermLength), Foreground = HighlightBrush });
                    index = originalText.IndexOf(searchTerm, currentIndex, 0, StringComparison.CurrentCultureIgnoreCase);
                }

                textBlock.Inlines.Add(new Run() { Text = originalText.Substring(currentIndex) });
            }
        }
    }
}
