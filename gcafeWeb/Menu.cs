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
    
    public partial class menu
    {
        public menu()
        {
            this.order_detail = new HashSet<order_detail>();
            this.order_detail_setmeal = new HashSet<order_detail_setmeal>();
            this.setmeal_item = new HashSet<setmeal_item>();
            this.setmeal_item_opt = new HashSet<setmeal_item_opt>();
            this.setmeal_item1 = new HashSet<setmeal_item>();
        }
    
        public int id { get; set; }
        public int branch_id { get; set; }
        public Nullable<int> menu_catalog_id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string unit { get; set; }
        public decimal price { get; set; }
        public decimal fprice { get; set; }
        public int printer_group_id { get; set; }
        public bool is_setmeal { get; set; }
        public bool is_locked { get; set; }
        public bool sold_out { get; set; }
    
        public virtual branch branch { get; set; }
        public virtual menu_catalog menu_catalog { get; set; }
        public virtual printer_group printer_group { get; set; }
        public virtual ICollection<order_detail> order_detail { get; set; }
        public virtual ICollection<order_detail_setmeal> order_detail_setmeal { get; set; }
        public virtual ICollection<setmeal_item> setmeal_item { get; set; }
        public virtual ICollection<setmeal_item_opt> setmeal_item_opt { get; set; }
        public virtual ICollection<setmeal_item> setmeal_item1 { get; set; }
    }
}
