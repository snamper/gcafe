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
    
    public partial class setmeal_item_opt
    {
        public int id { get; set; }
        public int setmeal_item_id { get; set; }
        public int menu_id { get; set; }
    
        public virtual menu menu { get; set; }
        public virtual setmeal_item setmeal_item { get; set; }
    }
}
