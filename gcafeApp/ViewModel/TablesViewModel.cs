using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class TablesViewModel : VMBase
    {
        private readonly IgcafeSvcClient _svc;

        /// <summary>
        /// 初始化
        /// </summary>
        public TablesViewModel(IgcafeSvcClient svc)
        {
            this.Items = new ObservableCollection<TableViewModel>();

            if (!DesignerProperties.IsInDesignTool)
            {
                _svc = svc;
                _svc.TableOprCompleted += _svc_TableOprCompleted;
                _svc.GetTablesInfoCompleted += _svc_GetTablesInfoCompleted;
            }
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            _svc.GetTablesInfoCompleted -= _svc_GetTablesInfoCompleted;
            _svc.TableOprCompleted -= _svc_TableOprCompleted;
        }

        void _svc_GetTablesInfoCompleted(object sender, GetTablesInfoCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _svc_TableOprCompleted(object sender, TableOprCompletedEventArgs e)
        {
            throw new NotImplementedException();
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

        public void OpenTable()
        {
            TableOprRequest req = new TableOprRequest() { DeviceId = Settings.AppSettings.DeviceID, tableNum = TableNum, customerNum = CustomerNum, oprType = TableOprType.OpenTable };

            _svc.TableOprAsync(req);
        }
    }
}
