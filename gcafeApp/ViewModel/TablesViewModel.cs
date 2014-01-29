using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class TablesViewModel : VMBase
    {
        private readonly IgcafeSvcClient _svc;
        private bool _isInputValid = true;
        private Action<string> _openTableCallBack;

        /// <summary>
        /// 初始化
        /// </summary>
        public TablesViewModel(IgcafeSvcClient svc)
        {
            this.Items = new ObservableCollection<TableViewModel>();
            CustomerNum = 1;

            if (!IsInDesignMode)
            {
                _svc = svc;
                _svc.TableOprCompleted += _svc_TableOprCompleted;
                _svc.GetTablesInfoCompleted += _svc_GetTablesInfoCompleted;
                _svc.IsTableAvaliableCompleted += _svc_IsTableAvaliableCompleted;
            }
        }

        void _svc_IsTableAvaliableCompleted(object sender, IsTableAvaliableCompletedEventArgs e)
        {
            if (e.Result != true)
            {
                _isInputValid = false;
                ErrorMsg = "台号已开，请重新选择台号";
            }
            else
            {
                _isInputValid = true;
                ErrorMsg = string.Empty;
            }

            IsBusy = false;
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            _svc.IsTableAvaliableCompleted -= _svc_IsTableAvaliableCompleted;
            _svc.GetTablesInfoCompleted -= _svc_GetTablesInfoCompleted;
            _svc.TableOprCompleted -= _svc_TableOprCompleted;
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
                    this.Items = new ObservableCollection<TableViewModel>(vmList);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            IsBusy = false;
        }

        void _svc_TableOprCompleted(object sender, TableOprCompletedEventArgs e)
        {
            if (ReferenceEquals(e.UserState, "TablesViewModel"))
            {
                if (e.Result != null)
                    _openTableCallBack("成功");
                else
                    _openTableCallBack("错误");
            }
            //string s = e.Result;
        }

        public void Reset()
        {
            TableNum = string.Empty;
            CustomerNum = 1;
            _isInputValid = true;
            ErrorMsg = string.Empty;
        }

        /// <summary>
        /// 已开台的列表
        /// </summary>
        public ObservableCollection<TableViewModel> Items 
        { 
            get { return _items; }
            private set
            {
                if (!ReferenceEquals(_items, value))
                {
                    _items = value;
                    RaisePropertyChanged();
                }
            }
        }
        private ObservableCollection<TableViewModel> _items;

        public bool IsInputValid
        {
            get { return _isInputValid; }
        }

        public string ErrorMsg
        {
            get { return _errorMsg; }
            set
            {
                if (!ReferenceEquals(value, _errorMsg))
                {
                    _errorMsg = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _errorMsg;

        /// <summary>
        /// 台号
        /// </summary>
        public string TableNum
        {
            get { return _tableNum; }
            set
            {
                if (!ReferenceEquals(value, _tableNum))
                {
                    _tableNum = value;
                    RaisePropertyChanged();

                    IsBusy = true;
                    _svc.IsTableAvaliableAsync(_tableNum);
                }
            }
        }
        private string _tableNum;

        /// <summary>
        /// 客人数目
        /// </summary>
        public int CustomerNum
        {
            get { return _customerNum; }
            set
            {
                if (_customerNum != value)
                {
                    _customerNum = value;
                    RaisePropertyChanged();
                }
            }
        }
        private int _customerNum;

        public void OpenTable(Action<string> callback)
        {
            _openTableCallBack = callback;

            _svc.TableOprAsync(Settings.AppSettings.DeviceID, 
                new TableInfo() { Num = TableNum, CustomerNum = CustomerNum, OpenTableStaff = Settings.AppSettings.LoginStaff }, 
                null, 
                TableOprType.OpenTable,
                "TablesViewModel");

            IsBusy = true;
        }

        public void GetOpenedTables()
        {
            //GetTablesInfoRequest req = new GetTablesInfoRequest(Settings.AppSettings.DeviceID);
            IsBusy = true;
            this.Items = new ObservableCollection<TableViewModel>();
            _svc.GetTablesInfoAsync(Settings.AppSettings.DeviceID);
        }
    }
}
