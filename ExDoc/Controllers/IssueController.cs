using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCommonFunction;
using ExDoc.Models;
using System.IO;
using ExDoc.ViewModel;
namespace ExDoc.Controllers
{
    public class IssueController : Controller
    {
        //
        // GET: /Issue/
        TNCRunNumber run = new TNCRunNumber();
        EX_DOCEntities ex_doc = new EX_DOCEntities();
        TNC_ADMINEntities tnc_admin = new TNC_ADMINEntities();
        private static int programId = 45;
        [Tnc_Auth]
        public ActionResult Index()
        {
            
            

            return View();
        }
        [Tnc_Auth]
        public ActionResult NewIssue()
        {
            var sql = ex_doc.DocType.Select(a => a).ToList();
            ViewBag.doc_type = sql;

            return View();
        }

        public ActionResult NewDoc()
        {
            return View();
        }

        [Tnc_Auth]
        public ActionResult NewRevise()
        {
            return View();
        }

        public  ActionResult _CreateIssue()
        {
            return PartialView();
        }

        public ActionResult _CustomerAndFile(string issue_no)
        {
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => new
            {
                Relation_IC = a.Relation_Issue_Cust
            }).FirstOrDefault();

            var sql_lvl2 = sql.Relation_IC.Select(a => new { a.Customer.cust_name, a.cust_no , a.DocFile }).ToList();

            List<CustomerFile> cf = new List<CustomerFile>();

            for (int i = 0; i < sql_lvl2.Count; i++)
            {
                cf.Add(new CustomerFile { cust_name = sql_lvl2[i].cust_name, cust_no = sql_lvl2[i].cust_no,DocFile = sql_lvl2[i].DocFile.ToList() });
            }


            return PartialView(cf);

        }

        public ActionResult _TransactionDetial(string issue_no)
        {
            ViewBag.issue_no = issue_no;

            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => new
            {
                a.Transaction
            }).FirstOrDefault();

            //var ss = sql.Transaction.Join(tnc_admin.V_Employee_Info, a => a.actor, b => b.emp_code, (a, b) => new { a.Action.action_name, name = b.emp_fname + " " + b.emp_lname, a.actor_date, a.Status.status_name }).ToList();

            //var leftOuterJoinFluentSyntax = first.GroupJoin(second,
            //                          f => f,
            //                          s => s,
            //                          (f, s) => new { First = f, Second = s })
            //                       .SelectMany(temp => temp.Second.DefaultIfEmpty(),
            //                          (f, s) => new { First = f.First, Second = s });

            var ss_groupjoin = sql.Transaction.GroupJoin(tnc_admin.V_Employee_Info,
                                            f => f.actor,
                                            s => s.emp_code,
                                            (f, s) => new { f.Action.action_name,f.actor_date,f.Status.status_name, s })
                                            .SelectMany(temp => temp.s.DefaultIfEmpty(),
                                            (f, s) => new
                                            {
                                                f.action_name,
                                                f.actor_date,
                                                f.status_name,
                                                Second = s
                                            }).ToList();

            List<TranVSEmpInfo> dd = new List<TranVSEmpInfo>();

            for (int i = 0; i < ss_groupjoin.Count; i++)
            {
                if (ss_groupjoin[i].Second != null)
                {
                    dd.Add(new TranVSEmpInfo
                    {
                        action_name = ss_groupjoin[i].action_name,
                        actor_name = ss_groupjoin[i].Second.emp_fname + " " + ss_groupjoin[i].Second.emp_lname,
                        actor_date = ss_groupjoin[i].actor_date,
                        status_name = ss_groupjoin[i].status_name
                    });
                }
                else
                {
                    dd.Add(new TranVSEmpInfo
                    {
                        action_name = ss_groupjoin[i].action_name,
                        actor_name = "-",
                        actor_date = ss_groupjoin[i].actor_date,
                        status_name = ss_groupjoin[i].status_name
                    });
                }
            }

            //for (int i = 0; i < ss.Count; i++)
            //{

            //    dd.Add(new TranVSEmpInfo { action_name = ss[i].action_name, actor_name = ss[i].name, actor_date = ss[i].actor_date, status_name = ss[i].status_name });
            //}



