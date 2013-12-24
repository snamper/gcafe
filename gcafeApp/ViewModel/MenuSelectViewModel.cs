using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace gcafeApp.ViewModel
{
    public class MenuSelectViewModel : VMBase
    {
        private RelayCommand<string> _navigateToCommand;

        public MenuSelectViewModel()
        {
            _navigateToCommand = new RelayCommand<string>(NavigateTo);
        }

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


        public string Pro1
        {
            get;
            set;
        }
    }
}
