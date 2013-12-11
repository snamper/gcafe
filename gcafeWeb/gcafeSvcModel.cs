using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace gcafeWeb
{
    [DataContract(IsReference = true)]
    public class Order
    {
        [DataMember]
        public int id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    //NotifyPropertyChanged();
                }
            }
        }
        private int _id;

        [DataMember]
        public string TableNo
        {
            get { return _tableNo; }
            set
            {
                if (!ReferenceEquals(_tableNo, value))
                {
                    _tableNo = value;
                    //NotifyPropertyChanged();
                }
            }
        }
        private string _tableNo;
    }

    [DataContract(IsReference = true)]
    public class MenuCatalog
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int ParentID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract(IsReference = true)]
    public class SetmealItem
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// 套餐项目的可选项目
        /// </summary>
        [DataMember]
        public List<SetmealItem> OptionItems { get; set; }
    }

    [DataContract(IsReference = true)]
    public class MenuItem
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Unit { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public bool IsSetmeal { get; set; }
        [DataMember]
        public List<SetmealItem> SetmealItems { get; set; }
    }
}
