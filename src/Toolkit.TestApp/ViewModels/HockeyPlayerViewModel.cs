using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.TestApp.ViewModels
{
    public class HockeyPlayerViewModel : ViewModelBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Number { get; set; }

        public DateTime Born { get; set; }

        public string NationalTeam { get; set; }

        public HockeyPosition Position { get; set; }

        public ShotType Shot { get; set; }

        public int GamesPlayed { get; set; }

        public int Goals { get; set; }

        public int Assists { get; set; }

        public int Points { get; set; }

        public int PlusMinus { get; set; }

        public int GameWinningGoals { get; set; }

        public int PenaltyMinutes { get; set; }

        public IReadOnlyCollection<TeamViewModel> Teams { get; set; }
    }
}
