using $safeprojectname$.PageViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Toolkit.Prism.Mvvm;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// TODO: Sort usings and remove this Code Analysis suppression
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.Analyzers", "SA1210", Justification = "Template can't sort project's using")]

namespace $safeprojectname$.Views
{
    public sealed partial class MainPage : MvvmPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public MainPageViewModel ConcreteDataContext
        {
            get
            {
                return DataContext as MainPageViewModel;
            }
        }
    }
}
