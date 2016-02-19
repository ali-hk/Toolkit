using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.TestApp.ViewModels
{
    public class DriverViewModel : ViewModelBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Born { get; set; }

        public string Nationality { get; set; }

        public string Photo { get; set; }

        public int Wins { get; set; }

        public int Podiums { get; set; }

        public int PolePositions { get; set; }

        public int FastestLaps { get; set; }

        public int CareerPoints { get; set; }

        public int Championships { get; set; }

        public IReadOnlyCollection<TeamViewModel> Teams { get; set; }
    }
}
