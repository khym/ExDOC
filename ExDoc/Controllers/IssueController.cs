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

        public void Save2DB(string id, string path, string uploadby)
        {
            using (var db = new EX_DOCEntities())
            {
                DocFile file = new DocFile();
                file.file_name = path;
                file.issue_id = id;
                db.DocFile.Add(file);
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult SaveIssue(string[] customer, string doc_name, string doc_no, string doc_rev, string date_rec, string ch_point, string tnc_product, HttpPostedFileBase[] doc_file, string customer_type)
        {
  
            Issue save_issue = new Issue();
            Relation_Issue_Cust save_cust = new Relation_Issue_Cust();
            DocFile file2db = new DocFile();

            DateTime d = Convert.ToDateTime(date_rec);
            string issue_no = "EXDOC-21312-" + DateTime.Now.Millisecond + "";
            save_issue.issue_no = issue_no;
            save_issue.doc_name = doc_name;
            save_issue.doc_no = doc_no;
            save_issue.doc_rev = doc_rev;
            save_issue.rec_date = d;
            save_issue.tnc_product = tnc_product;
            save_issue.doc_type_id = 1;
            ex_doc.Issue.Add(save_issue);
            //ex_doc.SaveChanges();

            
   //Contains

            //for (int i = 0; i < customer.Length; i++)
            //{
            //    string cust_id = customer[i];
            //    //if (customer_type == "Customer Number")
            //    //{
            //       //var sql =  ex_doc.Customer.Where(w => w.cust_name.Contains(item)).Select(a=>a.cust_no);
            //    //    cust_id = customer[i];
            //    //}
            //    //else
            //        if (customer_type == "Customer Name")
            //    {
            //        //var sql = ex_doc.Customer.Where(w => w.cust_name.Contains(customer[i])).Select(a => a.cust_no).ToArray();
            //        var sql = (from a in ex_doc.Customer where a.cust_name.Contains(customer[i]) select a.cust_no);

            //        //cust_id = sql[i];


                                            
            //    }

            //    save_cust.cust_no = cust_id;
            //    save_cust.issue_id = issue_no;
            //    ex_doc.Relation_Issue_Cust.Add(save_cust);
            //    ex_doc.SaveChanges();

            //    foreach (var doc in doc_file)
            //    {
            //        string subPath = "~/UploadFiles/" + cust_id + "/";
            //        if (!Directory.Exists(Server.MapPath(subPath)))
            //        {
            //            Directory.CreateDirectory(Server.MapPath(subPath));
            //        }

            //        var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(doc.FileName.Replace('#', ' ').Replace('%', ' '));
            //        var path = Path.Combine(Server.MapPath(subPath), fileName);
            //        doc.SaveAs(path);
            //        file2db.file_name = subPath + fileName;
            //        file2db.issue_id = issue_no;
            //        ex_doc.DocFile.Add(file2db);
            //        ex_doc.SaveChanges();
            //    }
            //}

            foreach (var item in customer)
            {

                if (customer_type == "Customer Number")
                {
                    //var sql =  ex_doc.Customer.Where(w => w.cust_name.Contains(item)).Select(a=>a.cust_no);

                    //save_cust.cust_no = item;
                    //save_cust.issue_id = issue_no;
                    //ex_doc.Relation_Issue_Cust.Add(save_cust);

                   var indy_cust = new Relation_Issue_Cust();
                   indy_cust.cust_no = item;
                   indy_cust.issue_id = issue_no;
                   ex_doc.Relation_Issue_Cust.Add(indy_cust);

                    //ex_doc.SaveChanges();

                    foreach (var doc in doc_file)
                    {
                        string subPath = "~/UploadFiles/" + item + "/";
                        if (!Directory.Exists(Server.MapPath(subPath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(subPath));
                        }

                        var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(doc.FileName.Replace('#', ' ').Replace('%', ' '));
                        var path = Path.Combine(Server.MapPath(subPath), fileName);
                        doc.SaveAs(path);

                        
                        //file2db.file_name = subPath + fileName;
                        //file2db.issue_id = issue_no;
                        //ex_doc.DocFile.Add(file2db);
                        //ex_doc.SaveChanges();

                        var filedb = new DocFile();
                        filedb.issue_id = issue_no;
                        filedb.file_name = subPath + fileName;
                        ex_doc.DocFile.Add(filedb);
                    }


                }
                else if (customer_type == "Customer Name")
                {
                    var sql = ex_doc.Customer.Where(w => w.cust_name.Contains(item)).Select(a => a.cust_no);

                    foreach (var item2 in sql)
                    {


                        //save_cust.cust_no = item2;
                        //save_cust.issue_id = issue_no;
                        //ex_doc.Relation_Issue_Cust.Add(save_cust);

                        var indy_cust = new Relation_Issue_Cust();
                        indy_cust.cust_no = item2;
                        indy_cust.issue_id = issue_no;
                        ex_doc.Relation_Issue_Cust.Add(indy_cust);
                        

                        foreach (var doc in doc_file)
                        {
                            string subPath = "~/UploadFiles/" + item2 + "/";
                            if (!Directory.Exists(Server.MapPath(subPath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(subPath));
                            }

                            var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(doc.FileName.Replace('#', ' ').Replace('%', ' '));
                            var path = Path.Combine(Server.MapPath(subPath), fileName);
                            doc.SaveAs(path);
                            //file2db.file_name = subPath + fileName;
                            //file2db.issue_id = issue_no;
                            //ex_doc.DocFile.Add(file2db);

                            var filedb = new DocFile();
                            filedb.issue_id = issue_no;
                            filedb.file_name = subPath + fileName;
                            ex_doc.DocFile.Add(filedb);
                            
                        }
                    }
                    //ex_doc.SaveChanges();
                }
                
            }

            ex_doc.SaveChanges();


            



            // Add data to DB TD_File //
            //string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + main_id + "/";
            //if (!Directory.Exists(Server.MapPath(subPath)))
            //{
            //    Directory.CreateDirectory(Server.MapPath(subPath));
            //}
            //foreach (var file in files)
            //{
            //    if (file != null && file.ContentLength > 0)
            //    {
            //        if (file.ContentType == "application/pdf")//Check accept file type.
            //        {
            //            var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName.Replace('#', ' ').Replace('%', ' '));
            //            var path = Path.Combine(Server.MapPath(subPath), fileName);
            //            file.SaveAs(path);
            //            SaveFiletoDB(main_id, subPath + fileName, Session["SA_Auth"].ToString());
            //        }
            //    }
            //}


            return RedirectToAction("Index", "Issue");
        }

    }
}
