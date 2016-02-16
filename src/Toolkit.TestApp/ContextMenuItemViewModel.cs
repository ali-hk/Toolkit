using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Xaml.ContextMenu;
using System.Windows.Input;

namespace Toolkit.TestApp
{
    public class ContextMenuItemViewModel : BindableBase, IContextMenuItem
    {
        public ContextMenuItemViewModel(string title, ICommand command)
        {
            Title = title;
            Command = command;
        }

        public ICommand Command { get; }

        public string Title { get; }
    }
}
