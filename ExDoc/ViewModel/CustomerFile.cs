using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExDoc.Models;

namespace ExDoc.ViewModel
{
    public class CustomerFile
    {
        public string cust_name { get; set; }
        public string cust_no { get; set; }
        public List<DocFile> DocFile { get; set; }
    }
}