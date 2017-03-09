using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.Behaviors;
using Toolkit.Uwp.Collections;
using Windows.System;
using Windows.UI.Popups;

namespace Toolkit.TestApp.PageViewModels
{
    public class OldMainPageViewModel : BindableBase
    {
        private string _searchTerm;
        private bool _hasMoreItemsEnabled = true;

        public OldMainPageViewModel()
        {
            People = new VirtualizingCollection<PersonInfoViewModel>(HasMoreItems, LoadMoreItems);
            GenerateDummyData();

            ShowMessageCommand = new DelegateCommand<object>(ShowMessageAction);

            ContextMenuItems = ContextMenuItemsDelegate;
        }

        public VirtualizingCollection<PersonInfoViewModel> People { get; private set; }

        public ICommand ShowMessageCommand { get; private set; }

        public Func<object, IReadOnlyCollection<IContextMenuItem>> ContextMenuItems { get; private set; }

        public Func<VirtualKey, ScrollDirection> ScrollDirectionDelegate => key => { return key == VirtualKey.Q ? ScrollDirection.Up : (key == VirtualKey.E ? ScrollDirection.Down : ScrollDirection.None); };

        public bool HasMoreItemsEnabled
        {
            get
            {
                return _hasMoreItemsEnabled;
            }

            set
            {
                SetProperty(ref _hasMoreItemsEnabled, value);
            }
        }

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

            int counter = 0;
            for (int i = 0; i < 6; i++)
            {
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Alex Del Piero",
                    Email = "alexdelpiero@juventus.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Wayne Gretzky",
                    Email = "waynegretzky@nhl.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Bill Gates",
                    Email = "billg@microsoft.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Peter Forsberg",
                    Email = "peterforsberg@nhl.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Gianluigi Buffon",
                    Email = "buffon@juventus.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Henrik Lundqvist",
                    Email = "theking@nhl.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Enzo Ferrari",
                    Email = "enzo@ferrari.com"
                });
                People.Add(new PersonInfoViewModel
                {
                    Name = $"{counter++} Dino Ferrari",
                    Email = "dino@ferrari.com"
                });
            }
        }

        private void AddItem(int i)
        {
            People.Add(new PersonInfoViewModel { Name = $"Person {i}", Email = $"person{i}@somewhere.com" });
        }

        private Task<IEnumerable<PersonInfoViewModel>> LoadMoreItems(uint count, CancellationToken cancellationToken)
        {
            var newItems = new List<PersonInfoViewModel>();
            for (int i = 0; i < count; i++)
            {
                newItems.Add(new PersonInfoViewModel { Name = $"Person {i}", Email = $"person{i}@somewhere.com" });
            }

            return Task.FromResult<IEnumerable<PersonInfoViewModel>>(newItems);
        }

        private bool HasMoreItems(VirtualizingCollection<PersonInfoViewModel> arg)
        {
            return HasMoreItemsEnabled;
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
                new ContextMenuItemViewModel("Buy it",
#pragma warning disable SA1118 // Parameter must not span multiple lines
                    new DelegateCommand<object>(async (param) =>
                    {
                        var dialog = new MessageDialog($"You clicked item {param.ToString()}");
                        await dialog.ShowAsync();
                    })),
#pragma warning restore SA1118 // Parameter must not span multiple lines
                new ContextMenuItemViewModel("Use it", new DelegateCommand(() => { })),
                new ContextMenuItemViewModel("Add item", new DelegateCommand(() => { AddItem(People.Count); }))
            };
        }
    }
}
