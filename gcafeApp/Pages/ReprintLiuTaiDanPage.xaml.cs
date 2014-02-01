using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
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

        private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ViewModel.VMPrintLiuTaiDan vm = (ViewModel.VMPrintLiuTaiDan)DataContext;

                string selectedItem = (string)e.AddedItems[0];
                if (selectedItem == "所有留台单")
                {
                    vm.PrnType = 0;
                }
                else
                {
                    string regexTxt = @"\.*(\d)\.*";
                    Regex cntRegex = new Regex(regexTxt, RegexOptions.None);
                    Match match = cntRegex.Match(selectedItem);
                    vm.PrnType = Int32.Parse(match.Value);
                }
            }
        }
    }
}