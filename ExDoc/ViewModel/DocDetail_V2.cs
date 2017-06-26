using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExDoc.ViewModel
{
    public class DocDetail_V2
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
        public Boolean check_tran { get; set; }

    }
}