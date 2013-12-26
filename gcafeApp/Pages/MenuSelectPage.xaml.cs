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

            ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;

            if (PhoneApplicationService.Current.State.ContainsKey("SelectedMenuItem"))
            {
                vm.MenuItem = PhoneApplicationService.Current.State["SelectedMenuItem"] as gcafeSvc.MenuItem;
                vm.MenuItem.Quantity = 1;
                PhoneApplicationService.Current.State.Remove("SelectedMenuItem");
            }
            else
            {
                vm.MenuItem = null;
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;

            if (btn.Text == "确定")
            {
                ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;
                PhoneApplicationService.Current.State["SelectedMenuItem"] = vm.MenuItem;
                NavigationService.GoBack();

                //int quantity;
                //if (Int32.TryParse(Quantity.Text, out quantity))
                //{
                //    vm.MenuItem.Quantity = quantity;
                //    PhoneApplicationService.Current.State["SelectedMenuItem"] = vm.MenuItem;
                //    NavigationService.GoBack();
                //}
                //else
                //{

                //}
            }
            else
            {
                if (PhoneApplicationService.Current.State.ContainsKey("SelectedMenuItem"))
                    PhoneApplicationService.Current.State.Remove("SelectedMenuItem");

                NavigationService.GoBack();
            }

        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;

            vm.MenuItem.Quantity++;
        }

        private void ButtonMinu_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;

            if (vm.MenuItem.Quantity > 1)
                vm.MenuItem.Quantity--;
        }
    }
}