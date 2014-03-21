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
    public partial class MenuItemPage : PhoneApplicationPage
    {
        public MenuItemPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                string value;
                if (NavigationContext.QueryString.TryGetValue("CatalogID", out value))
                {
                    int id;
                    if (Int32.TryParse(value, out id))
                    {
                        ViewModel.VMMenuItem ctx = (ViewModel.VMMenuItem)DataContext;
                        ctx.CatalogID = id;
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
            try
            {
                PhoneApplicationService.Current.State["SelectedMenuItem"] = e.AddedItems[0];

                App.RootFrame.RemoveBackEntry();
                App.RootFrame.RemoveBackEntry();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

    }
}