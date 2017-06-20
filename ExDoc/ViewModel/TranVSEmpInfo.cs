using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExDoc.ViewModel
{
    public class TranVSEmpInfo
    {
        public string action_name { get; set; }
        public string actor_name { get; set; }
        public Nullable<DateTime> actor_date { get; set; }
        public string status_name { get; set; }
        public string comment { get; set; }
        public string comment_pic { get; set; }
    }
}