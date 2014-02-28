using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gcafeApp.ViewModel
{
    /// <summary>
    /// 餐台号ViewModel
    /// </summary>
    public class TableViewModel : INotifyPropertyChanged
    {
        private string _tableNo;
        /// <summary>
        /// 餐台号
        /// </summary>
        public string TableNo
        {
            get { return _tableNo; }
            set
            {
                if (_tableNo != value)
                {
                    _tableNo = value;
                    NotifyPropertyChanged("TableNo");
                }
            }
        }

        private DateTime _tableOpenedTime;
        /// <summary>
        /// 开台时间
        /// </summary>
        public DateTime TableOpenedTime
        {
            get { return _tableOpenedTime; }
            set
            {
                if (!ReferenceEquals(_tableOpenedTime, value))
                {
                    _tableOpenedTime = value;
                    NotifyPropertyChanged("TableOpenedTime");
                }
            }
        }

        private int _customerNum;
        /// <summary>
        /// 客人数
        /// </summary>
        public int CustomerNum
        {
            get { return _customerNum; }
            set
            {
                if (_customerNum != value)
                {
                    _customerNum = value;
                    NotifyPropertyChanged("CustomerNum");
                }
            }
        }

        private string _openTableStaff;
        /// <summary>
        /// 开台的服务员
        /// </summary>
        public string OpenTableStaff
        {
            get { return _openTableStaff; }
            set
            {
                if (!ReferenceEquals(_openTableStaff, value))
                {
                    _openTableStaff = value;
                    NotifyPropertyChanged("OpenTableStaff");
                }
            }
        }

        private decimal _amount;
        /// <summary>
        /// 此台的当前消费金额
        /// </summary>
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }

        public string OrderNum
        {
            get { return _orderNum; }
            set
            {
                if (!ReferenceEquals(value, _orderNum))
                {
                    _orderNum = value;
                    NotifyPropertyChanged("OrderNum");
                }
            }
        }
        private string _orderNum;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
