using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Toolkit.Behaviors
{
    public interface IContextMenuItem
    {
        string Title { get; }

        ICommand Command { get; }
    }
}
