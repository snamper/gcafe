using System;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : VMBase
    {
        private readonly IgcafeSvcClient _svc;
        private MenuItem _methodMenuItem;
        private SetmealItem _methodSetmealItem;
        private Action<string> _callBack;

        public MainViewModel(IgcafeSvcClient svc)
        {
            //MethodCommand = new MethodCommand(this);

            if (IsInDesignMode)
            {
                //_svc = svc;
                List<SetmealItem> setMeals = new List<SetmealItem>();
                setMeals.Add(new SetmealItem() { Name = "火龙果" });
                setMeals.Add(new SetmealItem() { Name = "芒果饮" });
                setMeals.Add(new SetmealItem() { Name = "牛扒" });

                List<MenuItem> menuItems = new List<MenuItem>();
                menuItems.Add(new MenuItem()
                    {
                        ID = 1,
                        Name = "火龙果芒果饮",
                        Price = (decimal)12.01,
                        Unit = "杯",
                        Quantity = 1,
                        IsSetmeal = true,
                        SetmealItems = new ObservableCollection<SetmealItem>(setMeals),
                    });

                menuItems.Add(new MenuItem()
                {
                    ID = 2,
                    Name = "开心大餐",
                    Price = (decimal)123.12,
                    Unit = "份",
                    Quantity = 1,
                    IsSetmeal = false,
                });

                MenuItems = new ObservableCollection<MenuItem>(menuItems);
            }
            else
            {
                _svc = svc;
                _svc.OrderMealCompleted += _svc_OrderMealCompleted;
                _svc.GetTablesInfoCompleted += _svc_GetTablesInfoCompleted;
                _svc.GetOrderDetailByOrderNumCompleted += _svc_GetOrderDetailByOrderNumCompleted;

                MenuItems = new ObservableCollection<MenuItem>();
            }
        }

        void _svc_GetOrderDetailByOrderNumCompleted(object sender, GetOrderDetailByOrderNumCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=========================== ByOrderNum Completed ===============================");

            foreach (var menuItem in e.Result)
            {
                //System.Diagnostics.Debug.WriteLine(menuItem.Name);
                if (menuItem.Methods != null)
                {
                    foreach (var method in menuItem.Methods)
                    {
                        System.Diagnostics.Debug.WriteLine(method.Name);
                    }
                }

                if (menuItem.SetmealItems != null)
                {
                    foreach (var setmeal in menuItem.SetmealItems)
                    {
                        //System.Diagnostics.Debug.WriteLine(setmeal.Name);
                    }
                }
            }
        }

        void _svc_GetTablesInfoCompleted(object sender, GetTablesInfoCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    List<TableViewModel> vmList = new List<TableViewModel>();
                    foreach (TableInfo tbl in e.Result)
                    {
                        vmList.Add(new TableViewModel()
                        {
                            OrderNum = tbl.OrderNum,
                            TableNo = tbl.Num,
                            CustomerNum = tbl.CustomerNum,
                            OpenTableStaff = tbl.OpenTableStaff.Name,
                            TableOpenedTime = tbl.OpenTableTime,
                            Amount = tbl.Amount,
                        });
                    }

                    this.OpenedTables = new ObservableCollection<TableViewModel>(vmList);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            IsBusy = false;
        }

        void _svc_OrderMealCompleted(object sender, OrderMealCompletedEventArgs e)
        {
            IsBusy = false;
            _callBack(e.Result);
        }

        public RelayCommand<object> MenuOptionCommand
        {
            get
            {
                if (_menuOptionCommand == null)
                    _menuOptionCommand = new RelayCommand<object>(OnMenuOptionCommand);

                return _menuOptionCommand;
            }
        }
        private RelayCommand<object> _menuOptionCommand;

        private void OnMenuOptionCommand(object param)
        {
            ViewModel.VMMenuOption vm = ((ViewModelLocator)App.Current.Resources["Locator"]).VMMenuOption;

            vm.OptionItems = new List<SetmealItem>(((SetmealItem)param).OptionItems);

            //((ViewModelLocator)App.Current.Resources["Locator"]).VMMenuOption.OptionItems = (List<SetmealItem>)param;

            App.RootFrame.Navigate(new Uri("/Pages/MenuOptionPage.xaml", UriKind.Relative));
        }

        public RelayCommand<object> MethodCommand
        {
            get
            {
                if (_methodCommand == null)
                    _methodCommand = new RelayCommand<object>(OnMethodCommand);

                return _methodCommand;
            }
        }
        private RelayCommand<object> _methodCommand;

        private void OnMethodCommand(object param)
        {
            if (param.GetType() == typeof(MenuItem))
                _methodMenuItem = (MenuItem)param;
            else if (param.GetType() == typeof(SetmealItem))
                _methodSetmealItem = (SetmealItem)param;

            App.RootFrame.Navigate(new Uri("/Pages/SelectMethodPage.xaml", UriKind.Relative));
        }

        public RelayCommand<object> DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                    _deleteCommand = new RelayCommand<object>(OnDeleteCommand);

                return _deleteCommand;
            }
        }
        private RelayCommand<object> _deleteCommand;

        void OnDeleteCommand(object param)
        {
            MenuItem menuItem = null;

            if (param.GetType() == typeof(SetmealItem))
            {
                foreach (var mi in MenuItems)
                {
                    if (mi.SetmealItems != null && mi.SetmealItems.Count > 0)
                    {
                        foreach (var si in mi.SetmealItems)
                        {
                            if (si == (SetmealItem)param)
                            {
                                menuItem = mi;
                                break;
                            }
                        }
                    }
                }
            }
            else
                menuItem = (MenuItem)param;

            MenuItems.Remove(menuItem);
        }

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set
            {
                if (!ReferenceEquals(value, _menuItems))
                {
                    _menuItems = value;
                    RaisePropertyChanged();
                }
            }
        }
        private ObservableCollection<MenuItem> _menuItems;

        public void SetMethods(List<ViewModel.Method> methods)
        {
            List<gcafeSvc.Method> ms = new List<gcafeSvc.Method>();
            foreach (var method in methods)
                ms.Add(new gcafeSvc.Method() { ID = method.Id, Name = method.Name });

            if (_methodMenuItem != null)
            {
                _methodMenuItem.Methods = new ObservableCollection<gcafeSvc.Method>(ms);
            }
            else if (_methodSetmealItem != null)
            {
                _methodSetmealItem.Methods = new ObservableCollection<gcafeSvc.Method>(ms);
            }

            _methodMenuItem = null;
            _methodSetmealItem = null;
        }

        public void OrderMeals(string tableNum, Action<string> callback)
        {
            if (tableNum == "请点击这里选择台号" || tableNum.Length < 1)
            {
                callback("未输入台号");
            }
            else if (MenuItems.Count < 1)
            {
                callback("未输入菜品");
            }
            else
            {
                if (MenuItems.Count > 0)
                {
                    IsBusy = true;
                    _callBack = callback;
                    _svc.OrderMealAsync(gcafeApp.Settings.AppSettings.DeviceID, gcafeApp.Settings.AppSettings.LoginStaff.ID, new TableInfo() { Num = tableNum }, MenuItems);
                }
            }
        }

        public void CancelOrder()
        {
            MenuItems = new ObservableCollection<MenuItem>();
        }

        public ObservableCollection<TableViewModel> OpenedTables
        {
            get { return _openedTables; }
            private set
            {
                if (!ReferenceEquals(_openedTables, value))
                {
                    _openedTables = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<TableViewModel> _openedTables;

        public void GetOpenedTables()
        {
            IsBusy = true;
            _svc.GetTablesInfoAsync(Settings.AppSettings.DeviceID);
            //_svc.GetOrderDetailByOrderNumAsync("201412010001");
        }
    }

 }