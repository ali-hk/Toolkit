using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Toolkit.Prism.Mvvm;
using Toolkit.TestApp.PageViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Toolkit.TestApp.Views
{
    public sealed partial class DeferLoadStrategySamplePage : MvvmPage
    {
        public DeferLoadStrategySamplePage()
        {
            InitializeComponent();
        }

        public DeferLoadStrategySamplePageViewModel ConcreteDataContext
        {
            get
            {
                return DataContext as DeferLoadStrategySamplePageViewModel;
            }
        }

        private void DeferLoadFindNameButton_Click(object sender, RoutedEventArgs e)
        {
            FindName(nameof(DeferLoadFindNameGridView));
        }

        private void DeferLoadVSMButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, nameof(ListViewRealized), true);
        }
    }
}
