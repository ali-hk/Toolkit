using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.Repositories;
using Toolkit.TestApp.ViewModels;
using Windows.UI.Popups;

namespace Toolkit.TestApp.PageViewModels
{
    public class ExpanderSamplePageViewModel : ViewModelBase
    {
        private IReadOnlyCollection<ViewModelBase> _people = null;
        private IAthleteRepository _athleteRepository;

        public ExpanderSamplePageViewModel(IAthleteRepository athleteRepository)
        {
            _athleteRepository = athleteRepository;

            ShowMessageCommand = new DelegateCommand<HockeyPlayerViewModel>(ShowMessageAction);
        }

        public IReadOnlyCollection<ViewModelBase> People
        {
            get
            {
                return _people.ToList();
            }

            private set
            {
                SetProperty(ref _people, value);
            }
        }

        public DelegateCommand<HockeyPlayerViewModel> ShowMessageCommand { get; }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            PopulatePeople();
        }

        private void PopulatePeople()
        {
            var hockeyPlayers = PopulateHockeyPlayers();
            People = hockeyPlayers.ToList();
        }

        private IEnumerable<HockeyPlayerViewModel> PopulateHockeyPlayers()
        {
            var hockeyPlayers = _athleteRepository.GetHockeyPlayers(200);
            return hockeyPlayers.Select(player => new HockeyPlayerViewModel(player));
        }

        private async void ShowMessageAction(HockeyPlayerViewModel playerVM)
        {
            var dialog = new MessageDialog($"You clicked item {playerVM.Number} {playerVM.FirstName} {playerVM.LastName}");
            await dialog.ShowAsync();
        }
    }
}
