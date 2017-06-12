using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ExDoc.Models;
using WebCommonFunction;

namespace ExDoc.Controllers
{
    public class HomeController : Controller
    {
        EX_DOCEntities tnc_costomer = new EX_DOCEntities();
        TNCSecurity tnc_se = new TNCSecurity();
        EX_DOCEntities ex_doc = new EX_DOCEntities();

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
                // Session["i_level"] = chklogin.emp_code;
                Session["emp_name"] = chklogin.emp_fname + " " + chklogin.emp_lname.Substring(0,2)+".";

                Session["emp_lvl"] = chklogin.level;
                Session["emp_org_lvl"] = chklogin.LeafOrgLevel;
                //Session["sss"] = chklogin.

                //var sql = db_card.CardAdmin.Where(a => a.EmpCode == id).FirstOrDefault();


                //  Session["i_admin"] = sql.EmpCode;


            }

            if (Session["Redirect"] != null)
            {
                string url = Session["Redirect"].ToString();
                Session.Remove("Redirect");
                return Redirect(url);
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


        public JsonResult get_issue(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {

                var sql = from a in ex_doc.Issue select a;


                int TotalRecord = sql.Count();
                var data_out = (from a in ex_doc.Issue
                                select new
                                {
                                    a.issue_no,
                                    doc_type_name = a.DocType.doc_type_name,
                                    a.doc_name,
                                    a.doc_no,
                                    a.doc_rev,
                                    a.rec_date,
                                    a.tnc_product
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
