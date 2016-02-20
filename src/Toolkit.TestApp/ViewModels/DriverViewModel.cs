using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.Models;

namespace Toolkit.TestApp.ViewModels
{
    public class DriverViewModel : ViewModelBase
    {
        public DriverViewModel(Driver driver)
        {
            FirstName = driver.FirstName;
            LastName = driver.LastName;
            Born = driver.Born;
            Nationality = driver.Nationality;
            Photo = driver.Photo;
            Wins = driver.Wins;
            Podiums = driver.Podiums;
            PolePositions = driver.PolePositions;
            FastestLaps = driver.FastestLaps;
            CareerPoints = driver.CareerPoints;
            Championships = driver.Championships;
            Teams = driver.Teams.Select(team => new TeamViewModel(team)).ToList();
        }

        public string FirstName { get; }

        public string LastName { get; }

        public DateTime Born { get; }

        public string Nationality { get; }

        public string Photo { get; }

        public int Wins { get; }

        public int Podiums { get; }

        public int PolePositions { get; }

        public int FastestLaps { get; }

        public int CareerPoints { get; }

        public int Championships { get; }

        public IReadOnlyCollection<TeamViewModel> Teams { get; }
    }
}
