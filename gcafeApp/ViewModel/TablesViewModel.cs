using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace gcafeApp.ViewModel
{
    public class TablesViewModel : VMBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public TablesViewModel()
        {
            this.Items = new ObservableCollection<TableViewModel>();
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

        public void OpenTable(string tableNum, int customerNum)
        {

        }
    }
}
