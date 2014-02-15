using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class VMBillDetail : VMBase
    {
        private readonly IgcafeSvcClient _svc;

        public VMBillDetail(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;

                _svc.GetOrderDetailByOrderNumCompleted += _svc_GetOrderDetailByOrderNumCompleted;
            }
        }

        void _svc_GetOrderDetailByOrderNumCompleted(object sender, GetOrderDetailByOrderNumCompletedEventArgs e)
        {
            MenuItems = new List<MenuItem>(e.Result);
            IsBusy = false;
        }

        public List<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set
            {
                if (!ReferenceEquals(_menuItems, value))
                {
                    _menuItems = value;

                    RaisePropertyChanged();
                }
            }
        }
        List<MenuItem> _menuItems;

        public TableViewModel OrderDetail
        {
            get { return _orderDetail; }
            set
            {
                if (!ReferenceEquals(_orderDetail, value))
                {
                    _orderDetail = value;

                    RaisePropertyChanged();
                }
            }
        }
        TableViewModel _orderDetail;

        public int OrderId
        {
            get { return _orderId; }
            set
            {
                //if (_orderId != value)
                {
                    _orderId = value;

                    IsBusy = true;
                    MenuItems = new List<MenuItem>();
                    _svc.GetOrderDetailByOrderNumAsync(_orderId.ToString());
                }
            }
        }
        int _orderId;

        public ObservableCollection<MenuItem> OrderDetails
        {
            get { return _orderDetails; }
            set
            {
                if (!ReferenceEquals(_orderDetails, value))
                {
                    _orderDetails = value;
                    RaisePropertyChanged();
                }
            }
        }
        private ObservableCollection<MenuItem> _orderDetails;

        public RelayCommand<object> OrderDetailCommand
        {
            get
            {
                if (_orderDetailCommand == null)
                {
                    _orderDetailCommand = new RelayCommand<object>(o =>
                    {
                        Type t = o.GetType();
                        int i = 0;
                    });
                }

                return _orderDetailCommand;
            }
        }
        public RelayCommand<object> _orderDetailCommand;

        public RelayCommand<object> ReprintChupinDan
        {
            get
            {
                if (_reprintChupinDan == null)
                {
                    _reprintChupinDan = new RelayCommand<object>(menuItem =>
                    {
                        ObservableCollection<gcafeSvc.MenuItem> param = new ObservableCollection<gcafeSvc.MenuItem>();

                        if (menuItem.GetType() == typeof(gcafeSvc.MenuItem))
                            param.Add((gcafeSvc.MenuItem)menuItem);
                        else
                            param.Add(((gcafeSvc.SetmealItem)menuItem).MenuItem);

                        _svc.ReprintChupinDanAsync(OrderDetail.OrderNum, param);
                        
                    });
                }

                return _reprintChupinDan;
            }
        }
        RelayCommand<object> _reprintChupinDan;
    }
}
