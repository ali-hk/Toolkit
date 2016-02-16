﻿using System;
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
    public sealed partial class MenuView : MvvmUserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        public MenuViewModel ConcreteDataContext
        {
            get
            {
                return DataContext as MenuViewModel;
            }
        }
    }
}
