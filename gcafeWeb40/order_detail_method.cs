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
    
    public partial class order_detail_method
    {
        public int id { get; set; }
        public Nullable<int> order_detail_id { get; set; }
        public Nullable<int> order_detail_setmeal_id { get; set; }
        public int method_id { get; set; }
        public decimal price { get; set; }
    
        public virtual method method { get; set; }
        public virtual order_detail order_detail { get; set; }
        public virtual order_detail_setmeal order_detail_setmeal { get; set; }
    }
}
