using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Toolkit.TestApp.Controls
{
    /// <summary>
    /// This custom ListViewItemPresenter assumes the Grid/ListView is being used with SelectionMode == Multiple
    /// and applies custom selection visuals and focus visuals
    /// </summary>
    public class CustomListViewItemPresenter : ListViewItemPresenter
    {
        public static readonly DependencyProperty FocusRectBrushProperty =
            DependencyProperty.Register("FocusRectBrush", typeof(SolidColorBrush), typeof(CustomListViewItemPresenter), new PropertyMetadata(null));

        public static readonly DependencyProperty FocusRectThicknessProperty =
            DependencyProperty.Register("FocusRectThickness", typeof(double), typeof(CustomListViewItemPresenter), new PropertyMetadata(0d));

        public static readonly DependencyProperty FocusRectPaddingProperty =
            DependencyProperty.Register("FocusRectPadding", typeof(double), typeof(CustomListViewItemPresenter), new PropertyMetadata(0d));

        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(SolidColorBrush), typeof(CustomListViewItemPresenter), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedRectThicknessProperty =
            DependencyProperty.Register("SelectedRectThickness", typeof(double), typeof(CustomListViewItemPresenter), new PropertyMetadata(0d));

        // These are the only objects we need to show item's content and visuals for
        // focus and pointer over state. This is a huge reduction in total elements
        // over the expanded ListViewItem template. Even better is that these objects
        // are only instantiated when they are needed instead of at startup!
        private Grid _contentGrid = null;
        private Rectangle _focusVisual = null;

        private GridView _parentGridView;
        private Border _adornerBorder;
        private Border _selectedBorderVisual;
        private Polygon _selectedPathVisual;
        private FontIcon _selectedFontIconVisual;

        public CustomListViewItemPresenter()
            : base()
        {
            // These must be set to allow us to take advantage of the built in Border that ListViewItemPresenter adds
            // when the List/GridView has SelectionMode == Multiple
            CheckMode = ListViewItemPresenterCheckMode.Overlay;
            SelectionCheckMarkVisualEnabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating what color the focus rect should be.
        /// </summary>
        public SolidColorBrush FocusRectBrush
        {
            get { return (SolidColorBrush)GetValue(FocusRectBrushProperty); }
            set { SetValue(FocusRectBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating how thick the focus rect should be.
        /// </summary>
        public double FocusRectThickness
        {
            get { return (double)GetValue(FocusRectThicknessProperty); }
            set { SetValue(FocusRectThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating how much padding to add to the content for the focus rect.
        /// </summary>
        public double FocusRectPadding
        {
            get { return (double)GetValue(FocusRectPaddingProperty); }
            set { SetValue(FocusRectPaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating what color the selection visuals should be.
        /// </summary>
        public SolidColorBrush SelectedBrush
        {
            get { return (SolidColorBrush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating how thick the selection rect should be.
        /// </summary>
        public double SelectedRectThickness
        {
            get { return (double)GetValue(SelectedRectThicknessProperty); }
            set { SetValue(SelectedRectThicknessProperty, value); }
        }

        protected override bool GoToElementStateCore(string stateName, bool useTransitions)
        {
            base.GoToElementStateCore(stateName, useTransitions);

            // Change the visuals shown based on the state the item is going to
            switch (stateName)
            {
                case "Normal":
                    HideSelectedVisuals();
                    break;

                case "Focused":
                    ShowFocusVisuals();
                    break;

                case "Unfocused":
                    HideFocusVisuals();
                    break;

                case "PointerOverSelected":
                case "Selected":
                    ShowSelectedVisuals();
                    break;

                case "PointerOver":
                    HideSelectedVisuals();
                    break;
                default:
                    break;
            }

            return true;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var obj = VisualTreeHelper.GetParent(this);
            while (!(obj is GridView))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            _parentGridView = (GridView)obj;

            PrepareAdorner();
        }

        private void PrepareAdorner()
        {
            // ListViewItemPresenter already adds a border that sits above all other elements
            // when the List/GridView is set to SelectionMode == Multiple
            // Take advantage of that and use it to inject custom elmeents
            _adornerBorder = (Border)VisualTreeHelper.GetChild(this, 1);
            _contentGrid = new Grid();
            _adornerBorder.Child = _contentGrid;

            _adornerBorder.Width = Double.NaN;
            _adornerBorder.Height = Double.NaN;
            _adornerBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            _adornerBorder.VerticalAlignment = VerticalAlignment.Stretch;
            _adornerBorder.Clip = null;
            _adornerBorder.Margin = new Thickness(0);
            _adornerBorder.Padding = new Thickness(0);

            // Add the appropriate margin needed for the focus rect to the container
            var panel = VisualTreeHelper.GetChild(this, 0) as Panel;
            panel.Margin = new Thickness(FocusRectPadding);
        }

        private void ShowFocusVisuals()
        {
            // Create the elements necessary to show focus visuals if they have
            // not been created yet.
            if (!FocusElementsAreCreated())
            {
                CreateFocusElements();
            }

            // Make sure the elements necessary to show focus visuals are opaque
            _focusVisual.Opacity = 1;
        }

        private void HideFocusVisuals()
        {
            // Hide the elements that visualize focus if they have been created
            if (FocusElementsAreCreated())
            {
                _focusVisual.Opacity = 0;
            }
        }

        private void ShowSelectedVisuals()
        {
            // Create the elements necessary to show selected visuals if they have
            // not been created yet.
            if (!SelectedElementsAreCreated())
            {
                CreateSelectedElements();
            }

            // Make sure the elements necessary to show selected visuals are opaque
            _selectedBorderVisual.Opacity = 1;
            _selectedPathVisual.Opacity = 1;
            _selectedFontIconVisual.Opacity = 1;
        }

        private void HideSelectedVisuals()
        {
            // Hide the elements that visualize selection if they have been created
            if (SelectedElementsAreCreated())
            {
                _selectedBorderVisual.Opacity = 0;
                _selectedPathVisual.Opacity = 0;
                _selectedFontIconVisual.Opacity = 0;
            }
        }

        private void CreateFocusElements()
        {
            // Create the focus visual which is a Rectangle with the correct attributes
            _focusVisual = new Rectangle();
            _focusVisual.IsHitTestVisible = false;
            _focusVisual.Opacity = 0;
            _focusVisual.StrokeThickness = FocusRectThickness;

            _focusVisual.Stroke = FocusRectBrush;

            _contentGrid.Children.Insert(0, _focusVisual);
        }

        private bool FocusElementsAreCreated()
        {
            return _focusVisual != null;
        }

        private void CreateSelectedElements()
        {
            _selectedBorderVisual = new Border();
            _selectedPathVisual = new Polygon();
            _selectedFontIconVisual = new FontIcon();
            var focusRectMargin = new Thickness(FocusRectPadding);

            _selectedBorderVisual.IsHitTestVisible = false;
            _selectedBorderVisual.Opacity = 0;
            _selectedBorderVisual.BorderBrush = SelectedBrush;
            _selectedBorderVisual.BorderThickness = new Thickness(SelectedRectThickness);
            _selectedBorderVisual.Margin = focusRectMargin;

            _selectedPathVisual.IsHitTestVisible = false;
            _selectedPathVisual.Opacity = 0;
            _selectedPathVisual.HorizontalAlignment = HorizontalAlignment.Right;
            _selectedPathVisual.Fill = SelectedBrush;
            _selectedPathVisual.Points.Add(new Point(0, 0));
            _selectedPathVisual.Points.Add(new Point(35, 0));
            _selectedPathVisual.Points.Add(new Point(35, 35));
            _selectedPathVisual.Margin = focusRectMargin;

            _selectedFontIconVisual.IsHitTestVisible = false;
            _selectedFontIconVisual.Opacity = 0;
            _selectedFontIconVisual.Margin = new Thickness(FocusRectPadding, FocusRectPadding + SelectedRectThickness, FocusRectPadding + SelectedRectThickness, FocusRectPadding);
            _selectedFontIconVisual.HorizontalAlignment = HorizontalAlignment.Right;
            _selectedFontIconVisual.VerticalAlignment = VerticalAlignment.Top;
            _selectedFontIconVisual.FontFamily = new FontFamily("Segoe MDL2 Assets");
            _selectedFontIconVisual.FontSize = 16;
            _selectedFontIconVisual.Foreground = new SolidColorBrush(Colors.White);
            _selectedFontIconVisual.Glyph = "\uE73E";

            _contentGrid.Children.Insert(0, _selectedFontIconVisual);
            _contentGrid.Children.Insert(0, _selectedBorderVisual);
            _contentGrid.Children.Insert(0, _selectedPathVisual);
        }

        private bool SelectedElementsAreCreated()
        {
            return _selectedBorderVisual != null;
        }
    }
}
