using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class VMBillDetail : VMBase
    {
        private readonly IgcafeSvcClient _svc;

        public VMBillDetail()
        {

        }

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
