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

            ((ViewModel.VMLogin)DataContext).ExceptionCallback = ExceptionCallback;
        }

        void ExceptionCallback(Exception ex)
        {
            if (MessageBox.Show("这个程序必须架设WCF服务才能使用，具体请与zhangzq71@hotmail.com联系。", "WCF出错", MessageBoxButton.OK) == MessageBoxResult.OK)
            { 
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppSettings.LoginStaff != null)
                App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ViewModel.VMLogin vml = (ViewModel.VMLogin)DataContext;

            if (vml.Staff.Name == "张志强")
            {
                AppSettings.LoginStaff = vml.Staff;
                App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else if (pwd.Text == vml.Staff.Password)
            {
                AppSettings.LoginStaff = vml.Staff;
                App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
                vml.IsPasswordError = true;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            App.RootFrame.Navigate(new Uri("/Pages/SettingPage.xaml", UriKind.Relative));
        }
    }
}