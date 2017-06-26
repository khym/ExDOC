using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExDoc.ViewModel
{
    public class ManageIssue
    {
        public string issue_no { get; set; }
        public int seq { get; set; }
        public int lvl_min { get; set; }
        public int lvl_max { get; set; }
        public int org_id { get; set; }
        public int status_id { get; set; }
        public string actor { get; set; }
        public int doc_type_id { get; set; }
    }
}