using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Practices.ServiceLocation;
using gcafeApp.ViewModel;

namespace gcafeApp.Pages
{
    public partial class BillDetailPage : PhoneApplicationPage
    {
        public BillDetailPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
            //ViewModel.VMChangeTable vm = (ViewModel.VMBillDetail)DataContext;

            if (btn.Text == "账单")
            {
                ((ViewModelLocator)App.Current.Resources["Locator"]).VMBilling.MenuItems = ServiceLocator.Current.GetInstance<VMBillDetail>().MenuItems;
                ((ViewModelLocator)App.Current.Resources["Locator"]).VMBilling.OrderDetail = ServiceLocator.Current.GetInstance<VMBillDetail>().OrderDetail;
                App.RootFrame.Navigate(new Uri("/Pages/BillingPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}