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
    public partial class MenuSelectPage : PhoneApplicationPage
    {
        public MenuSelectPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("SelectedMenuItem"))
            {
                object o = PhoneApplicationService.Current.State["SelectedMenuItem"];
                PhoneApplicationService.Current.State.Remove("SelectedMenuItem");
            }
        }
    }
}