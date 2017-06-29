using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ExDoc.Models;
using WebCommonFunction;
using System.Diagnostics;

namespace ExDoc.Controllers
{
    public class HomeController : Controller
    {
        EX_DOCEntities tnc_costomer = new EX_DOCEntities();
        TNCSecurity tnc_se = new TNCSecurity();
        EX_DOCEntities ex_doc = new EX_DOCEntities();
        TNC_ADMINEntities tnc_admin = new TNC_ADMINEntities();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult login(string id, string pwd, string tmpurl)
        {
            var chklogin = tnc_se.Login(id, pwd,false);

            if (chklogin != null)
            {
                Session["emp_code"] = chklogin.emp_code;
                Session["emp_name"] = chklogin.emp_fname + " " + chklogin.emp_lname.Substring(0,2)+".";
                Session["emp_lvl"] = chklogin.position_level;
                Session["emp_org_lvl"] = chklogin.LeafOrgGroup;


                //test show data
                Session["d_name"] = chklogin.dept_name;
                Session["d_id"] = chklogin.dept_id;
                Session["email"] = chklogin.email;

                if (String.IsNullOrEmpty(chklogin.group_id.ToString()) || String.IsNullOrWhiteSpace(chklogin.group_id.ToString()))
                {
                    Session["g_id"] = chklogin.dept_id;
                    int? sds = chklogin.dept_id;
                }
                else
                {
                    Session["g_id"] = chklogin.group_id;
                    int? ss = chklogin.group_id;  
                }

                //Session["g_id"] = chklogin.group_id;
                Session["g_name"] = chklogin.group_name;
                Session["po_lvl"] = chklogin.position_level;
                Session["po_name"] = chklogin.position_name;
                Session["p_id"] = chklogin.plant_id;
                Session["p_name"] = chklogin.plant_name;

            }

            if (Session["Redirect"] != null)
            {
                string url = Session["Redirect"].ToString();
                Session.Remove("Redirect");
                if (url != null)
                {
                    return Redirect(url);
                }
                else
                {
                    return RedirectToAction("Index", "Issue");
                }
            }
            else
            {

                return RedirectToAction("Index", "Home");
            }
        }



