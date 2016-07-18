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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Toolkit.Behaviors
{
    public class HighlightTextBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty SearchTermProperty =
            DependencyProperty.Register(nameof(SearchTerm), typeof(string), typeof(HighlightTextBehavior), new PropertyMetadata(null, OnSearchTermChanged));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(HighlightTextBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        public static readonly DependencyProperty FirstOccurrenceOnlyProperty =
            DependencyProperty.Register(nameof(FirstOccurrenceOnly), typeof(bool), typeof(HighlightTextBehavior), new PropertyMetadata(false, OnFirstOccurrenceOnlyChanged));

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

        public bool FirstOccurrenceOnly
        {
            get { return (bool)GetValue(FirstOccurrenceOnlyProperty); }
            set { SetValue(FirstOccurrenceOnlyProperty, value); }
        }

        protected override void OnAttached()
        {
        }

        protected override void OnDetaching()
        {
        }

        private static void OnSearchTermChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HighlightTextBehavior;
            var textBlocks = VisualTreeUtilities.GetChildrenOfType<TextBlock>(behavior.AssociatedObject);
            behavior.HighlightText(textBlocks);
        }

        private static void OnFirstOccurrenceOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HighlightTextBehavior;
            var textBlocks = VisualTreeUtilities.GetChildrenOfType<TextBlock>(behavior.AssociatedObject);
            behavior.HighlightText(textBlocks);
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

                textBlock.Inlines.Clear();
                var currentIndex = 0;
                var searchTermLength = searchTerm.Length;
                int index = originalText.IndexOf(searchTerm, 0, StringComparison.CurrentCultureIgnoreCase);
                while (index > -1)
                {
                    textBlock.Inlines.Add(new Run() { Text = originalText.Substring(currentIndex, index - currentIndex) });
                    currentIndex = index + searchTermLength;
                    textBlock.Inlines.Add(new Run() { Text = originalText.Substring(index, searchTermLength), Foreground = HighlightBrush });
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
