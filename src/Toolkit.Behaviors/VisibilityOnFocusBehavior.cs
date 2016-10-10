using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Common.Types;
using Toolkit.Xaml.VisualTree;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Toolkit.Behaviors
{
    /// <summary>
    /// Makes the associated object visible when it's parent control has focus
    /// </summary>
    public class VisibilityOnFocusBehavior : Behavior<Control>
    {
        private long? _handlerToken;
        private WeakReference<Control> _focusableElementWeakRef;

        protected override void OnAttached()
        {
            AttachFocusCallback();
        }

        protected override void OnDetaching()
        {
            Control focusableElement = null;
            if (_handlerToken.HasValue && _focusableElementWeakRef != null && _focusableElementWeakRef.TryGetTarget(out focusableElement))
            {
                focusableElement.UnregisterPropertyChangedCallback(Control.FocusStateProperty, _handlerToken.Value);
            }
        }

        private void OnFocusChanged(DependencyObject sender, DependencyProperty dp)
        {
            var focusStateObj = sender.GetValue(Control.FocusStateProperty);
            if (focusStateObj is FocusState)
            {
                var focusState = (FocusState)focusStateObj;
                switch (focusState)
                {
                    case FocusState.Pointer:
                    case FocusState.Keyboard:
                    case FocusState.Programmatic:
                        AssociatedObject.Visibility = Visibility.Visible;
                        break;
                    case FocusState.Unfocused:
                    default:
                        AssociatedObject.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private Control FindFocusableElement()
        {
            Control focusableElement = null;
            if (AssociatedObject is Control)
            {
                focusableElement = AssociatedObject as Control;
            }
            else
            {
                focusableElement = AssociatedObject.GetParent<Control>();
            }

            return focusableElement;
        }

        private void AttachFocusCallback()
        {
            Control focusableElement = FindFocusableElement();

            if (focusableElement == null)
            {
                AssociatedObject.Loaded += AssociatedObject_Loaded;
            }
            else
            {
                _handlerToken = focusableElement.RegisterPropertyChangedCallback(Control.FocusStateProperty, OnFocusChanged);
                _focusableElementWeakRef = focusableElement.AsWeakRef();
            }

            // If the item already has focus the callback won't get triggered so set it to visible.
            if (focusableElement != null && focusableElement.FocusState != FocusState.Unfocused)
            {
                AssociatedObject.Visibility = Visibility.Visible;
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AttachFocusCallback();
        }
    }
}
