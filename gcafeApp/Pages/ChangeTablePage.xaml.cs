using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GalaSoft.MvvmLight.Messaging;

namespace gcafeApp.Pages
{
    public partial class ChangeTablePage : PhoneApplicationPage
    {
        public ChangeTablePage()
        {
            InitializeComponent();

            Messenger.Default.Register<string>(this, "ChangeTablePage", msg =>
            {
                if (msg == "换台成功")
                {
                    NavigationService.GoBack();
                }
                else
                {
                    tbMsg.Visibility = System.Windows.Visibility.Visible;
                }
            });
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
            ViewModel.VMChangeTable vm = (ViewModel.VMChangeTable)DataContext;

            if (btn.Text == "确定")
            {
                if (tbTableNo.Text.Length > 0)
                {
                    vm.DestTableNum = tbTableNo.Text;
                }
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}