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
    
    public partial class method_catalog
    {
        public method_catalog()
        {
            this.method = new HashSet<method>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<method> method { get; set; }
    }
}
