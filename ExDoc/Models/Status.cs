//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExDoc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Status
    {
        public Status()
        {
            this.Transaction = new HashSet<Transaction>();
        }
    
        public int status_id { get; set; }
        public string status_name { get; set; }
    
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
