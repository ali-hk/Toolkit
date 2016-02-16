﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.Behaviors;
using Toolkit.Collections;
using Toolkit.Xaml.ContextMenu;
using Windows.System;
using Windows.UI.Popups;

namespace Toolkit.TestApp
{
    public class MainPageViewModel : BindableBase
    {
        private string _searchTerm;

        public MainPageViewModel()
        {
            People = new VirtualizingCollection<PersonInfoViewModel>();
            GenerateDummyData();

            ShowMessageCommand = new DelegateCommand<object>(ShowMessageAction);

            ContextMenuItems = ContextMenuItemsDelegate;
        }

        public VirtualizingCollection<PersonInfoViewModel> People { get; private set; }

        public ICommand ShowMessageCommand { get; private set; }

        public Func<object, IReadOnlyCollection<IContextMenuItem>> ContextMenuItems { get; private set; }

        public Func<VirtualKey, ScrollDirection> ScrollDirectionDelegate => key => { return key == VirtualKey.Q ? ScrollDirection.Up : (key == VirtualKey.E ? ScrollDirection.Down : ScrollDirection.None); };

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }

        private void GenerateDummyData()
        {
            ////for (int i = 0; i < 100; i++)
            ////{
            ////    AddItem(i);
            ////}

            for (int i = 0; i < 50; i++)
            {
                People.Add(new PersonInfoViewModel
                {
                    Name = "Alex Del Piero",
                    Email = "alexdelpiero@juventus.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = "Wayne Gretzky",
                    Email = "waynegretzky@nhl.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = "Bill Gates",
                    Email = "billg@microsoft.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = "Peter Forsberg",
                    Email = "peterforsberg@nhl.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = "Gianluigi Buffon",
                    Email = "buffon@juventus.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = "Henrik Lundqvist",
                    Email = "theking@nhl.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = "Enzo Ferrari",
                    Email = "enzo@ferrari.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = "Dino Ferrari",
                    Email = "dino@ferrari.com"
                });
            }
        }

        private void AddItem(int i)
        {
            People.Add(new PersonInfoViewModel { Name = $"Person {i}", Email = $"person{i}@somewhere.com" });
        }

        private async void ShowMessageAction(object param)
        {
            ////var personInfo = param as PersonInfoViewModel;
            var dialog = new MessageDialog($"You clicked item {param.ToString()}");
            await dialog.ShowAsync();
        }

        private IReadOnlyCollection<IContextMenuItem> ContextMenuItemsDelegate(object item)
        {
            return new List<IContextMenuItem>
            {
                new MenuItemViewModel("Buy it",
#pragma warning disable SA1118 // Parameter must not span multiple lines
                    new DelegateCommand<object>(async (param) =>
                    {
                        var dialog = new MessageDialog($"You clicked item {param.ToString()}");
                        await dialog.ShowAsync();
                    })),
#pragma warning restore SA1118 // Parameter must not span multiple lines
                new MenuItemViewModel("Use it", new DelegateCommand(() => { })),
                new MenuItemViewModel("Add item", new DelegateCommand(() => { AddItem(People.Count); }))
            };
        }
    }
}
