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
    
    public partial class sys_info
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public int order_cnt { get; set; }
        public bool is_festival { get; set; }
        public int order_detail_cnt { get; set; }
        public int print_cnt { get; set; }
    
        public virtual branch branch { get; set; }
    }
}
