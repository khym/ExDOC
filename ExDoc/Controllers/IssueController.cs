using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCommonFunction;
using ExDoc.Models;
using System.IO;
namespace ExDoc.Controllers
{
    public class IssueController : Controller
    {
        //
        // GET: /Issue/
        TNCRunNumber run = new TNCRunNumber();
        EX_DOCEntities ex_doc = new EX_DOCEntities();
        //private static int programId = 44;
        [Tnc_Auth]
        public ActionResult Index()
        {
            //string issue = run.GetRunNumber(programId);
            //run.SetRunNumber(programId);
            return View();
        }
        [Tnc_Auth]
        public ActionResult NewIssue()
        {
            return View();
        }

        public ActionResult _CreateIssue()
        {
            return PartialView();
        }

        public void File2DB(string issue_no, HttpPostedFileBase[] doc_file, string cust_no)
        {

            var indy_cust = new Relation_Issue_Cust();

            indy_cust.cust_no = cust_no;
            indy_cust.issue_id = issue_no;
            ex_doc.Relation_Issue_Cust.Add(indy_cust);

            foreach (var doc in doc_file)
            {
                string subPath = "~/UploadFiles/" + cust_no + "/";
                if (!Directory.Exists(Server.MapPath(subPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(subPath));
                }

                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(doc.FileName.Replace('#', ' ').Replace('%', ' '));
                var path = Path.Combine(Server.MapPath(subPath), fileName);
                doc.SaveAs(path);

                var filedb = new DocFile();
                filedb.issue_id = issue_no;
                filedb.file_name = subPath + fileName;
                ex_doc.DocFile.Add(filedb);
            }
        }

        [HttpPost]
        public ActionResult SaveIssue(string[] customer, string doc_name, string doc_no, string doc_rev, string date_rec, string ch_point, string tnc_product, HttpPostedFileBase[] doc_file, string customer_type)
        {
  
            Issue save_issue = new Issue();
            Relation_Issue_Cust save_cust = new Relation_Issue_Cust();
            DocFile file2db = new DocFile();

            DateTime d = Convert.ToDateTime(date_rec);
            string issue_no = "EXDOC-" + DateTime.Now.Date + DateTime.Now.Month + DateTime.Now.Year + "-"+ DateTime.Now.Minute + DateTime.Now.Millisecond + "";
            save_issue.issue_no = issue_no;
            save_issue.doc_name = doc_name;
            save_issue.doc_no = doc_no;
            save_issue.doc_rev = doc_rev;
            save_issue.rec_date = d;
            save_issue.tnc_product = tnc_product;
            save_issue.doc_type_id = 1;
            save_issue.issue_date = DateTime.Now;
            ex_doc.Issue.Add(save_issue);

            foreach (var item in customer)
            {

                if (customer_type == "Customer Number")
                {
                   File2DB(issue_no,doc_file,item);
                }
                else if (customer_type == "Customer Name")
                {
                    var sql = ex_doc.Customer.Where(w => w.cust_name.Contains(item)).Select(a => a.cust_no);

                    foreach (var item2 in sql)
                    {
                        File2DB(issue_no, doc_file, item2);
                    }
                }
                
            }

            ex_doc.SaveChanges();
            return RedirectToAction("Index", "Issue");
        }

    }
}
