using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.Common.Strings;
using Toolkit.Common.Types;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Toolkit.Behaviors
{
    public enum ItemClickCommandMode
    {
        Binding,
        Path
    }

    public enum ItemClickCommandParameterMode
    {
        Value,
        ClickedItem,
        Path
    }

    // TODO: Reconsider the need for RelativeTo.Binding. Why not just use Binding/Value and specify the path there?
    public enum ItemClickCommandRelativeTo
    {
        ClickedItem,
        Self,
        Binding
    }

    /// <summary>
    /// Executes a command on ListViewBase.ItemClick
    /// The command can be bound or a path to the command can be specified.
    /// </summary>
    public class ItemClickCommandAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty RelativeToProperty =
            DependencyProperty.Register(nameof(RelativeTo), typeof(ItemClickCommandRelativeTo), typeof(ItemClickCommandAction), new PropertyMetadata(ItemClickCommandRelativeTo.ClickedItem));

        public static readonly DependencyProperty RelativeSourceProperty =
            DependencyProperty.Register(nameof(RelativeSource), typeof(object), typeof(ItemClickCommandAction), new PropertyMetadata(null));

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(ItemClickCommandMode), typeof(ItemClickCommandAction), new PropertyMetadata(ItemClickCommandMode.Binding));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ItemClickCommandAction), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandPathProperty =
            DependencyProperty.Register(nameof(CommandPath), typeof(string), typeof(ItemClickCommandAction), new PropertyMetadata(null));

        public static readonly DependencyProperty ParameterRelativeToProperty =
            DependencyProperty.Register(nameof(ParameterRelativeTo), typeof(ItemClickCommandRelativeTo), typeof(ItemClickCommandAction), new PropertyMetadata(ItemClickCommandRelativeTo.ClickedItem));

        public static readonly DependencyProperty ParameterRelativeSourceProperty =
            DependencyProperty.Register(nameof(ParameterRelativeSource), typeof(object), typeof(ItemClickCommandAction), new PropertyMetadata(null));

        public static readonly DependencyProperty ParameterModeProperty =
            DependencyProperty.Register(nameof(ParameterMode), typeof(ItemClickCommandParameterMode), typeof(ItemClickCommandAction), new PropertyMetadata(ItemClickCommandParameterMode.ClickedItem));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ItemClickCommandAction), new PropertyMetadata(null));

        public ItemClickCommandRelativeTo RelativeTo
        {
            get { return (ItemClickCommandRelativeTo)GetValue(RelativeToProperty); }
            set { SetValue(RelativeToProperty, value); }
        }

        public object RelativeSource
        {
            get { return (object)GetValue(RelativeSourceProperty); }
            set { SetValue(RelativeSourceProperty, value); }
        }

        public ItemClickCommandMode Mode
        {
            get { return (ItemClickCommandMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string CommandPath
        {
            get { return (string)GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }

        public ItemClickCommandRelativeTo ParameterRelativeTo
        {
            get { return (ItemClickCommandRelativeTo)GetValue(ParameterRelativeToProperty); }
            set { SetValue(ParameterRelativeToProperty, value); }
        }

        public object ParameterRelativeSource
        {
            get { return (object)GetValue(ParameterRelativeSourceProperty); }
            set { SetValue(ParameterRelativeSourceProperty, value); }
        }

        public ItemClickCommandParameterMode ParameterMode
        {
            get { return (ItemClickCommandParameterMode)GetValue(ParameterModeProperty); }
            set { SetValue(ParameterModeProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Parameter is an ItemClickEventArgs object.
        /// </summary>
        public object Execute(object sender, object parameter)
        {
            var eventArgs = parameter as ItemClickEventArgs;
            if (parameter == null)
            {
                throw new ArgumentException("Parameter to ItemClickCommandAction must be an ItemClickEventArgs.", nameof(parameter));
            }

            var parameterRelativeSource = DetermineRelativeSource(ParameterRelativeTo, eventArgs, ParameterRelativeSource);
            var commandParameter = PrepareParameter(parameterRelativeSource);

            var relativeSource = DetermineRelativeSource(RelativeTo, eventArgs, RelativeSource);
            switch (Mode)
            {
                case ItemClickCommandMode.Binding:
                    ExecuteBindingMode(commandParameter);
                    break;
                case ItemClickCommandMode.Path:
                    ExecutePathMode(relativeSource, commandParameter);
                    break;
                default:
                    break;
            }

            return null;
        }

        private object DetermineRelativeSource(ItemClickCommandRelativeTo relativeTo, ItemClickEventArgs eventArgs, object bindingSource)
        {
            object relativeSource = null;
            switch (relativeTo)
            {
                case ItemClickCommandRelativeTo.Self:
                    relativeSource = eventArgs.OriginalSource;
                    break;
                case ItemClickCommandRelativeTo.ClickedItem:
                    relativeSource = eventArgs.ClickedItem;
                    break;
                case ItemClickCommandRelativeTo.Binding:
                    relativeSource = bindingSource;
                    break;
                default:
                    break;
            }

            if (relativeSource == null)
            {
                throw new ArgumentException("Invalid relative source specified");
            }

            return relativeSource;
        }

        private object PrepareParameter(object relativeSource)
        {
            object result = null;
            switch (ParameterMode)
            {
                case ItemClickCommandParameterMode.Value:
                    result = CommandParameter;
                    break;
                case ItemClickCommandParameterMode.ClickedItem:
                    Debug.Assert(ParameterRelativeTo == ItemClickCommandRelativeTo.ClickedItem, $"{nameof(ParameterRelativeTo)} must be {nameof(ItemClickCommandRelativeTo.ClickedItem)} if {nameof(ParameterMode)} is {nameof(ItemClickCommandParameterMode.ClickedItem)}");
                    result = relativeSource;
                    break;
                case ItemClickCommandParameterMode.Path:
                    Debug.Assert(CommandParameter is string, $"{nameof(CommandParameter)} must be a string when {nameof(ParameterMode)} is {nameof(ItemClickCommandParameterMode.Path)}");
                    result = relativeSource.GetPropertyFromPath(CommandParameter as string);
                    break;
                default:
                    break;
            }

            return result;
        }

        private void ExecuteBindingMode(object parameter)
        {
            var command = Command;

            Debug.Assert(command != null, $"{nameof(Command)} must not be null in {nameof(ItemClickCommandMode.Binding)} mode");
            Debug.Assert(CommandPath.IsNullOrWhiteSpace(), $"{nameof(CommandPath)} is ineffective in {nameof(ItemClickCommandMode.Binding)} mode");

            if (command != null)
            {
                command.Execute(parameter);
            }
        }

        private void ExecutePathMode(object relativeSource, object parameter)
        {
            var commandPath = CommandPath;

            Debug.Assert(!commandPath.IsNullOrWhiteSpace(), $"{nameof(CommandPath)} must not be null in {nameof(ItemClickCommandMode.Path)} mode");
            Debug.Assert(Command == null, $"{nameof(Command)} is ineffective in {nameof(ItemClickCommandMode.Path)} mode");

            if (!commandPath.IsNullOrWhiteSpace())
            {
                // Walk the CommandPath for nested properties.
                object propertyValue = relativeSource.GetPropertyFromPath(CommandPath);

                var command = propertyValue as ICommand;
                Debug.Assert(command != null, $"Unable to find property of type ICommand at specified path {CommandPath}");
                if (command != null)
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}
