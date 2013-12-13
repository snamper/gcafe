using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace gcafeApp.ViewModel
{
    public class VMMenuCatalog : VMBase
    {
        private RelayCommand<string> navigateToCommand;

        public VMMenuCatalog()
        {
            navigateToCommand = new RelayCommand<string>(NavigateTo);
        }

        public ICommand NavigateToCommand
        {
            get { return navigateToCommand; }
        }

        private void NavigateTo(string cata)
        {
            App.RootFrame.Navigate(new Uri(string.Format("/Pages/MenuSubCatalogPage.xaml?Catalog={0}", cata), UriKind.Relative));
        }
    }
}
