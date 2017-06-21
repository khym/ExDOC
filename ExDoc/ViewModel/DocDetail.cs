using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExDoc.ViewModel
{
    public class DocDetail
    {
        public string issue_no { get; set; }
        public int doc_type_id { get; set; }
        public string doc_type { get; set; }
        public string doc_name { get; set; }
        public string doc_no { get; set; }
        public string doc_rev { get; set; }
        public DateTime rec_date { get; set; }
        public string change_point { get; set; }
        public string tnc_product { get; set; }
        public Nullable<DateTime> issue_date { get; set; }
        public int last_tran_seq { get; set; }
        public int last_tran_lvl_min { get; set; }
        public int last_tran_lvl_max { get; set; }
        public int last_tran_org_id { get; set; }
        public int last_tran_status_id { get; set; }
        public string last_tran_actor { get; set; }
    }
}