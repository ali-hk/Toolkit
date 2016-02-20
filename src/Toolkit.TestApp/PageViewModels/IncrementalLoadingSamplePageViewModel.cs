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
using Toolkit.TestApp.ViewModels;

namespace Toolkit.TestApp.PageViewModels
{
    public class IncrementalLoadingSamplePageViewModel : ViewModelBase
    {
        private IReadOnlyCollection<ViewModelBase> _people = null;

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

            var combinedList = new List<ViewModelBase>();

            for (int i = 0; i < 20; i++)
            {
                combinedList.AddRange(hockeyPlayers);
            }

            People = combinedList;
        }

        private IEnumerable<HockeyPlayerViewModel> PopulateHockeyPlayers()
        {
            // TODO: Should be in Model layer
            var content = File.ReadAllText(@"Data\HockeyPlayers.json");
            var hockeyPlayers = JsonConvert.DeserializeObject<IEnumerable<HockeyPlayerViewModel>>(content);
            return hockeyPlayers;
        }
    }
}
