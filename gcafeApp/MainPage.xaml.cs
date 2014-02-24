using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using gcafeApp.Resources;
using Microsoft.Practices.ServiceLocation;

namespace gcafeApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        ApplicationBarIconButton _btnConfirm = null;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 将 listbox 控件的数据上下文设置为示例数据
            //DataContext = App.ViewModel;

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();

            //object o = this.Resources["mydata"];
        }

        // 为 ViewModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.RootFrame.BackStack.Count() > 0)
                App.RootFrame.RemoveBackEntry();

            ViewModel.MainViewModel mv = (ViewModel.MainViewModel)DataContext;
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}

            if (PhoneApplicationService.Current.State.ContainsKey("SelectedMenuItem"))
            {
                mv.MenuItems.Add((gcafeSvc.MenuItem)PhoneApplicationService.Current.State["SelectedMenuItem"]);

                PhoneApplicationService.Current.State.Remove("SelectedMenuItem");
            }

            if (PhoneApplicationService.Current.State.ContainsKey("SelectedMethods"))
            {
                List<ViewModel.Method> methods = (List<ViewModel.Method>)PhoneApplicationService.Current.State["SelectedMethods"];
                mv.SetMethods(methods);

                PhoneApplicationService.Current.State.Remove("SelectedMethods");
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确定退出登录吗？", "退出", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Settings.AppSettings.LoginStaff = null;
                ViewModel.MainViewModel mv = (ViewModel.MainViewModel)DataContext;
                mv.CancelOrder();
                App.RootFrame.Navigate(new Uri("/Pages/AuthenticationPage.xaml", UriKind.Relative));
            }

            e.Cancel = true;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            Settings.AppSettings.LoginStaff = null;
            App.RootFrame.Navigate(new Uri("/Pages/AuthenticationPage.xaml", UriKind.Relative));
        }

        private void ExpanderView_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
        }

        private void ShowMsg(string msg)
        {
            if ((msg == "未输入台号") || (msg == "未输入菜品"))
            {
                MessageBox.Show(msg);
            }
            else
            {
                MessageBox.Show("点菜成功");

                OrderCtrl.TableNum = "请点击这里选择台号";
                ViewModel.MainViewModel mv = (ViewModel.MainViewModel)DataContext;
                mv.CancelOrder();
            }

            _btnConfirm.IsEnabled = true;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
            ViewModel.MainViewModel mv = (ViewModel.MainViewModel)DataContext;

            if (btn.Text == "确定")
            {
                _btnConfirm = btn;

                btn.IsEnabled = false;
                mv.OrderMeals(OrderCtrl.TableNum, ShowMsg);
            }
            else
            {
                mv.CancelOrder();
                OrderCtrl.TableNum = "请点击这里选择台号";
            }
        }

        private void TableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TableList.SelectedItem == null)
                return;

            string orderNum = ((ViewModel.TableViewModel)e.AddedItems[0]).OrderNum;
            //((ViewModel.ViewModelLocator)App.Current.Resources["Locator"]).VMBillDetail.OrderId = Int32.Parse(orderNum.Length > 7 ? orderNum.Substring(2) : orderNum);
            //((ViewModel.ViewModelLocator)App.Current.Resources["Locator"]).VMBillDetail.OrderDetail = (ViewModel.TableViewModel)e.AddedItems[0];

            ServiceLocator.Current.GetInstance<ViewModel.VMBillDetail>().OrderId = Int32.Parse(orderNum.Length > 7 ? orderNum.Substring(2) : orderNum);
            ServiceLocator.Current.GetInstance<ViewModel.VMBillDetail>().OrderDetail = (ViewModel.TableViewModel)e.AddedItems[0];

            App.RootFrame.Navigate(new Uri("/Pages/BillDetailPage.xaml", UriKind.Relative));

            TableList.SelectedItem = null;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)e.AddedItems[0];
            string header = (string)pivotItem.Header;

            if (header == "账单")
                ((ViewModel.MainViewModel)DataContext).GetOpenedTables();
        }

        private void GestureListener_Tap(object sender, GestureEventArgs e)
        {
            Type t = sender.GetType();

            //ContextMenu contextMenu = ContextMenuService.GetContextMenu()
        }

        private void GestureListener_Tap_1(object sender, GestureEventArgs e)
        {
            Type t = sender.GetType();
        }



        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}