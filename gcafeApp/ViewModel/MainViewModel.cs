using System;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
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

        public ICommand MethodCommand 
        {
            get { return _methodCommand; }
            private set
            {
                _methodCommand = value;
                RaisePropertyChanged();
            }
        }
        private ICommand _methodCommand;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(/*IgcafeSvcClient svc*/)
        {
            System.Diagnostics.Debug.WriteLine("========================================================");

            MethodCommand = new MethodCommand();

            if (!IsInDesignMode)
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

    }

    public class MethodCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            int i = 0;
        }
    }
}