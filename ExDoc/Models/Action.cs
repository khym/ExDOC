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
    
    public partial class Action
    {
        public Action()
        {
            this.Transaction = new HashSet<Transaction>();
        }
    
        public int action_id { get; set; }
        public string action_name { get; set; }
        public string update_by { get; set; }
        public Nullable<System.DateTime> up_date { get; set; }
    
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}