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
    
    public partial class branch
    {
        public branch()
        {
            this.member_refill = new HashSet<member_refill>();
            this.menu = new HashSet<menu>();
            this.order = new HashSet<order>();
            this.printer_group = new HashSet<printer_group>();
            this.staff = new HashSet<staff>();
            this.sys_info = new HashSet<sys_info>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<member_refill> member_refill { get; set; }
        public virtual ICollection<menu> menu { get; set; }
        public virtual ICollection<order> order { get; set; }
        public virtual ICollection<printer_group> printer_group { get; set; }
        public virtual ICollection<staff> staff { get; set; }
        public virtual ICollection<sys_info> sys_info { get; set; }
    }
}
