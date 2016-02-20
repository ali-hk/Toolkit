using Newtonsoft.Json;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.Repositories;
using Toolkit.TestApp.ViewModels;

namespace Toolkit.TestApp.PageViewModels
{
    public class DTSBehaviorSamplePageViewModel : ViewModelBase
    {
        private IReadOnlyCollection<ViewModelBase> _people = null;
        private IAthleteRepository _athleteRepository;

        public DTSBehaviorSamplePageViewModel(IAthleteRepository athleteRepository)
        {
            _athleteRepository = athleteRepository;
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

            People = combinedList;
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
