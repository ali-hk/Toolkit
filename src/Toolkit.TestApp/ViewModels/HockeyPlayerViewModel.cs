using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.Models;

namespace Toolkit.TestApp.ViewModels
{
    public class HockeyPlayerViewModel : ViewModelBase
    {
        public HockeyPlayerViewModel(HockeyPlayer hockeyPlayer)
        {
            FirstName = hockeyPlayer.FirstName;
            LastName = hockeyPlayer.LastName;
            Number = hockeyPlayer.Number;
            Photo = hockeyPlayer.Photo;
            Born = hockeyPlayer.Born;
            NationalTeam = hockeyPlayer.NationalTeam;
            Position = hockeyPlayer.Position;
            Shot = hockeyPlayer.Shot;
            GamesPlayed = hockeyPlayer.GamesPlayed;
            Goals = hockeyPlayer.Goals;
            Assists = hockeyPlayer.Assists;
            Points = hockeyPlayer.Points;
            PenaltyMinutes = hockeyPlayer.PenaltyMinutes;
            Teams = hockeyPlayer.Teams.Select(team => new TeamViewModel(team)).ToList();
        }

        public string FirstName { get; }

        public string LastName { get; }

        public int Number { get; }

        public string Photo { get; }

        public DateTime Born { get; }

        public string NationalTeam { get; }

        public HockeyPosition Position { get; }

        public ShotType Shot { get; }

        public int GamesPlayed { get; }

        public int Goals { get; }

        public int Assists { get; }

        public int Points { get; }

        public int PenaltyMinutes { get; }

        public IReadOnlyCollection<TeamViewModel> Teams { get; }
    }
}
