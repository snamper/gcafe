using System;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
            System.Diagnostics.Debug.WriteLine("========================================================");

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

                MenuItems = new ObservableCollection<MenuItem>();
            }
        }

        void _svc_OrderMealCompleted(object sender, OrderMealCompletedEventArgs e)
        {
            _callBack(e.Result);
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
            List<method> ms = new List<method>();
            foreach (var method in methods)
                ms.Add(new method() { id = method.Id, name = method.Name });

            if (_methodMenuItem != null)
            {
                _methodMenuItem.Methods = new ObservableCollection<method>(ms);
            }
            else if (_methodSetmealItem != null)
            {
                _methodSetmealItem.Methods = new ObservableCollection<method>(ms);
            }

            _methodMenuItem = null;
            _methodSetmealItem = null;
        }

        public void OrderMeals(string tableNum, Action<string> callback)
        {
            _callBack = callback;
            _svc.OrderMealAsync(gcafeApp.Settings.AppSettings.DeviceID, gcafeApp.Settings.AppSettings.LoginStaff.ID, tableNum, MenuItems);
        }
    }

 }