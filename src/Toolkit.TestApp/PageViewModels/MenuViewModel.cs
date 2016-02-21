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
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("DTSBehaviorSampleMenuItemDisplayName"), FontIcon = "\ue15f", Command = new DelegateCommand(() => NavigateToSamplePage(PageTokens.DTSBehaviorSample), () => CanNavigateToSamplePage(PageTokens.DTSBehaviorSample)) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("DTSSampleMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(() => NavigateToSamplePage(PageTokens.DTSSample), () => CanNavigateToSamplePage(PageTokens.DTSSample)) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("IncrementalLoadingMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(() => NavigateToSamplePage(PageTokens.IncrementalLoadingSample), () => CanNavigateToSamplePage(PageTokens.IncrementalLoadingSample)) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("DeferLoadStrategyMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(() => NavigateToSamplePage(PageTokens.DeferLoadStrategySample), () => CanNavigateToSamplePage(PageTokens.DeferLoadStrategySample)) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("CustomLVIPMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(() => NavigateToSamplePage(PageTokens.CustomLVIPSample), () => CanNavigateToSamplePage(PageTokens.CustomLVIPSample)) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("StateTriggerMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(() => NavigateToSamplePage(PageTokens.StateTriggerSample), () => CanNavigateToSamplePage(PageTokens.StateTriggerSample)) },
                new MenuItemViewModel { DisplayName = resourceLoader.GetString("OldMainPageMenuItemDisplayName"), FontIcon = "\ue19f", Command = new DelegateCommand(() => NavigateToSamplePage(PageTokens.OldMain), () => CanNavigateToSamplePage(PageTokens.OldMain)) },
            };

            _currentPageToken = PageTokens.DTSBehaviorSample;
            _canNavigateLookup = new Dictionary<PageTokens, bool>();

            foreach (PageTokens pageToken in Enum.GetValues(typeof(PageTokens)))
            {
                _canNavigateLookup.Add(pageToken, true);
            }

            _canNavigateLookup[_currentPageToken] = false;
        }

        public ObservableCollection<MenuItemViewModel> Commands { get; set; }

        private void NavigateToSamplePage(PageTokens pageToken)
        {
            if (CanNavigateToSamplePage(pageToken))
            {
                if (_navigationService.Navigate(pageToken.ToString(), null))
                {
                    UpdateCanNavigateLookup(pageToken);
                    RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanNavigateToSamplePage(PageTokens pageToken)
        {
            return _canNavigateLookup[pageToken];
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
