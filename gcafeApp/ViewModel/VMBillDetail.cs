using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
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
                if (_orderId != value)
                {
                    _orderId = value;

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
    }
}
