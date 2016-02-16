using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Toolkit.Behaviors
{
    public class CustomCursorBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CursorTypeProperty =
            DependencyProperty.Register("CursorType", typeof(CoreCursorType), typeof(CustomCursorBehavior), new PropertyMetadata(CoreCursorType.Arrow));

        public CoreCursorType CursorType
        {
            get { return (CoreCursorType)GetValue(CursorTypeProperty); }
            set { SetValue(CursorTypeProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PointerEntered += OnPointerEntered;
            AssociatedObject.PointerExited += OnPointerExited;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PointerEntered -= OnPointerEntered;
            AssociatedObject.PointerExited -= OnPointerExited;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CursorType, 0);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }
    }
}
