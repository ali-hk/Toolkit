using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace Toolkit.Behaviors
{
    [ContentProperty(Name = nameof(Mappings))]
    public class KeyInputMappingBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty MappingsProperty =
           DependencyProperty.Register(nameof(Mappings), typeof(DependencyObjectCollection), typeof(KeyInputMappingBehavior), new PropertyMetadata(null));

        private readonly Dictionary<VirtualKey, KeyInputMapping> _keyDownMappings;
        private readonly Dictionary<VirtualKey, KeyInputMapping> _keyUpMappings;
        private KeyEventHandler _keyDownEventHandler;
        private KeyEventHandler _keyUpEventHandler;

        public KeyInputMappingBehavior()
        {
            Mappings = new DependencyObjectCollection();
            _keyUpMappings = new Dictionary<VirtualKey, KeyInputMapping>();
            _keyDownMappings = new Dictionary<VirtualKey, KeyInputMapping>();
        }

        public DependencyObjectCollection Mappings
        {
            get
            {
                var mappingsCollection = (DependencyObjectCollection)GetValue(MappingsProperty);
                if (mappingsCollection == null)
                {
                    mappingsCollection = new DependencyObjectCollection();
                    SetValue(MappingsProperty, mappingsCollection);
                }

                return mappingsCollection;
            }

            set
            {
                SetValue(MappingsProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _keyDownEventHandler = new KeyEventHandler(OnKeyDown);
            AssociatedObject.AddHandler(UIElement.KeyDownEvent, _keyDownEventHandler, true);

            _keyUpEventHandler = new KeyEventHandler(OnKeyUp);
            AssociatedObject.AddHandler(UIElement.KeyUpEvent, _keyUpEventHandler, true);

            ProcessMappings();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(UIElement.KeyDownEvent, _keyDownEventHandler);
            AssociatedObject.RemoveHandler(UIElement.KeyUpEvent, _keyUpEventHandler);
            base.OnDetaching();
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            KeyInputMapping mapping;
            if (_keyDownMappings.TryGetValue(e.Key, out mapping))
            {
                if (mapping.Command?.CanExecute(mapping.CommandParameter) == true)
                {
                    mapping.Command.Execute(mapping.CommandParameter);
                }
            }
        }

        private void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            KeyInputMapping mapping;
            if (_keyUpMappings.TryGetValue(e.Key, out mapping))
            {
                if (mapping.Command?.CanExecute(mapping.CommandParameter) == true)
                {
                    mapping.Command.Execute(mapping.CommandParameter);
                }
            }
        }

        private void ProcessMappings()
        {
            foreach (KeyInputMapping mapping in Mappings)
            {
                if (mapping.EventType == CoreAcceleratorKeyEventType.KeyUp)
                {
                    _keyUpMappings.Add(mapping.Key, mapping);
                }
                else
                {
                    _keyDownMappings.Add(mapping.Key, mapping);
                }
            }
        }
    }
}
