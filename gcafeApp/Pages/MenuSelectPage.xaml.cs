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
        private OpticalReaderLib.IProcessor _processor = null;
        private OpticalReaderLib.OpticalReaderTask _task = null;
        private OpticalReaderLib.OpticalReaderResult _taskResult = null;

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            try
            {
                if (!e.Uri.OriginalString.Contains("Reader"))
                {
                    if (_task != null)
                    {
                        _task.Completed -= _task_Completed;
                        _task.Dispose();
                        _task = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                if (_taskResult != null)
                {
                    ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;

                    if (!string.IsNullOrEmpty(_taskResult.Text))
                    {
                        if (_taskResult.Text.Substring(0, 2) == "11" ||
                            _taskResult.Text.Substring(0, 2) == "22")
                        {
                            AllItems.Text = _taskResult.Text;
                            vm.GetMenuItemByNumber(_taskResult.Text);
                        }
                        else
                        {
                            vm.MenuItem = null;
                            AllItems.Text = string.Empty;
                        }
                    }
                    else
                    {
                        vm.MenuItem = null;
                        AllItems.Text = string.Empty;
                    }

                    _taskResult = null;
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;

            if (btn.Text == "确定")
            {
                ViewModel.MenuSelectViewModel vm = (ViewModel.MenuSelectViewModel)DataContext;
                if (vm.MenuItem != null)
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

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_task != null)
            {
                _task.Completed += _task_Completed;
                _task.Dispose();
                _task = null;
            }

            _processor = new OpticalReaderLib.ZxingProcessor();

            _task = new OpticalReaderLib.OpticalReaderTask()
            {
                Processor = _processor,
                ShowDebugInformation = false,
                FocusInterval = new TimeSpan(0, 0, 0, 0, 1200),
                ObjectSize = new Windows.Foundation.Size(30, 30),
                RequireConfirmation = false
            };

            _task.Completed += _task_Completed;
            _task.Show();

        }

        void _task_Completed(object sender, OpticalReaderLib.OpticalReaderResult e)
        {
            _taskResult = e;

            System.Diagnostics.Debug.WriteLine(e.Text);
        }
    }
}