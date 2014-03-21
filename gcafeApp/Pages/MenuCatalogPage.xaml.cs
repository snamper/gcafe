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
    public partial class MenuCatalogPage : PhoneApplicationPage
    {
        public MenuCatalogPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                if (PhoneApplicationService.Current.State.ContainsKey("SelectedMenuItem"))
                    NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }
    }
}