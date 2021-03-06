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
    public partial class MenuSubCatalogPage : PhoneApplicationPage
    {
        public MenuSubCatalogPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (PhoneApplicationService.Current.State.ContainsKey("SelectedMenuItem"))
                {
                    NavigationService.GoBack();
                }
                else
                {
                    string value;
                    if (NavigationContext.QueryString.TryGetValue("Catalog", out value))
                    {
                        ViewModel.VMSubMenuCatalog ctx = (ViewModel.VMSubMenuCatalog)DataContext;
                        ctx.Catalog = value;
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gcafeSvc.MenuCatalog menuCatalog = (gcafeSvc.MenuCatalog)e.AddedItems[0];
            App.RootFrame.Navigate(new Uri(string.Format("/Pages/MenuItemPage.xaml?CatalogID={0}", menuCatalog.ID), UriKind.Relative));
        }
    }
}