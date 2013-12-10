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
    public class MenuItem
    {
    }

}