        public ActionResult logout()
        {
            Session.Remove("emp_code");
            Session.Remove("emp_name");
            Session.Remove("Redirect");
            Session.Remove("emp_admin");
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public ActionResult get_doc_file(string issue_no)
        {
            
            return Json(new{ data = "asdas" });
        }

        [HttpPost]
        public ActionResult get_all_customer(string term)
        {

            var sql = tnc_costomer.Customer.Select(a => new { id = a.cust_no, text = a.cust_name + "(" + a.cust_no + ")" }).Take(16).ToList();
            if (term != null)
            {
                sql = tnc_costomer.Customer.Where(a => a.cust_no.Contains(term) || a.cust_name.Contains(term)).Select(a => new { id = a.cust_no, text = a.cust_name + "(" + a.cust_no + ")" }).Take(16).ToList();
            }

            return Json(sql, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult get_distinct_customer(string term)
        {
            var sql = tnc_costomer.Customer.Select(a => new { id = a.cust_name, text = a.cust_name }).Distinct().Take(16);

            if (term != null)
            {
                sql = tnc_costomer.Customer.Where(a => a.cust_name.Contains(term)).Select(a => new { id = a.cust_name, text = a.cust_name }).Distinct().Take(16);
            }
            return Json(sql, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult get_tnc_group(string term)
        {
            var sql = tnc_admin.tnc_group_master.Select(a => new { id = a.id, text = a.group_name }).OrderBy(a=>a.text).Take(16);

            if (term != null)
            {
                sql = tnc_admin.tnc_group_master.Where(a => a.group_name.Contains(term)).Select(a => new { id = a.id, text = a.group_name }).OrderBy(a => a.text).Take(16);
            }
            return Json(sql, JsonRequestBehavior.AllowGet);
        }


        public JsonResult get_issue(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var sql = from a in ex_doc.Issue select a;
                string emp_code = Session["emp_code"].ToString();
                //int g_id = 0;
                var check1 = String.IsNullOrEmpty(Session["emp_code"].ToString());
                var check2 = String.IsNullOrWhiteSpace(Session["emp_code"].ToString());
                //if (String.IsNullOrEmpty(Session["emp_code"].ToString()) || String.IsNullOrWhiteSpace(Session["emp_code"].ToString()))
                //{
                //    g_id = int.Parse(Session["g_id"].ToString());
                //}
                int g_id = int.Parse(Session["g_id"].ToString());
                int po_lvl = int.Parse(Session["po_lvl"].ToString());
                int d_id = int.Parse(Session["d_id"].ToString());
                

            //    var test_sql = ex_doc.Issue.Where(a => a.Transaction.OrderBy(b => b.seq).FirstOrDefault().actor == emp_code
            //|| a.Transaction.Any(
            //                    b => b.org_id == g_id &&
            //                    ( po_lvl >= b.User_level.position_min &&  po_lvl <= b.User_level.position_max) &&
            //                     b.action_id == 5
            //                     )).Select(a => a);
                //Stopwatch stopwatch = Stopwatch.StartNew();


                //old
                //var test_sql2 = ex_doc.Issue.Where(a =>
                //(
                //((a.Transaction.OrderBy(b => b.seq).FirstOrDefault().actor == emp_code &&
                //a.Transaction.OrderBy(b => b.seq).FirstOrDefault().action_id == 1) ||
                //(po_lvl >= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_min &&
                //po_lvl <= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_max &&
                //a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().org_id == g_id &&
                //a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().action_id == 0)) &&
                //a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().status_id < 100) ||
                //a.Transaction.Any(b => b.status_id == 5 && b.org_id == g_id && po_lvl >= b.User_level.position_min &&
                //  po_lvl <= b.User_level.position_max && b.action_id == 0  && b.action_id != 5 ) 
                //).Select(a => a);


var test_sql2 = ex_doc.Issue.Where(a =>
                                (
                                    (
                                            (
                                                a.Transaction.OrderBy(b => b.seq).FirstOrDefault().actor == emp_code
                                                && a.Transaction.OrderBy(b => b.seq).FirstOrDefault().action_id == 1
                                            )
                                            ||
                                            (
                                                a.Transaction.Any(b => (b.status_id == 2
                                                                        || b.status_id == 3
                                                                        || b.status_id == 4
                                                                        || b.status_id == 5
                                                                        || b.status_id == 6
                                                                        || b.status_id == 7)
                                                && b.org_id == g_id
                                                && po_lvl >= b.User_level.position_min
                                                && po_lvl <= b.User_level.position_max
                                                && b.action_id == 0
                                                && b.action_id != 5)
                                            )
                                    )
                                    && !(
                                        a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().status_id == 100 
                                        || a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().action_id == 5
                                       )
                                )
                            ).Select(a => a);


                //var test00 = ex_doc.Transaction.Where(a=>
                //                                      (a.actor == emp_code && a.action_id == 1) ||
                //                                      (a.action_id == 0 && 
                //                                      a.org_id == g_id &&
                //                                      po_lvl >= a.User_level.position_min &&
                //                                      po_lvl <= a.User_level.position_max &&
                //                                      a.action_id < 99
                //                                      )
                //                                      ).Select(a=>a.Issue);

                //var inprogress = ex_doc.Issue.Where(a=>a.Transaction.All(b=>b.status_id<99)).Select(a=>a);

                //var not_issuer = inprogress.Where(a => a.Transaction.Any(b => b.action_id != 1)).Select(a => a);


                

                // check group_id is null
                //var test_sql2 = ex_doc.Issue.Where(a => a.doc_type_id == 0).Select(a => a);
                //if (g_id != 0) 
                //{
                //    test_sql2 = ex_doc.Issue.Where(a =>
                //                    ((a.Transaction.OrderBy(b => b.seq).FirstOrDefault().actor == emp_code &&
                //                    a.Transaction.OrderBy(b => b.seq).FirstOrDefault().action_id == 1) ||
                //                    (po_lvl >= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_min &&
                //                    po_lvl <= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_max &&
                //                    a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().org_id == g_id &&
                //                    a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().action_id == 0)) &&
                //                    a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().status_id < 100
                //                    ).Select(a => a);

                //}else {
                // test_sql2 = ex_doc.Issue.Where(a =>
                //                ((a.Transaction.OrderBy(b => b.seq).FirstOrDefault().actor == emp_code &&
                //                a.Transaction.OrderBy(b => b.seq).FirstOrDefault().action_id == 1) ||
                //                (po_lvl >= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_min &&
                //                po_lvl <= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_max &&
                //                a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().org_id == d_id &&
                //                a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().action_id == 0)) &&
                //                a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().status_id < 100
                //                ).Select(a => a);
                //}
                


                //stopwatch.Stop();
                //a.Transaction.All(c=>c.status_id < 100 )
                //var test_sql3 = ex_doc.Issue.Where(a => 
                //    po_lvl <= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_min &&
                //    po_lvl <= a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().User_level.position_max &&
                //    a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().org_id == g_id &&
                //    a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().action_id == 5
                //    ).Select(a => a);

                //var all_issue = test_sql2.Union(test_sql3);

                int TotalRecord = test_sql2.ToList().Count();
                var data_out = (from a in test_sql2
                                select new
                                {
                                    a.issue_no,
                                    doc_type_name = a.DocType.doc_type_name,
                                    a.doc_name,
                                    a.doc_no,
                                    a.doc_rev,
                                    a.rec_date,
                                    a.tnc_product,
                                    status_name = a.Transaction.OrderByDescending(v=>v.seq).FirstOrDefault().Status.status_name
                                }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                return Json(new { Result = "OK", Records = data_out, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult canceled_issue(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var test_sql = ex_doc.Issue.Where(a => a.Transaction.Any(b => b.action_id == 5)).Select(a=>a);

                //var test_sql2 = ex_doc.Issue.Where(a => a.Transaction.OrderByDescending(b => b.seq).FirstOrDefault().status_id == 100).Select(a => a);

                int TotalRecord = test_sql.ToList().Count();
                var data_out = (from a in test_sql
                                select new
                                {
                                    a.issue_no,
                                    doc_type_name = a.DocType.doc_type_name,
                                    a.doc_name,
                                    a.doc_no,
                                    a.doc_rev,
                                    a.rec_date,
                                    a.tnc_product,
                                    status_name = a.Transaction.OrderByDescending(v => v.seq).FirstOrDefault().Status.status_name
                                }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                return Json(new { Result = "OK", Records = data_out, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult controlled_issue(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {

                var test_sql2 = ex_doc.Issue.Where(a=>a.Transaction.OrderByDescending(b=>b.seq).FirstOrDefault().status_id == 100).Select(a => a);

                int TotalRecord = test_sql2.ToList().Count();
                var data_out = (from a in test_sql2
                                select new
                                {
                                    a.issue_no,
                                    doc_type_name = a.DocType.doc_type_name,
                                    a.doc_name,
                                    a.doc_no,
                                    a.doc_rev,
                                    a.rec_date,
                                    a.tnc_product,
                                    status_name = a.Transaction.OrderByDescending(v => v.seq).FirstOrDefault().Status.status_name
                                }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                return Json(new { Result = "OK", Records = data_out, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}
