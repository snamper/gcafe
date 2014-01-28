using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace gcafeApp.Pages
{
    public partial class MenuOptionPage : PhoneApplicationPage
    {
        public MenuOptionPage()
        {
            InitializeComponent();

        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ViewModel.ViewModelLocator)App.Current.Resources["Locator"]).Main.SelectedOptionMenu = (gcafeSvc.SetmealItem)e.AddedItems[0];
            NavigationService.GoBack();
        }
    }
}