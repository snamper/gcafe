//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gcafePrnConsole
{
    using System;
    using System.Collections.Generic;
    
    public partial class staff
    {
        public staff()
        {
            this.member_refill = new HashSet<member_refill>();
            this.order = new HashSet<order>();
            this.order1 = new HashSet<order>();
            this.order2 = new HashSet<order>();
            this.order_detail = new HashSet<order_detail>();
        }
    
        public int id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int branch_id { get; set; }
        public int role_id { get; set; }
        public System.DateTime join_date { get; set; }
        public bool is_deleted { get; set; }
    
        public virtual branch branch { get; set; }
        public virtual ICollection<member_refill> member_refill { get; set; }
        public virtual ICollection<order> order { get; set; }
        public virtual ICollection<order> order1 { get; set; }
        public virtual ICollection<order> order2 { get; set; }
        public virtual ICollection<order_detail> order_detail { get; set; }
        public virtual role role { get; set; }
    }
}