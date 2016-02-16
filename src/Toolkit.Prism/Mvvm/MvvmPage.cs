using Prism.Windows.Mvvm;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Toolkit.Prism.Mvvm
{
    public class MvvmPage : SessionStateAwarePage, INotifyPropertyChanged
    {
        public MvvmPage()
        {
            ViewModelLocator.SetAutoWireViewModel(this, true);
            DataContextChanged += MvvmPage_DataContextChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void MvvmPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConcreteDataContext"));
        }
    }
}
