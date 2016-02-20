using Newtonsoft.Json;
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
    public class DeferLoadStrategySamplePageViewModel : ViewModelBase
    {
        private IReadOnlyCollection<ViewModelBase> _people = null;
        private IAthleteRepository _athleteRepository;

        public DeferLoadStrategySamplePageViewModel(IAthleteRepository athleteRepository)
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

            People = hockeyPlayers.ToList();
        }

        private IEnumerable<HockeyPlayerViewModel> PopulateHockeyPlayers()
        {
            var hockeyPlayers = _athleteRepository.GetHockeyPlayers(200);
            return hockeyPlayers.Select(player => new HockeyPlayerViewModel(player));
        }
    }
}
