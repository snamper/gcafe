//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gcafeFoxproSvc
{
    using System;
    using System.Collections.Generic;
    
    public partial class order_detail
    {
        public order_detail()
        {
            this.order_detail_method = new HashSet<order_detail_method>();
            this.order_detail_setmeal = new HashSet<order_detail_setmeal>();
        }
    
        public int id { get; set; }
        public int device_id { get; set; }
        public int order_id { get; set; }
        public int menu_id { get; set; }
        public int group_cnt { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public int order_staff_id { get; set; }
        public System.DateTime order_time { get; set; }
        public Nullable<System.DateTime> produced_time { get; set; }
        public bool is_cancle { get; set; }
        public Nullable<int> cancel_staff_id { get; set; }
        public Nullable<System.DateTime> cancel_time { get; set; }
        public string memo { get; set; }
    
        public virtual device device { get; set; }
        public virtual menu menu { get; set; }
        public virtual order order { get; set; }
        public virtual ICollection<order_detail_method> order_detail_method { get; set; }
        public virtual ICollection<order_detail_setmeal> order_detail_setmeal { get; set; }
        public virtual staff staff { get; set; }
    }
}
