using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Common.Strings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Toolkit.Behaviors
{
    public class RightTapMenuAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty MenuItemsDelegateProperty = DependencyProperty.Register(
            "MenuItemsDelegate",
            typeof(Func<object, IReadOnlyCollection<IContextMenuItem>>),
            typeof(RightTapMenuAction),
            new PropertyMetadata(null));

        public Func<object, IReadOnlyCollection<IContextMenuItem>> MenuItemsDelegate
        {
            get { return (Func<object, IReadOnlyCollection<IContextMenuItem>>)GetValue(MenuItemsDelegateProperty); }
            set { SetValue(MenuItemsDelegateProperty, value); }
        }

        public object Execute(object sender, object parameter)
        {
            var eventArgs = parameter as RightTappedRoutedEventArgs;
            var tappedElement = eventArgs?.OriginalSource as FrameworkElement;

            if (tappedElement != null)
            {
                var contextMenuItemsDelegate = MenuItemsDelegate;
                if (contextMenuItemsDelegate != null && tappedElement.DataContext != null)
                {
                    var itemViewModel = tappedElement.DataContext;
                    var contextMenuItems = contextMenuItemsDelegate(itemViewModel);

                    if (contextMenuItems != null)
                    {
                        var menuFlyout = new MenuFlyout();
                        foreach (var contextMenuItem in contextMenuItems)
                        {
                            if (!contextMenuItem.Title.IsNullOrWhiteSpace() && contextMenuItem.Command != null)
                            {
                                var item = new MenuFlyoutItem { Text = contextMenuItem.Title, Command = contextMenuItem.Command, CommandParameter = itemViewModel };
                                menuFlyout.Items.Add(item);
                            }
                        }

                        if (menuFlyout.Items.Count > 0)
                        {
                            menuFlyout.ShowAt(tappedElement, eventArgs.GetPosition(tappedElement));
                            Flyout.SetAttachedFlyout(tappedElement, menuFlyout);
                            Flyout.ShowAttachedFlyout(tappedElement);

                            // This ensures that nested ListView objects don't try to keep handling this right tap.
                            eventArgs.Handled = true;
                        }
                    }
                }
            }

            return null;
        }
    }
}
