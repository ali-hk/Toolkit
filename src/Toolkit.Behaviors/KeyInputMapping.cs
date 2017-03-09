using Microsoft.Xaml.Interactivity;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Toolkit.Behaviors
{
    public class KeyInputMapping : DependencyObject
    {
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(nameof(Key), typeof(VirtualKey), typeof(KeyInputMapping), null);

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(KeyInputMapping), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(KeyInputMapping), new PropertyMetadata(null));

        public static readonly DependencyProperty EventTypeProperty =
            DependencyProperty.Register(nameof(EventType), typeof(CoreAcceleratorKeyEventType), typeof(KeyInputMapping), new PropertyMetadata(CoreAcceleratorKeyEventType.KeyUp));

        public VirtualKey Key
        {
            get { return (VirtualKey)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public CoreAcceleratorKeyEventType EventType
        {
            get { return (CoreAcceleratorKeyEventType)GetValue(EventTypeProperty); }
            set { SetValue(EventTypeProperty, value); }
        }
    }
}
