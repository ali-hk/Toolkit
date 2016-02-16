using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Toolkit.Prism.Mvvm
{
    public class MvvmUserControl : UserControl, INotifyPropertyChanged
    {
        public MvvmUserControl()
        {
            ViewModelLocator.SetAutoWireViewModel(this, true);
            DataContextChanged += MvvmUserControl_DataContextChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void MvvmUserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConcreteDataContext"));
        }
    }
}
