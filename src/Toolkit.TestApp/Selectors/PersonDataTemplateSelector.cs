using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Toolkit.TestApp.Selectors
{
    public class PersonDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HockeyPlayerDataTemplate { get; set; }

        public DataTemplate GoaltenderDataTemplate { get; set; }

        public DataTemplate DriverDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is HockeyPlayerViewModel)
            {
                return HockeyPlayerDataTemplate;
            }
            else if (item is GoaltenderViewModel)
            {
                return GoaltenderDataTemplate;
            }
            else if (item is DriverViewModel)
            {
                return DriverDataTemplate;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
