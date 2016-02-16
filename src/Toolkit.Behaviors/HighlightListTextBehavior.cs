using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Xaml.VisualTree;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Toolkit.Behaviors
{
    public class HighlightListTextBehavior : Behavior<ListViewBase>
    {
        public static readonly DependencyProperty SearchTermProperty =
            DependencyProperty.Register("SearchTerm", typeof(string), typeof(HighlightListTextBehavior), new PropertyMetadata(null, OnSearchTermChanged));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(HighlightListTextBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        public static readonly DependencyProperty VisibleRangeProperty =
            DependencyProperty.Register("VisibleRange", typeof(object), typeof(HighlightListTextBehavior), new PropertyMetadata(null, new PropertyChangedCallback(OnVisibleRangeChanged)));

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

        // Must be an ItemIndexRange. XAML doesn't allow making this ItemIndexRange.
        public object VisibleRange
        {
            get { return (object)GetValue(VisibleRangeProperty); }
            set { SetValue(VisibleRangeProperty, value); }
        }

        protected override void OnAttached()
        {
        }

        protected override void OnDetaching()
        {
        }

        private static void OnSearchTermChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HighlightListTextBehavior;
            var textBlocks = VisualTreeUtilities.GetChildrenOfType<TextBlock>(behavior.AssociatedObject);
            behavior.HighlightText(textBlocks);
        }

        private static void OnVisibleRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HighlightListTextBehavior;
            var textBlocks = behavior.GetTextBlocksFromRange();
            behavior.HighlightText(textBlocks);
        }

        private IEnumerable<TextBlock> GetTextBlocksFromRange()
        {
            var containers = new List<DependencyObject>();
            for (int i = (VisibleRange as ItemIndexRange).FirstIndex; i <= (VisibleRange as ItemIndexRange).LastIndex; i++)
            {
                var container = AssociatedObject.ContainerFromIndex(i);
                containers.Add(container);
            }

            return containers.SelectMany(item => VisualTreeUtilities.GetChildrenOfType<TextBlock>(item as DependencyObject));
        }

        private void HighlightText(IEnumerable<TextBlock> textBlocks)
        {
            foreach (var textBlock in textBlocks)
            {
                var searchTerm = SearchTerm;
                var originalText = textBlock.Text;
                if (string.IsNullOrEmpty(originalText) || (searchTerm == null))
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
