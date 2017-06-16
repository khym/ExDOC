using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExDoc.ViewModel;
using ExDoc.Models;

namespace ExDoc.ViewModel
{
    public class IssueAndEmp
    {
        public string issue_no { get; set; }
        public string doc_type { get; set; }
        public string doc_name { get; set; }
        public string doc_no { get; set; }
        public string doc_rev { get; set; }
        public DateTime rec_date { get; set; }
        public string change_point { get; set; }
        public string tnc_product { get; set; }
        public Nullable<DateTime> issue_date { get; set; }

        public List<TranVSEmpInfo> tranVSemp { get; set; }
        public List<Relation_Issue_Cust> Relation_IC { get; set; }
    }
}