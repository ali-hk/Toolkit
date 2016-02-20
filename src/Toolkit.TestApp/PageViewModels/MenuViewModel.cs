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
        private Dictionary<PageTokens, bool> _canNavigateLookup;
        private PageTokens _currentPageToken;

        public MenuViewModel(INavigationService navigationService, IResourceLoader resourceLoader)
        {
            _navigationService = navigationService;

            Commands = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("DTSBehaviorSampleMenuItemDisplayName"), FontIcon = "\ue15f", Command = new DelegateCommand(NavigateToDTSBehaviorSamplePage, CanNavigateToDTSBehaviorSamplePage) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("DTSSampleMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(NavigateToDTSSamplePage, CanNavigateToDTSSamplePage) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("IncrementalLoadingMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(NavigateToIncrementalLoadingSamplePage, CanNavigateToIncrementalLoadingSamplePage) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("DeferLoadStrategyMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(NavigateToDeferLoadStrategySamplePage, CanNavigateToDeferLoadStrategySamplePage) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("OldMainPageMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(NavigateToOldMainPage, CanNavigateToOldMainPage) }
            };

            _currentPageToken = PageTokens.DTSBehaviorSample;
            _canNavigateLookup = new Dictionary<PageTokens, bool>
            {
                { PageTokens.DTSBehaviorSample, false },
                { PageTokens.DTSSample, true },
                { PageTokens.IncrementalLoadingSample, true },
                { PageTokens.DeferLoadStrategySample, true },
                { PageTokens.OldMain, true }
            };
        }

        public ObservableCollection<MenuItemViewModel> Commands { get; set; }

        private void NavigateToDTSBehaviorSamplePage()
        {
            if (CanNavigateToDTSBehaviorSamplePage())
            {
                if (_navigationService.Navigate(nameof(PageTokens.DTSBehaviorSample), null))
                {
                    UpdateCanNavigateLookup(PageTokens.DTSBehaviorSample);
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToDTSBehaviorSamplePage()
        {
            return _canNavigateLookup[PageTokens.DTSBehaviorSample];
        }

        private void NavigateToOldMainPage()
        {
            if (CanNavigateToOldMainPage())
            {
                if (_navigationService.Navigate(nameof(PageTokens.OldMain), null))
                {
                    UpdateCanNavigateLookup(PageTokens.OldMain);
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToOldMainPage()
        {
            return _canNavigateLookup[PageTokens.OldMain];
        }

        private void NavigateToDTSSamplePage()
        {
            if (CanNavigateToDTSSamplePage())
            {
                if (_navigationService.Navigate(nameof(PageTokens.DTSSample), null))
                {
                    UpdateCanNavigateLookup(PageTokens.DTSSample);
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToDTSSamplePage()
        {
            return _canNavigateLookup[PageTokens.DTSSample];
        }

        private void NavigateToIncrementalLoadingSamplePage()
        {
            if (CanNavigateToIncrementalLoadingSamplePage())
            {
                if (_navigationService.Navigate(nameof(PageTokens.IncrementalLoadingSample), null))
                {
                    UpdateCanNavigateLookup(PageTokens.IncrementalLoadingSample);
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToIncrementalLoadingSamplePage()
        {
            return _canNavigateLookup[PageTokens.IncrementalLoadingSample];
        }

        private void NavigateToDeferLoadStrategySamplePage()
        {
            if (CanNavigateToIncrementalLoadingSamplePage())
            {
                if (_navigationService.Navigate(nameof(PageTokens.DeferLoadStrategySample), null))
                {
                    UpdateCanNavigateLookup(PageTokens.DeferLoadStrategySample);
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToDeferLoadStrategySamplePage()
        {
            return _canNavigateLookup[PageTokens.DeferLoadStrategySample];
        }

        private void RaiseCanExecuteChanged()
        {
            foreach (var item in Commands)
            {
                (item.Command as DelegateCommand).RaiseCanExecuteChanged();
            }
        }

        private void UpdateCanNavigateLookup(PageTokens navigatedTo)
        {
            _canNavigateLookup[_currentPageToken] = true;
            _canNavigateLookup[navigatedTo] = false;
            _currentPageToken = navigatedTo;
        }
    }
}
