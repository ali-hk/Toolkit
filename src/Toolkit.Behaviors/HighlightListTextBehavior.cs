using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Common.Types;
using Toolkit.Uwp.Collections;
using Toolkit.Xaml.VisualTree;
using Windows.UI;
using Windows.UI.Text;
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
            DependencyProperty.Register(nameof(SearchTerm), typeof(string), typeof(HighlightListTextBehavior), new PropertyMetadata(null, OnSearchTermChanged));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(HighlightListTextBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        public static readonly DependencyProperty HighlightFontStyleProperty =
            DependencyProperty.Register(nameof(HighlightFontStyle), typeof(FontStyle), typeof(HighlightListTextBehavior), new PropertyMetadata(FontStyle.Normal));

        public static readonly DependencyProperty HighlightFontWeightProperty =
            DependencyProperty.Register(nameof(HighlightFontWeight), typeof(FontWeight), typeof(HighlightListTextBehavior), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty HighlightUnderlineProperty =
            DependencyProperty.Register(nameof(HighlightUnderline), typeof(bool), typeof(HighlightListTextBehavior), new PropertyMetadata(false));

        public static readonly DependencyProperty FirstOccurrenceOnlyProperty =
            DependencyProperty.Register(nameof(FirstOccurrenceOnly), typeof(bool), typeof(HighlightListTextBehavior), new PropertyMetadata(false, OnFirstOccurrenceOnlyChanged));

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

        public FontStyle HighlightFontStyle
        {
            get { return (FontStyle)GetValue(HighlightFontStyleProperty); }
            set { SetValue(HighlightFontStyleProperty, value); }
        }

        public FontWeight HighlightFontWeight
        {
            get { return (FontWeight)GetValue(HighlightFontWeightProperty); }
            set { SetValue(HighlightFontWeightProperty, value); }
        }

        public bool HighlightUnderline
        {
            get { return (bool)GetValue(HighlightUnderlineProperty); }
            set { SetValue(HighlightUnderlineProperty, value); }
        }

        public bool FirstOccurrenceOnly
        {
            get { return (bool)GetValue(FirstOccurrenceOnlyProperty); }
            set { SetValue(FirstOccurrenceOnlyProperty, value); }
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

        private static void OnFirstOccurrenceOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HighlightListTextBehavior;
            var textBlocks = VisualTreeUtilities.GetChildrenOfType<TextBlock>(behavior.AssociatedObject);
            behavior.HighlightText(textBlocks);
        }

        private void AssociatedObject_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (_collectionWeakRef?.SafeResolve() != null)
            {
                _collectionWeakRef.SafeResolve().VisibleItemsChanged -= Collection_VisibleItemsChanged;
            }

            var collection = AssociatedObject.ItemsSource as IVisibleItemsAwareCollection;
            if (collection == null)
            {
                Debug.WriteLine($"{nameof(HighlightListTextBehavior)}: Collection must implement {nameof(IVisibleItemsAwareCollection)} to be used with {nameof(HighlightListTextBehavior)}.");
            }
            else
            {
                _collectionWeakRef = collection.AsWeakRef();
                collection.VisibleItemsChanged += Collection_VisibleItemsChanged;
            }
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

                textBlock.Inlines.Clear();
                var currentIndex = 0;
                var searchTermLength = searchTerm.Length;
                int index = originalText.IndexOf(searchTerm, 0, StringComparison.CurrentCultureIgnoreCase);
                bool useUnderline = HighlightUnderline;
                while (index > -1)
                {
                    textBlock.Inlines.Add(new Run() { Text = originalText.Substring(currentIndex, index - currentIndex) });
                    currentIndex = index + searchTermLength;
                    var highlightedRun = new Run() { Text = originalText.Substring(index, searchTermLength), Foreground = HighlightBrush ?? textBlock.Foreground, FontStyle = HighlightFontStyle, FontWeight = HighlightFontWeight };
                    if (useUnderline)
                    {
                        var ul = new Underline();
                        ul.Inlines.Add(highlightedRun);
                        textBlock.Inlines.Add(ul);
                    }
                    else
                    {
                        textBlock.Inlines.Add(highlightedRun);
                    }

                    index = originalText.IndexOf(searchTerm, currentIndex, StringComparison.CurrentCultureIgnoreCase);
                    if (FirstOccurrenceOnly)
                    {
                        break;
                    }
                }

                textBlock.Inlines.Add(new Run() { Text = originalText.Substring(currentIndex) });
            }
        }
    }
}
