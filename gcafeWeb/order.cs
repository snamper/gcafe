//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gcafeWeb
{
    using System;
    using System.Collections.Generic;
    
    public partial class order
    {
        public order()
        {
            this.credit = new HashSet<credit>();
            this.order_detail = new HashSet<order_detail>();
            this.shift_detail = new HashSet<shift_detail>();
        }
    
        public int id { get; set; }
        public int branch_id { get; set; }
        public int device_id { get; set; }
        public string order_num { get; set; }
        public string table_no { get; set; }
        public int customer_number { get; set; }
        public int open_table_staff_id { get; set; }
        public System.DateTime table_opened_time { get; set; }
        public Nullable<decimal> receivable { get; set; }
        public Nullable<decimal> net_receipts { get; set; }
        public Nullable<decimal> discount { get; set; }
        public Nullable<int> discount_staff_id { get; set; }
        public Nullable<int> check_out_staff_id { get; set; }
        public Nullable<System.DateTime> check_out_time { get; set; }
        public Nullable<int> member_id { get; set; }
        public Nullable<int> shift_id { get; set; }
        public bool is_cancel { get; set; }
        public Nullable<int> cancel_staff_id { get; set; }
        public Nullable<System.DateTime> cancel_time { get; set; }
        public string memo { get; set; }
    
        public virtual branch branch { get; set; }
        public virtual ICollection<credit> credit { get; set; }
        public virtual device device { get; set; }
        public virtual member member { get; set; }
        public virtual staff staff { get; set; }
        public virtual staff staff1 { get; set; }
        public virtual ICollection<order_detail> order_detail { get; set; }
        public virtual shift shift { get; set; }
        public virtual staff staff2 { get; set; }
        public virtual ICollection<shift_detail> shift_detail { get; set; }
    }
}
