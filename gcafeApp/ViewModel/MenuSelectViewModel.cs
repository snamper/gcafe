using System;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class MenuSelectViewModel : VMBase
    {
        gcafeSvc.IgcafeSvcClient _svc; 
        private RelayCommand<string> _navigateToCommand;

        public MenuSelectViewModel(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _navigateToCommand = new RelayCommand<string>(NavigateTo);

                IsBusy = true;
                _svc = svc;
                _svc.GetMenuItemsByCatalogIdCompleted += _svc_GetMenuItemsByCatalogIdCompleted;
                _svc.GetMenuItemsByCatalogIdAsync("", -1, "MenuSelectViewModel");
            }
        }

        void _svc_GetMenuItemsByCatalogIdCompleted(object sender, GetMenuItemsByCatalogIdCompletedEventArgs e)
        {
            if (ReferenceEquals(e.UserState, "MenuSelectViewModel"))
            {
                _allMenuItems = new List<MenuItem>(e.Result);
                IsBusy = false;
            }
        }

        public MenuItem MenuItem
        {
            get { return _menuItem; }
            set
            {
                if (!ReferenceEquals(value, _menuItem))
                {
                    _menuItem = value;
                    RaisePropertyChanged();
                }
            }
        }
        private MenuItem _menuItem;

        public ICommand NavigateToCommand
        {
            get { return _navigateToCommand; }
        }

        private void NavigateTo(string type)
        {
            if (type == "CataSel")
                App.RootFrame.Navigate(new Uri("/Pages/MenuCatalogPage.xaml", UriKind.Relative));
            else if (type == "CameraSel")
                App.RootFrame.Navigate(new Uri("/Pages/MenuCameraSelect.xaml", UriKind.Relative));
        }

        public List<MenuItem> AllMenuItems
        {
            get { return _allMenuItems; }
            set
            {
                if (!ReferenceEquals(_allMenuItems, value))
                {
                    _allMenuItems = value;
                    RaisePropertyChanged();
                }
            }
        }
        private List<MenuItem> _allMenuItems;

        public string Pro1
        {
            get;
            set;
        }
    }
}
