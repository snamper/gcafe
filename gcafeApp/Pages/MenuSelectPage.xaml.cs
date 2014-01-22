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

            AllItems.TextFilter += SearchItem;
            AllItems.TextChanged += AllItems_TextChanged;
            AllItems.SelectionChanged += AllItems_SelectionChanged;
        }

        void AllItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.RemovedItems.Count > 0)
                    return;

                if (e.AddedItems.Count > 0)
                {
                    ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;
                    vm.MenuItem = (gcafeSvc.MenuItem)e.AddedItems[0];
                    //AllItems.Text = vm.MenuItem.Name;
                }
            }
            catch (Exception ex)
            {

            }
        }        

        protected bool isNumberic(string message)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");

            if (rex.IsMatch(message))
                return true;
            else
                return false;
        }

        void AllItems_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AllItems.Text.Length == 6 && isNumberic(AllItems.Text))
                {
                    ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;
                    vm.GetMenuItemByNumber(AllItems.Text);
                }
            }
            catch (Exception ex)
            {

            }
        }

        bool SearchItem(string search, string value)
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("filter: {0}, {1}", search, value));

            if (!string.IsNullOrEmpty(search))
            {
                if (search.Length > 2)
                {
                    if (value.StartsWith(search))
                        return true;
                }
            }

            return false;
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