            return PartialView(dd);
        }

        public ActionResult _PriviewDoc(string issue_no)
        {
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => new {
                a.issue_no,
                doc_type_name = a.DocType.doc_type_name,
                a.doc_name,
                a.doc_no,
                a.doc_rev,
                a.rec_date,
                a.change_point,
                a.tnc_product,
                a.issue_date
            }).FirstOrDefault();

            DocDetail doc_detial = new DocDetail()
            {
                issue_no = sql.issue_no,
                doc_type = sql.doc_type_name,
                doc_name = sql.doc_name,
                doc_no = sql.doc_no,
                doc_rev = sql.doc_rev,
                rec_date = sql.rec_date,
                change_point = sql.change_point,
                tnc_product = sql.tnc_product,
                issue_date = sql.issue_date
            };

            ViewBag.emp_code = Session["emp_code"].ToString();

            //var ss = sql.Transaction.Join(tnc_admin.V_Employee_Info, a => a.actor, b => b.emp_code, (a, b) => new { a.Action.action_name, name = b.emp_fname + " " + b.emp_lname, a.actor_date, a.Status.status_name }).ToList();

            //List<TranVSEmpInfo> dd = new List<TranVSEmpInfo>();


            //for (int i = 0; i < ss.Count; i++)
            //{

            //    dd.Add(new TranVSEmpInfo { action_name = ss[i].action_name, actor_name = ss[i].name, actor_date = ss[i].actor_date, status_name = ss[i].status_name });
            //}

            //var doc_file = sql.Relation_Issue_Cust.Select(a => a.DocFile.Any(b => b.relation_id == a.id)).Select(a => a);

            

            //IssueAndEmp xx = new IssueAndEmp()
            //{
            //    issue_no = sql.issue_no,
            //    doc_type = sql.DocType.doc_type_name,
            //    doc_name = sql.doc_name,
            //    doc_no = sql.doc_no,
            //    doc_rev = sql.doc_rev,
            //    rec_date = sql.rec_date,
            //    change_point = sql.change_point,
            //    tnc_product = sql.tnc_product,
            //    issue_date = sql.issue_date,
            //    Relation_IC = sql.Relation_Issue_Cust.Select(a=>a).ToList(),
            //    tranVSemp = dd
                               
            //};

            return PartialView(doc_detial);
        }

        public ActionResult _PreviewIssue(string issue_no)
        {
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => a).FirstOrDefault();

            //var tnc_data = (from a in sql.Transaction
            //               join b in tnc_admin.V_Employee_Info on a.actor equals b.emp_code
            //               select b);
            var tnc_data = sql.Transaction;

            ViewBag.emp_code = Session["emp_code"].ToString();

            return PartialView(sql);
        }

        public ActionResult ApproveIssue(string code)
        {
            var ss = code;
            return RedirectToAction("Index", "Issue");
        }

        [Tnc_Auth]
        public ActionResult MgrApprove(string code)
        {
            var ss = code;
            return RedirectToAction("Index", "Issue");
        }

        public void File2DB(string issue_no, HttpPostedFileBase[] doc_file, string cust_no)
        {

            var indy_cust = new Relation_Issue_Cust();

            indy_cust.cust_no = cust_no;
            indy_cust.issue_id = issue_no;
            ex_doc.Relation_Issue_Cust.Add(indy_cust);
            ex_doc.SaveChanges();

            var id = indy_cust.id;

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
                filedb.relation_id = id;
                filedb.file_name = subPath + fileName;
                ex_doc.DocFile.Add(filedb);
            }
        }

        [HttpPost]
        public ActionResult SaveIssue(string[] customer, string doc_name, string doc_no, string doc_rev, string date_rec, string ch_point, string tnc_product, HttpPostedFileBase[] doc_file, string customer_type, int doc_type)
        {

            Issue save_issue = new Issue();
            DateTime d = Convert.ToDateTime(date_rec);

            string issue_no = run.GetRunNumber(programId);
            run.SetRunNumber(programId);
            
            save_issue.issue_no = issue_no;
            save_issue.doc_name = doc_name;
            save_issue.doc_no = doc_no;
            save_issue.doc_rev = doc_rev;
            save_issue.rec_date = d;
            save_issue.tnc_product = tnc_product;
            save_issue.doc_type_id = doc_type;
            save_issue.issue_date = DateTime.Now;

            ex_doc.Issue.Add(save_issue);

            foreach (var item in customer)
            {
                    File2DB(issue_no, doc_file, item);
            }

            var create_tran = new Transaction();
            create_tran.issue_no = issue_no;
            create_tran.status_id = 0;
            create_tran.action_id = 4;
            create_tran.actor = Session["emp_code"].ToString();
            create_tran.actor_date = DateTime.Now;
            create_tran.org_id = int.Parse(Session["g_id"].ToString());
            create_tran.level_id = 1;
            create_tran.comment = null;

            ex_doc.Transaction.Add(create_tran);

            var create_tran2 = new Transaction();
            create_tran2.issue_no = issue_no;
            create_tran2.status_id = 4;
            create_tran2.action_id = 5;
            create_tran2.actor = null;
            create_tran2.actor_date = null;
            create_tran2.org_id = int.Parse(Session["g_id"].ToString());
            create_tran2.level_id = 2;
            create_tran2.comment = null;
            ex_doc.Transaction.Add(create_tran2);

            ex_doc.SaveChanges();
            return RedirectToAction("Index", "Issue");
        }

    }




}
