using Prism.Commands;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.ViewModels;

namespace Toolkit.TestApp.PageViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private bool _canNavigateToMain = false;
        private bool _canNavigateToOldMain = true;

        public MenuViewModel(INavigationService navigationService, IResourceLoader resourceLoader)
        {
            _navigationService = navigationService;

            Commands = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("MainPageMenuItemDisplayName"), FontIcon = "\ue15f", Command = new DelegateCommand(NavigateToMainPage, CanNavigateToMainPage) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("OldMainPageMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(NavigateToOldMainPage, CanNavigateToOldMainPage) }
            };
        }

        public ObservableCollection<MenuItemViewModel> Commands { get; set; }

        private void NavigateToMainPage()
        {
            if (CanNavigateToMainPage())
            {
                if (_navigationService.Navigate(PageTokens.MainPage, null))
                {
                    _canNavigateToMain = false;
                    _canNavigateToOldMain = true;
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToMainPage()
        {
            return _canNavigateToMain;
        }

        private void NavigateToOldMainPage()
        {
            if (CanNavigateToOldMainPage())
            {
                if (_navigationService.Navigate(PageTokens.OldMainPage, null))
                {
                    _canNavigateToMain = true;
                    _canNavigateToOldMain = false;
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToOldMainPage()
        {
            return _canNavigateToOldMain;
        }

        private void RaiseCanExecuteChanged()
        {
            foreach (var item in Commands)
            {
                (item.Command as DelegateCommand).RaiseCanExecuteChanged();
            }
        }
    }
}
