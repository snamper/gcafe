using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using gcafeApp.Settings;

namespace gcafeApp.Pages
{
    public partial class AuthenticationPage : PhoneApplicationPage
    {
        public AuthenticationPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppSettings.LoginStaff != null)
                App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ViewModel.VMLogin vml = (ViewModel.VMLogin)DataContext;
            if (pwd.Password == vml.Staff.Password)
            {
                AppSettings.LoginStaff = vml.Staff;
                App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}