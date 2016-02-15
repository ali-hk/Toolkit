using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Toolkit.Behaviors.Attached
{
    public static class DismissParentFlyoutBehavior
    {
        public static readonly DependencyProperty DismissParentFlyoutBehaviorProperty = DependencyProperty.RegisterAttached(
            "DismissParentFlyoutBehavior",
            typeof(bool),
            typeof(DismissParentFlyoutBehavior),
            new PropertyMetadata(false, DismissParentFlyoutBehaviorProperty_Changed));

        public static void SetDismissParentFlyoutBehavior(DependencyObject obj, bool value)
        {
            if (obj != null)
            {
                obj.SetValue(DismissParentFlyoutBehaviorProperty, value);
            }
        }

        public static bool GetDismissParentFlyoutBehavior(DependencyObject obj)
        {
            return obj != null ? (bool)obj.GetValue(DismissParentFlyoutBehaviorProperty) : false;
        }

        private static void DismissParentFlyoutBehaviorProperty_Changed(DependencyObject parentObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            Button button = parentObject as Button;
            Debug.Assert(button != null, "DismissParentFlyoutBehavior must be attached to a Button control");

            if (button != null)
            {
                button.Click -= Button_Click;

                if ((bool)eventArgs.NewValue)
                {
                    button.Click += Button_Click;
                }
            }
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            // Find the flyout pop-up hosting this button and close it.  We can't use VisualTreeUtil.FindParentOfType
            // here because Popup types are not in the same visual tree as the Button.
            FrameworkElement element = sender as FrameworkElement;
            while (element != null)
            {
                Popup popup = element as Popup;
                if (popup != null)
                {
                    popup.IsOpen = false;
                    return;
                }
                else
                {
                    element = element.Parent as FrameworkElement;
                }
            }
        }
    }
}
