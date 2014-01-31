using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;
using GalaSoft.MvvmLight.Messaging;

namespace gcafeApp.ViewModel
{
    public class VMBilling : VMBase
    {
        IgcafeSvcClient _svc;

        public VMBilling(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;
            }
        }

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

    }
}
