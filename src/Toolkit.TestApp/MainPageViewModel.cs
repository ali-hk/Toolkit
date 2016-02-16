using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.Xaml.ContextMenu;
using Windows.UI.Popups;

namespace Toolkit.TestApp
{
    public class MainPageViewModel : BindableBase
    {
        private string _searchTerm;

        public MainPageViewModel()
        {
            People = new ObservableCollection<PersonInfoViewModel>();
            GenerateDummyData();

            ShowMessageCommand = new DelegateCommand<object>(ShowMessageAction);

            ContextMenuItems = ContextMenuItemsDelegate;
        }

        public ObservableCollection<PersonInfoViewModel> People { get; private set; }

        public ICommand ShowMessageCommand { get; private set; }

        public Func<object, IReadOnlyCollection<IContextMenuItem>> ContextMenuItems { get; private set; }

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }

        private void GenerateDummyData()
        {
            for (int i = 0; i < 100; i++)
            {
                AddItem(i);
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
