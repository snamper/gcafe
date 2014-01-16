using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace gcafeWeb40
{
    [DataContract(Name = "TableOprType")]
    public enum TableOprType
    {
        [EnumMember]
        OpenTable,
        [EnumMember]
        ChangeTable,
        [EnumMember]
        ChangeCustomerNum,
    }

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
        public int MenuID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// 套餐项目的可选项目
        /// </summary>
        [DataMember]
        public List<SetmealItem> OptionItems { get; set; }
        [DataMember]
        public List<Method> Methods { get; set; }
    }

    [DataContract(IsReference = true)]
    public class MenuItem
    {
        [DataMember]
        public int ID { get; set; }
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
        [DataMember]
        public List<Method> Methods { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public DateTime OrderTime { get; set; }
        [DataMember]
        public DateTime? ProduceTime { get; set; }
        [DataMember]
        public string OrderStaffName { get; set; }
        [DataMember]
        public int GroupCnt { get; set; }
    }

    [DataContract(IsReference = true)]
    public class TableInfo
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string OrderNum { get; set; }
        [DataMember]
        public string Num { get; set; }
        [DataMember]
        public int CustomerNum { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public Staff OpenTableStaff { get; set; }
        [DataMember]
        public DateTime OpenTableTime { get; set; }
    }

    [DataContract(IsReference = true)]
    public class Method
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public virtual MethodCatalog MethodCatalog { get; set; }
    }

    [DataContract(IsReference = true)]
    public class MethodCatalog
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<Method> Methods { get; set; }
    }

    [DataContract(IsReference = true)]
    public class Staff
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Number { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Branch { get; set; }
        [DataMember]
        public int BranchID { get; set; }
        [DataMember]
        public int RoleID { get; set; }
    }
}
