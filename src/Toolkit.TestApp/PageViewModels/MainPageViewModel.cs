using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.TestApp.PageViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public string TestString
        {
            get
            {
                return "Testing 123";
            }
        }
    }
}
