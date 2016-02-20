using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.Models;

namespace Toolkit.TestApp.ViewModels
{
    public class TeamViewModel : ViewModelBase
    {
        public TeamViewModel(Team team)
        {
            Name = team.Name;
        }

        public string Name { get; }
    }
}
