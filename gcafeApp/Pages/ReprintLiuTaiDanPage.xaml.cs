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
    public partial class ReprintLiuTaiDanPage : PhoneApplicationPage
    {
        public ReprintLiuTaiDanPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
            ViewModel.VMPrintLiuTaiDan vm = (ViewModel.VMPrintLiuTaiDan)DataContext;

            if (btn.Text == "确定")
            {
                vm.PrintLiuTaiDan();
            }
            else
            {

            }
        }
    }
}