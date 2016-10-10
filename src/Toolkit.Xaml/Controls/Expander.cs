using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Xaml.VisualTree;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Toolkit.Xaml.Controls
{
    /// <summary>
    /// This control encapsulates the behavior of a header view and a button expanding and collapsing a sub-view.
    /// Typical usage would be to provide accordion/drop-down functionality in a XAML-only, declarative fashion.
    /// In this case, the Header is the ExpandButton.
    /// Alternatively, it can be styled such that the ExpandButton is placed elsewhere relative to the Header (ex. a smaller
    /// expand button to the right of a non-interactive header)
    /// </summary>
    [TemplatePart(Name = nameof(_expanderButton), Type = typeof(ButtonBase))]
    [TemplatePart(Name = nameof(_expandedContentPresenter), Type = typeof(ContentPresenter))]
    [TemplateVisualState(GroupName = ExpanderStatesVisualStateGroupName, Name = ExpanderStatesVisualStateExpandedName)]
    [TemplateVisualState(GroupName = ExpanderStatesVisualStateGroupName, Name = ExpanderStatesVisualStateCollapsedName)]
    public sealed class Expander : Control
    {
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderContentTemplateProperty =
                    DependencyProperty.Register(nameof(HeaderContentTemplate), typeof(DataTemplate), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderContentTemplateSelectorProperty =
                    DependencyProperty.Register(nameof(HeaderContentTemplateSelector), typeof(DataTemplateSelector), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderStyleProperty =
                    DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty ExpandedContentProperty =
            DependencyProperty.Register(nameof(ExpandedContent), typeof(object), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty ExpandedContentTemplateProperty =
                    DependencyProperty.Register(nameof(ExpandedContentTemplate), typeof(DataTemplate), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty ExpandedContentTemplateSelectorProperty =
                    DependencyProperty.Register(nameof(ExpandedContentTemplateSelector), typeof(DataTemplateSelector), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty ExpandedStyleProperty =
                    DependencyProperty.Register(nameof(ExpandedStyle), typeof(Style), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty ExpandButtonStyleProperty =
                    DependencyProperty.Register(nameof(ExpandButtonStyle), typeof(Style), typeof(Expander), new PropertyMetadata(null));

        public static readonly DependencyProperty IsExpandedProperty =
                    DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Expander), new PropertyMetadata(false, OnIsExpandedChanged));

        public static readonly DependencyProperty IsCollapseOnLostFocusEnabledProperty =
                    DependencyProperty.Register(nameof(IsCollapseOnLostFocusEnabled), typeof(bool), typeof(Expander), new PropertyMetadata(false));

        private const string ExpanderStatesVisualStateGroupName = "ExpanderStates";
        private const string ExpanderStatesVisualStateExpandedName = "Expanded";
        private const string ExpanderStatesVisualStateCollapsedName = "Collapsed";

        private ContentPresenter _expandedContentPresenter;
        private ButtonBase _expanderButton;

        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
        }

        public object HeaderContent
        {
            get { return (object)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public DataTemplate HeaderContentTemplate
        {
            get { return (DataTemplate)GetValue(HeaderContentTemplateProperty); }
            set { SetValue(HeaderContentTemplateProperty, value); }
        }

        public DataTemplateSelector HeaderContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderContentTemplateSelectorProperty); }
            set { SetValue(HeaderContentTemplateSelectorProperty, value); }
        }

        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        public object ExpandedContent
        {
            get { return (object)GetValue(ExpandedContentProperty); }
            set { SetValue(ExpandedContentProperty, value); }
        }

        public DataTemplate ExpandedContentTemplate
        {
            get { return (DataTemplate)GetValue(ExpandedContentTemplateProperty); }
            set { SetValue(ExpandedContentTemplateProperty, value); }
        }

        public DataTemplateSelector ExpandedContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ExpandedContentTemplateSelectorProperty); }
            set { SetValue(ExpandedContentTemplateSelectorProperty, value); }
        }

        public Style ExpandedStyle
        {
            get { return (Style)GetValue(ExpandedStyleProperty); }
            set { SetValue(ExpandedStyleProperty, value); }
        }

        public Style ExpandButtonStyle
        {
            get { return (Style)GetValue(ExpandButtonStyleProperty); }
            set { SetValue(ExpandButtonStyleProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to automatically collapsed
        /// the <see cref="Expander"/> when it loses focus.
        /// This is primarily intendd for use with a GamePad.
        /// </summary>
        public bool IsCollapseOnLostFocusEnabled
        {
            get { return (bool)GetValue(IsCollapseOnLostFocusEnabledProperty); }
            set { SetValue(IsCollapseOnLostFocusEnabledProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _expanderButton = GetTemplateChild(nameof(_expanderButton)) as ButtonBase;
            _expandedContentPresenter = GetTemplateChild(nameof(_expandedContentPresenter)) as ContentPresenter;

            _expanderButton.Click += OnExpanderButtonClick;
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if ((e.Key == VirtualKey.GamepadB || e.Key == VirtualKey.Escape) && IsExpanded)
            {
                SetExpanded(false);
                _expanderButton.Focus(FocusState.Programmatic);

                e.Handled = true;
            }

            base.OnKeyUp(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (IsCollapseOnLostFocusEnabled)
            {
                var focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
                if (focusedElement != null)
                {
                    // If the focused element does not share the same ancestor, focus
                    // has moved outside the accordion and the dropdown list should
                    // be collapsed
                    if (!this.ContainsElement(focusedElement))
                    {
                        SetExpanded(false);
                    }
                }
            }
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expanderControl = d as Expander;
            bool oldValBool = (bool)e.OldValue;
            bool newValBool = (bool)e.NewValue;

            if (oldValBool != newValBool)
            {
                expanderControl?.SetExpanded(newValBool);
            }
        }

        private void OnExpanderButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleExpanded();
        }

        private void ToggleExpanded()
        {
            if (IsExpanded)
            {
                SetExpanded(false);
            }
            else
            {
                SetExpanded(true);
            }
        }

        private void SetExpanded(bool expand)
        {
            // Don't do anything if the new state is the same as the previous state
            if (IsExpanded == expand)
            {
                return;
            }

            if (expand)
            {
                // Transition the visual state to expanded
                VisualStateManager.GoToState(this, ExpanderStatesVisualStateExpandedName, false);

                // Find the first focusable element in the expanded content presenter and set focus to it.
                if (!_expandedContentPresenter.SetFocusOnChildControl(FocusState.Programmatic))
                {
                    // If we're unable to get the first focusable element, it may be because x:DeferLoadStrategy was used, wait for the content
                    // to load then set focus.
                    var element = _expandedContentPresenter.Content as FrameworkElement;
                    if (element != null)
                    {
                        element.Loaded += OnContentPresenterContentLoaded;
                    }
                }
            }
            else
            {
                VisualStateManager.GoToState(this, ExpanderStatesVisualStateCollapsedName, false);
            }

            IsExpanded = !IsExpanded;
        }

        private void OnContentPresenterContentLoaded(object sender, RoutedEventArgs e)
        {
            _expandedContentPresenter.SetFocusOnChildControl(FocusState.Programmatic);

            var element = _expandedContentPresenter.Content as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= OnContentPresenterContentLoaded;
            }
        }
    }
}