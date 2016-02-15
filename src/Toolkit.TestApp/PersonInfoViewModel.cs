using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Toolkit.TestApp
{
    public class PersonInfoViewModel
    {
        public PersonInfoViewModel()
        {
            ShowMessageCommand = new DelegateCommand<object>(ShowMessageAction);
        }

        public string Name { get; internal set; }

        public string Email { get; internal set; }

        public ICommand ShowMessageCommand { get; private set; }

        private async void ShowMessageAction(object param)
        {
            var dialog = new MessageDialog($"You clicked item {param.ToString()}");
            await dialog.ShowAsync();
        }
    }
}
