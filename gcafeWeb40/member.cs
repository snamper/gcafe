//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gcafeWeb40
{
    using System;
    using System.Collections.Generic;
    
    public partial class member
    {
        public member()
        {
            this.credit1 = new HashSet<credit>();
            this.member_refill = new HashSet<member_refill>();
            this.order = new HashSet<order>();
        }
    
        public int id { get; set; }
        public string idcard_number { get; set; }
        public string name { get; set; }
        public string phone_num { get; set; }
        public string email { get; set; }
        public int point { get; set; }
        public decimal credit { get; set; }
        public System.DateTime join_date { get; set; }
    
        public virtual ICollection<credit> credit1 { get; set; }
        public virtual ICollection<member_refill> member_refill { get; set; }
        public virtual ICollection<order> order { get; set; }
    }
}
