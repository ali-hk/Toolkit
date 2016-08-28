using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Toolkit.Prism.Mvvm;
using Toolkit.TestApp.PageViewModels;
using Toolkit.TestApp.ViewModels;
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

namespace Toolkit.TestApp.Views
{
    public sealed partial class IncrementalLoadingSamplePage : MvvmPage
    {
        public IncrementalLoadingSamplePage()
        {
            InitializeComponent();
        }

        public IncrementalLoadingSamplePageViewModel ConcreteDataContext
        {
            get
            {
                return DataContext as IncrementalLoadingSamplePageViewModel;
            }
        }

        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            switch (args.Phase)
            {
                case 0:
                    args.RegisterUpdateCallback(OnContainerContentChanging);
                    break;
                case 1:
                    ApplyPhaseOne(args);
                    args.RegisterUpdateCallback(OnContainerContentChanging);
                    break;
                case 2:
                    ApplyPhaseTwo(args);
                    break;
                default:
                    break;
            }

            // Set args.Handled = true if using x:Bind to skip apply DataContext
            args.Handled = true;
        }

        private void ApplyPhaseOne(ContainerContentChangingEventArgs args)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as StackPanel;
            var hockeyPlayerVM = args.Item as HockeyPlayerViewModel;

            var numberTextBlock = templateRoot.Children[1] as TextBlock;
            numberTextBlock.Text = hockeyPlayerVM.Number.ToString();
            numberTextBlock.Opacity = 1;

            var goalsTextBlock = templateRoot.Children[4] as TextBlock;
            goalsTextBlock.Text = hockeyPlayerVM.Goals.ToString();
            goalsTextBlock.Opacity = 1;
        }

        private void ApplyPhaseTwo(ContainerContentChangingEventArgs args)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as StackPanel;
            var hockeyPlayerVM = args.Item as HockeyPlayerViewModel;
            var image = templateRoot.Children[0] as Image;
            image.Source = new BitmapImage { UriSource = new Uri(hockeyPlayerVM.Photo), DecodePixelWidth = 50, DecodePixelHeight = 50 };
            image.Opacity = 1;
        }
    }
}