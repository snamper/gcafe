using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;
using GalaSoft.MvvmLight.Messaging;

namespace gcafeApp.ViewModel
{
    public class VMChangeTable : VMBase
    {
        IgcafeSvcClient _svc;

        public VMChangeTable(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;
                _svc.TableOprCompleted += _svc_TableOprCompleted;
            }
        }

        void _svc_TableOprCompleted(object sender, TableOprCompletedEventArgs e)
        {
            if (ReferenceEquals(e.UserState, "VMChangeTable"))
            {
                if (e.Result != null)
                {
                    Messenger.Default.Send<string>("换台成功", "ChangeTablePage");
                }
                else
                {
                    Messenger.Default.Send<string>("换台错误", "ChangeTablePage");
                }
            }
        }

        public TableViewModel OrigTable
        {
            get { return _origTable; }
            set
            {
                if (!ReferenceEquals(_origTable, value))
                {
                    _origTable = value;
                    RaisePropertyChanged();
                }
            }
        }
        TableViewModel _origTable;

        public string DestTableNum
        {
            get { return _destTableNum; }
            set
            {
                if (_destTableNum != value)
                {
                    _destTableNum = value;

                    TableInfo ti = new TableInfo()
                    {
                        Num = value,
                        OrderNum = OrigTable.OrderNum,
                        CustomerNum = OrigTable.CustomerNum,
                    };

                    _svc.TableOprAsync("", ti, OrigTable.TableNo, TableOprType.ChangeTable, "VMChangeTable");
                }
            }
        }
        string _destTableNum;
    }
}
