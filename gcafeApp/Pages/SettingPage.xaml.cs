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
    public partial class SettingPage : PhoneApplicationPage
    {
        public SettingPage()
        {
            InitializeComponent();

            tbUrl.Text = Settings.AppSettings.ServiceURL;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            Settings.AppSettings.ServiceURL = tbUrl.Text;

            NavigationService.GoBack();
        }
    }
}