using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace $rootnamespace$
{
    public sealed partial class $safeitemname$ : MvvmPage
    {
        public $safeitemname$()
        {
            InitializeComponent();
        }

        public $safeitemname$ViewModel ConcreteDataContext
        {
            get
            {
                return DataContext as $safeitemname$ViewModel;
            }
        }
    }
}