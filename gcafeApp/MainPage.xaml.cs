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

namespace gcafeApp
{
    public partial class MainPage : PhoneApplicationPage
    {
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
            int i = 0;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
            if (btn.Text == "确定")
            {
                ViewModel.MainViewModel mv = (ViewModel.MainViewModel)DataContext;
                mv.OrderMeals(OrderCtrl.TableNum, ShowMsg);
            }
            else
            {
                OrderCtrl.TableNum = "请点击这里选择台号";
            }
        }

        private void TableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)e.AddedItems[0];
            string header = (string)pivotItem.Header;

            if (header == "账单")
                ((ViewModel.MainViewModel)DataContext).GetOpenedTables();
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