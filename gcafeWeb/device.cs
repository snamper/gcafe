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
    
    public partial class device
    {
        public device()
        {
            this.order_detail = new HashSet<order_detail>();
            this.order = new HashSet<order>();
        }
    
        public int id { get; set; }
        public string device_id { get; set; }
        public string register_ticket { get; set; }
        public System.DateTime register_time { get; set; }
        public bool is_deny { get; set; }
    
        public virtual ICollection<order_detail> order_detail { get; set; }
        public virtual ICollection<order> order { get; set; }
    }
}
