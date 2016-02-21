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

namespace Toolkit.TestApp.PageViewModels
{
    public class CollectionViewSourceSamplePageViewModel : ViewModelBase
    {
        private IEnumerable<IGrouping<string, ViewModelBase>> _groupedPeople = null;
        private IAthleteRepository _athleteRepository;

        public CollectionViewSourceSamplePageViewModel(IAthleteRepository athleteRepository)
        {
            _athleteRepository = athleteRepository;
        }

        public IEnumerable<IGrouping<string, ViewModelBase>> GroupedPeople
        {
            get
            {
                return _groupedPeople?.ToList();
            }

            private set
            {
                SetProperty(ref _groupedPeople, value);
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            PopulatePeople();
        }

        private void PopulatePeople()
        {
            var hockeyPlayers = PopulateHockeyPlayers();
            var goaltenders = PopulateGoaltenders();
            var drivers = PopulateDrivers();

            var combinedList = new List<ViewModelBase>();

            combinedList.AddRange(hockeyPlayers);
            combinedList.AddRange(goaltenders);
            combinedList.AddRange(drivers);

            GroupedPeople = combinedList.GroupBy(item => item.GetType().Name.Replace("ViewModel", string.Empty));
        }

        private IEnumerable<HockeyPlayerViewModel> PopulateHockeyPlayers()
        {
            var hockeyPlayers = _athleteRepository.GetHockeyPlayers(120);
            return hockeyPlayers.Select(player => new HockeyPlayerViewModel(player));
        }

        private IEnumerable<GoaltenderViewModel> PopulateGoaltenders()
        {
            var goaltenders = _athleteRepository.GetGoaltenders(75);
            return goaltenders.Select(goaltender => new GoaltenderViewModel(goaltender));
        }

        private IEnumerable<DriverViewModel> PopulateDrivers()
        {
            var drivers = _athleteRepository.GetDrivers(50);
            return drivers.Select(driver => new DriverViewModel(driver));
        }
    }
}
