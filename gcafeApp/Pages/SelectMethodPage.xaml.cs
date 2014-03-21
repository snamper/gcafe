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
    public partial class SelectMethodPage : PhoneApplicationPage
    {
        public SelectMethodPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
                if (btn.Text == "确定")
                {

                    List<ViewModel.Method> methods = new List<ViewModel.Method>();
                    foreach (var method in MenuCatalog.SelectedItems)
                    {
                        methods.Add(new ViewModel.Method() { Id = ((ViewModel.Method)method).Id, Name = ((ViewModel.Method)method).Name });
                    }
                    PhoneApplicationService.Current.State["SelectedMethods"] = methods;

                    //PhoneApplicationService.Current.State["SelectedMethods"] = MenuCatalog.SelectedItems;
                    NavigationService.GoBack();
                }
                else
                {
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }
    }
}