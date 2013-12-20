using System;
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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(/*IgcafeSvcClient svc*/)
        {
            if (!IsInDesignMode)
            {
                //_svc = svc;

                List<SetmealItem> setMeals = new List<SetmealItem>();
                setMeals.Add(new SetmealItem() { Name = "»ðÁú¹û" });
                setMeals.Add(new SetmealItem() { Name = "Ã¢¹ûÒû" });

                List<MenuItem> menuItems = new List<MenuItem>();
                menuItems.Add(new MenuItem()
                    {
                        ID = 1,
                        Name = "»ðÁú¹ûÃ¢¹ûÒû",
                        Price = (decimal)12.01,
                        Unit = "±­",
                        IsSetmeal = true,
                        SetmealItems = new ObservableCollection<SetmealItem>(setMeals),
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
}