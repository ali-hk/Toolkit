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
    public class StateTriggerSamplePageViewModel : ViewModelBase
    {
        private IReadOnlyCollection<HockeyPlayerViewModel> _hockeyPlayers = null;
        private IReadOnlyCollection<GoaltenderViewModel> _goaltenders = null;
        private IAthleteRepository _athleteRepository;
        private bool _isGoaltenderData;

        public StateTriggerSamplePageViewModel(IAthleteRepository athleteRepository)
        {
            _athleteRepository = athleteRepository;
            SwitchDataCommand = new DelegateCommand(SwitchDataAction);
        }

        public IReadOnlyCollection<HockeyPlayerViewModel> HockeyPlayers
        {
            get
            {
                return _hockeyPlayers.ToList();
            }

            private set
            {
                SetProperty(ref _hockeyPlayers, value);
            }
        }

        public IReadOnlyCollection<GoaltenderViewModel> Goaltenders
        {
            get
            {
                return _goaltenders.ToList();
            }

            private set
            {
                SetProperty(ref _goaltenders, value);
            }
        }

        public DelegateCommand SwitchDataCommand { get; }

        public bool IsGoaltenderData
        {
            get
            {
                return _isGoaltenderData;
            }

            private set
            {
                SetProperty(ref _isGoaltenderData, value);
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            PopulatePeople();
        }

        private void SwitchDataAction()
        {
            IsGoaltenderData = !IsGoaltenderData;
        }

        private void PopulatePeople()
        {
            HockeyPlayers = PopulateHockeyPlayers().ToList();
            Goaltenders = PopulateGoaltenders().ToList();
        }

        private IEnumerable<HockeyPlayerViewModel> PopulateHockeyPlayers()
        {
            var hockeyPlayers = _athleteRepository.GetHockeyPlayers(200);
            return hockeyPlayers.Select(player => new HockeyPlayerViewModel(player));
        }

        private IEnumerable<GoaltenderViewModel> PopulateGoaltenders()
        {
            var goaltenders = _athleteRepository.GetGoaltenders(75);
            return goaltenders.Select(goaltender => new GoaltenderViewModel(goaltender));
        }
    }
}
