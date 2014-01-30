using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;
using GalaSoft.MvvmLight.Messaging;

namespace gcafeApp.ViewModel
{
    public class VMPrintLiuTaiDan : VMBase
    {
        IgcafeSvcClient _svc;

        public VMPrintLiuTaiDan(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;

                _svc.GetTableOrderCountCompleted += _svc_GetTableOrderCountCompleted;
            }
        }

        void _svc_GetTableOrderCountCompleted(object sender, GetTableOrderCountCompletedEventArgs e)
        {
            OrderCount = e.Result;

            List<string> prnItems = new List<string>();
            prnItems.Add("所有留台单");
            for (int i = 0; i < OrderCount; i++)
            {
                prnItems.Add(string.Format("第{0}次点菜留台单", i + 1));
            }
            PrintItems = new List<string>(prnItems);

            IsBusy = false;
        }

        public List<string> PrintItems
        {
            get { return _printItems; }
            set
            {
                if (!ReferenceEquals(_printItems, value))
                {
                    _printItems = value;
                    RaisePropertyChanged();
                }
            }
        }
        List<string> _printItems;

        public int OrderCount
        {
            get { return _orderCount; }
            set
            {
                if (_orderCount != value)
                {
                    _orderCount = value;
                    RaisePropertyChanged();
                }
            }
        }
        int _orderCount;

        public TableViewModel TableInfo
        {
            get { return _tableInfo; }
            set
            {
                if (!ReferenceEquals(_tableInfo, value))
                {
                    _tableInfo = value;

                    IsBusy = true;
                    _svc.GetTableOrderCountAsync(_tableInfo.OrderNum);

                    RaisePropertyChanged();
                }
            }
        }
        TableViewModel _tableInfo;
    }
}
