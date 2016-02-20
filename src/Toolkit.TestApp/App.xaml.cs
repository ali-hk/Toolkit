using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows;
using Prism.Windows.AppModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Toolkit.Common.Strings;
using Toolkit.TestApp.Repositories;
using Toolkit.TestApp.Services;
using Toolkit.Uwp;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Toolkit.TestApp
{
    public sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.Resolve<AppShell>();
            shell.SetContentFrame(rootFrame);
            return shell;
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterType<IDriverService, DriverServiceProxy>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IHockeyPlayerService, HockeyPlayerServiceProxy>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAthleteRepository, AthleteRepository>(new ContainerControlledLifetimeManager());

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                if (viewName.Contains(".Views."))
                {
                    viewName = viewName.Replace(".Views.", ".PageViewModels.");
                }
                else
                {
                    throw new ArgumentException($"The specified View type {viewName} isn't in the Views namespace.");
                }

                ////var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName.Replace("TestApp", "TestApp.ViewModel");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = StringHelper.InvariantCulture($"{viewName}{suffix}, {viewAssemblyName}");
                return Type.GetType(viewModelName);
            });

            return base.OnInitializeAsync(args);
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate(nameof(PageTokens.DTSBehaviorSample), null);
            CoreWindow.GetForCurrentThread().Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
            return Task.FromResult(true);
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType == CoreAcceleratorKeyEventType.KeyUp && args.VirtualKey == VirtualKey.F)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                Debug.Write(MemoryUsageHelper.DumpMemoryUsage());
            }
        }
    }
}
