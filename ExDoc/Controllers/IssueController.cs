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

        [Tnc_Auth]
        public ActionResult NewRevise()
        {
            return View();
        }

        public void Add_transaction(string issue_no, int sta_id, int act_id, string act, DateTime? act_date, int? org_id, int? lvl_id, string comment, HttpPostedFileBase comment_pic,string remark)
        {
            var t = new Transaction();
            t.issue_no = issue_no;
            t.status_id = sta_id;
            t.action_id = act_id;
            t.actor = act;
            t.actor_date = act_date;
            t.org_id = org_id;
            t.level_id = lvl_id;
            t.comment = comment;
            t.remark = remark;
            ex_doc.Transaction.Add(t);
        }

        public void CopyFileWhenComplete(string issue_no)
        {
            //select file form DocFileBeforeAppr table 
            var doc_file = ex_doc.DocFileBeforeAppr.Where(a => a.issue_no == issue_no).Select(a => a).ToList();

            //select cust in this issue
            var get_cust = ex_doc.Relation_Issue_Cust.Where(a => a.issue_id == issue_no).Select(a => a).ToList();

            //loop customer
            foreach (var item in get_cust)
            {
                //loop add file to new path
                foreach (var item2 in doc_file)
                {
                    //get file name ex. 412_dog.pdf
                    string fileName = Path.GetFileName(item2.path_file);

                    // MapPath ex. c/webapp/UploadFiles/TempFile/ use in Path.Combine
                    string PathFile = Server.MapPath("~/UploadFiles/TempFile/");

                    //create folder if not found
                    string targetPath1 = "~/UploadFiles/" + item.cust_no + "/";
                    if (!Directory.Exists(Server.MapPath(targetPath1)))
                    {
                        Directory.CreateDirectory(Server.MapPath(targetPath1));
                    }

                    // use in Path.Combine
                    string targetPath = Server.MapPath("~/UploadFiles/" + item.cust_no + "/");

                    //create new file name
                    var new_fileName = DateTime.Now.Millisecond + "_" + fileName;

                    string sourceFile = Path.Combine(PathFile, fileName);
                    string destFile = Path.Combine(targetPath, new_fileName);

                    //copy file form sourceFile to destFile
                    System.IO.File.Copy(sourceFile, destFile, true);

                    //add data to DocFile table
                    var copy_file = new DocFile();
                    copy_file.file_name = targetPath1 + new_fileName;
                    copy_file.relation_id = item.id;
                    ex_doc.DocFile.Add(copy_file);
                }
            }
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

        public ActionResult _TransactionDetail(string issue_no)
        {
            var v_trandetail = ex_doc.V_TransactionEmp.Where(a => a.issue_no.Contains(issue_no)).Select(a=>a).ToList();

            List<TranVSEmpInfo> dd = new List<TranVSEmpInfo>();

            for (int i = 0; i < v_trandetail.Count; i++)
            {
                if (v_trandetail[i].action_name != null)
                {
                    dd.Add(new TranVSEmpInfo
                    {
                        action_id = v_trandetail[i].action_id.Value,
                        action_name = v_trandetail[i].action_name,
                        actor_name = v_trandetail[i].actor_name,
                        position_name = v_trandetail[i].position_name,
                        org_name = v_trandetail[i].org_name,
                        actor_date = v_trandetail[i].actor_date,
                        status_name = v_trandetail[i].status_name,
                        comment = v_trandetail[i].comment,
                        comment_pic = v_trandetail[i].comment_pic,
                        status_id = v_trandetail[i].status_id,
                        remark = v_trandetail[i].remark

                    });
                }
                else
                {
                    dd.Add(new TranVSEmpInfo
                    {
                        action_id = v_trandetail[i].action_id.Value,
                        action_name = v_trandetail[i].action_name,
                        actor_name = "-",
                        actor_date = v_trandetail[i].actor_date,
                        org_name = v_trandetail[i].org_name,
                        status_name = v_trandetail[i].status_name,
                        status_id = v_trandetail[i].status_id
                    });
                }
            }
            return PartialView(dd);
        }

        [HttpGet]
        public ActionResult EditIssue(string issue_no)
        {
            var sql = ex_doc.DocType.Select(a => a).ToList();
            var issue_data = ex_doc.Issue.Where(a => a.issue_no == issue_no).FirstOrDefault();
            ViewBag.doc_type = sql;
            return View(issue_data);
        }

        [HttpPost]
        public ActionResult SaveIssueWhenEdit(string[] customer, string doc_name, string doc_no, string doc_rev, string date_rec, string ch_point, string tnc_product, HttpPostedFileBase[] doc_file, string customer_type, int doc_type, string[] old_file_id, string issue_no)
        {
            var search_issue = ex_doc.Issue.Where(a => a.issue_no == issue_no).FirstOrDefault();

            // remove Relation_Issue_Cust.issue_no == issue_no
            var remove_rc = search_issue.Relation_Issue_Cust.Select(a => a).ToList();
            if (remove_rc.Count > 0)
            {
                foreach (var item in remove_rc)
                {
                    ex_doc.Relation_Issue_Cust.Remove(item);
                }
                ex_doc.SaveChanges();
            }

            // remove DocFileBeforeAppr.issue_no == issue_no
            var remove_df = search_issue.DocFileBeforeAppr.Select(a => a).ToList();
            if (remove_df.Count > 0)
            {
                foreach (var item in remove_df)
                {
                    ex_doc.DocFileBeforeAppr.Remove(item);
                }
                ex_doc.SaveChanges();
            }

            //update issue table
            search_issue.doc_name = doc_name;
            search_issue.doc_no = doc_no;
            search_issue.doc_rev = doc_rev;
            search_issue.rec_date = Convert.ToDateTime(date_rec);
            search_issue.change_point = ch_point;
            search_issue.tnc_product = tnc_product;
            search_issue.doc_type_id = doc_type;
            search_issue.issue_date = DateTime.Now;

            //add new file to DocFileBeforeAppr table
            if (doc_file != null)
            {
                foreach (var doc in doc_file)
                {
                    string subPath = "~/UploadFiles/TempFile/";//path wait all appr
                    if (!Directory.Exists(Server.MapPath(subPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(subPath));
                    }

                    var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(doc.FileName.Replace('#', ' ').Replace('%', ' '));
                    var path = Path.Combine(Server.MapPath(subPath), fileName);
                    doc.SaveAs(path);

                    var filedb = new DocFileBeforeAppr();
                    filedb.path_file = subPath + fileName;
                    filedb.issue_no = issue_no;
                    ex_doc.DocFileBeforeAppr.Add(filedb);
                }
            }

            if (old_file_id != null)
            {
                //add old file to DocFileBeforeAppr table
                foreach (var item in old_file_id)
                {
                    var filedb = new DocFileBeforeAppr();
                    filedb.path_file = item;
                    filedb.issue_no = issue_no;
                    ex_doc.DocFileBeforeAppr.Add(filedb);
                }
            }

            if (customer != null)
            {
                //add new data to Relation_Issue_Cust table
                foreach (var item in customer)
                {
                    var indy_cust = new Relation_Issue_Cust();
                    indy_cust.cust_no = item;
                    indy_cust.issue_id = issue_no;
                    ex_doc.Relation_Issue_Cust.Add(indy_cust);
                }
            }

            //select last transaction for update
            var sql = ex_doc.Transaction.Where(a => a.issue_no == issue_no).OrderByDescending(b => b.seq).FirstOrDefault();

            string emp_code = Session["emp_code"].ToString();

            sql.action_id = 6; // 6 = edited
            sql.actor = emp_code;
            sql.actor_date = DateTime.Now;


            int check_doc_type = ex_doc.Issue.Where(a => a.issue_no == issue_no).Select(a=>a.doc_type_id).FirstOrDefault();

            if (check_doc_type == 3)
            {
                Add_transaction(issue_no, 4, 0, null, null, 49, 3, null, null, null);
            }
            else
            {


            }

            //new transaction 
            //if (sql.status_id == 8)
            //{
            //        //create wait Mgr. (Issuer) Appr after Mgr. group (not accept)
            //        var create_tran2 = new Transaction();
            //        create_tran2.issue_no = issue_no;
            //        create_tran2.status_id = 6; // 6 = Mgr. (Issuer) After Mgr. Review (Not Accept)
            //        create_tran2.action_id = 0; // 0 = idle
            //        create_tran2.actor = null;
            //        create_tran2.actor_date = null;
            //        create_tran2.org_id = int.Parse(Session["g_id"].ToString()); // group_id Mgr. (Issuer) 
            //        create_tran2.level_id = 2; // 2 = Mgr ,position_min = 5 ,	position_max = 5
            //        create_tran2.comment = null;
            //        ex_doc.Transaction.Add(create_tran2);
            //}
            //else
            //{
            //        //create wait Mgr. (Issuer) Appr
            //        var create_tran2 = new Transaction();
            //        create_tran2.issue_no = issue_no;
            //        create_tran2.status_id = 2; // 2 = Mgr. (Issuer)
            //        create_tran2.action_id = 0; // 0 = idle
            //        create_tran2.actor = null;
            //        create_tran2.actor_date = null;
            //        create_tran2.org_id = int.Parse(Session["g_id"].ToString()); // group_id Mgr. (Issuer) 
            //        create_tran2.level_id = 2; // 2 = Mgr ,position_min = 5 ,	position_max = 5
            //        create_tran2.comment = null;
            //        ex_doc.Transaction.Add(create_tran2);
            //}

            ex_doc.SaveChanges(); // save to database

            return RedirectToAction("Index", "Issue");
        }

        public ActionResult IssueGetCustNo(string issue_no)
        {

            var get_other = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => a).FirstOrDefault();

       
            var sql = get_other.Relation_Issue_Cust.Select(a => new { id = a.cust_no, text = a.Customer.cust_name + "(" + a.cust_no + ")" }).ToList();
         

            return Json(sql, JsonRequestBehavior.AllowGet);
        }
                [HttpGet]
                public ActionResult IssueGetGroup(string issue_no)
        {
            var test1 = ex_doc.GroupReview.Where(a => a.issue_no.Contains(issue_no)).ToList().Join(tnc_admin.tnc_group_master.ToList(),
                a => a.group_id, b => b.id, (a, b) => new { id = a.group_id, text = b.group_name });
            return Json(test1, JsonRequestBehavior.AllowGet);
        }

            [HttpGet]
                public ActionResult GetGroupReviwer(string issue_no)
            {
                var test1 = ex_doc.Transaction.Where(a => a.issue_no == issue_no && a.status_id == 5).Select(a => new { a.org_id , a.remark}).ToList();
                return Json(test1, JsonRequestBehavior.AllowGet);
            }

            public ActionResult _ShowGroupReview(string issue_no)
        {
            var test1 = ex_doc.GroupReview.Where(a => a.issue_no.Contains(issue_no)).ToList().Join(tnc_admin.tnc_group_master.ToList(),
    a => a.group_id, b => b.id, (a, b) => new { id = a.group_id, text = b.group_name });

            List<string> xx = new List<string>();
            foreach (var item in test1.ToList())
            {
                string s = item.text;
                xx.Add(s);
            }

            ViewBag.Cust_name = xx;

            return PartialView();
        }


        public ActionResult _FooterPreviewDoc(string issue_no)
        {
            ViewBag.test = issue_no;

            int g_id = int.Parse(Session["g_id"].ToString());
            int user_lvl = int.Parse(Session["po_lvl"].ToString());
            string emp_code = Session["emp_code"].ToString();

            ViewBag.emp_code = emp_code;
            ViewBag.user_lvl = user_lvl;
            ViewBag.g_id = g_id;

            ViewBag.Group = tnc_admin.tnc_group_master.Select(a => a).OrderBy(a=>a.group_name).ToList();

            ViewBag.Count_reviwer = ex_doc.Transaction.Count(a=>a.issue_no == issue_no && a.status_id == 5);

            ViewBag.File = ex_doc.DocFileBeforeAppr.Where(a => a.issue_no == issue_no).ToList();

            var check_tran = ex_doc.Transaction.Where(a => a.issue_no.Contains(issue_no) &&
                            a.org_id == g_id &&
                            user_lvl >= a.User_level.position_min &&
                            user_lvl <= a.User_level.position_max &&
                            a.action_id == 0
                            ).FirstOrDefault();

            ManageIssue tran_data = new ManageIssue()
            {
                actor = check_tran.actor,
                issue_no = check_tran.issue_no,
                lvl_max = check_tran.User_level.position_max,
                lvl_min = check_tran.User_level.position_min,
                org_id = check_tran.org_id.Value,
                seq = check_tran.seq,
                status_id = check_tran.status_id,
                doc_type_id = check_tran.Issue.doc_type_id,
                remark = check_tran.remark

            };

            return PartialView(tran_data);
        }

        public ActionResult _PreviewDocV2(string issue_no)
        {
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => new
            {
                a.issue_no,
                doc_type_name = a.DocType.doc_type_name,
                a.doc_name,
                a.doc_no,
                a.doc_rev,
                a.rec_date,
                a.change_point,
                a.tnc_product,
                a.doc_type_id
            }).FirstOrDefault();


            int g_id = int.Parse(Session["g_id"].ToString());
            int user_lvl = int.Parse(Session["po_lvl"].ToString());
            string emp_code = Session["emp_code"].ToString();

            var check_tran = ex_doc.Transaction.Any(a => a.issue_no.Contains(issue_no) &&
                            a.org_id == g_id &&
                            user_lvl >= a.User_level.position_min &&
                            user_lvl <= a.User_level.position_max &&
                            a.action_id == 0
                            );

            //int status_tran = ex_doc.Transaction.Where(a => a.issue_no.Contains(issue_no) &&
            //                a.org_id == g_id &&
            //                user_lvl >= a.User_level.position_min &&
            //                user_lvl <= a.User_level.position_max &&
            //                a.action_id == 0).Select(a=>a.status_id).FirstOrDefault();

            DocDetail_V2 doc_detail = new DocDetail_V2();

                doc_detail.doc_type_id = sql.doc_type_id;
                doc_detail.issue_no = sql.issue_no;
                doc_detail.doc_type = sql.doc_type_name;
                doc_detail.doc_name = sql.doc_name;
                doc_detail.doc_no = sql.doc_no;
                doc_detail.doc_rev = sql.doc_rev;
                doc_detail.rec_date = sql.rec_date;
                doc_detail.change_point = sql.change_point;
                doc_detail.tnc_product = sql.tnc_product;
                doc_detail.check_tran = check_tran;

                var test1 = ex_doc.GroupReview.Where(a => a.issue_no.Contains(issue_no)).ToList().Join(tnc_admin.tnc_group_master.ToList(),
        a => a.group_id, b => b.id, (a, b) => new { id = a.group_id, text = b.group_name });

                List<string> xx = new List<string>();
                foreach (var item in test1.ToList())
                {
                    string s = item.text;
                    xx.Add(s);
                }

                doc_detail.cust_name = xx;

                ViewBag.commar = " , ";

            return PartialView(doc_detail);
        }

        public ActionResult _PreviewDoc(string issue_no)
        {
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => new
            {
                a.issue_no,
                doc_type_name = a.DocType.doc_type_name,
                a.doc_name,
                a.doc_no,
                a.doc_rev,
                a.rec_date,
                a.change_point,
                a.tnc_product,
                a.issue_date,
                a.doc_type_id
            }).FirstOrDefault();

            var last_tran = ex_doc.Transaction.Where(a => a.issue_no.Contains(issue_no)).OrderByDescending(b => b.seq).Select(a => a).FirstOrDefault();

            int g_id  = int.Parse(Session["g_id"].ToString());
            int user_lvl = int.Parse(Session["po_lvl"].ToString());
            string emp_code = Session["emp_code"].ToString();

            var this_tran = ex_doc.Transaction.Where(a => a.issue_no.Contains(issue_no) &&
                    a.org_id == g_id &&
                    user_lvl >= a.User_level.position_min &&
                    user_lvl <= a.User_level.position_max &&
                    a.action_id == 0
                    ).FirstOrDefault();



            DocDetail doc_detail = new DocDetail()
            {
                issue_no = sql.issue_no,
                doc_type = sql.doc_type_name,
                doc_type_id = sql.doc_type_id,
                doc_name = sql.doc_name,
                doc_no = sql.doc_no,
                doc_rev = sql.doc_rev,
                rec_date = sql.rec_date,
                change_point = sql.change_point,
                tnc_product = sql.tnc_product,
                issue_date = sql.issue_date,
                last_tran_lvl_max = this_tran.User_level.position_max,
                last_tran_lvl_min = this_tran.User_level.position_min,
                last_tran_org_id = this_tran.org_id.Value,
                last_tran_seq = this_tran.seq,
                last_tran_status_id = this_tran.status_id,
                last_tran_actor = this_tran.actor
            };

            ViewBag.emp_code = emp_code;
            ViewBag.user_lvl = user_lvl;
            ViewBag.g_id = g_id;

            return PartialView(doc_detail);
        }



        public ActionResult _CustomerAndFileV2(string issue_no)
        {
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => new
            {
                a.Relation_Issue_Cust,
                a.DocFileBeforeAppr
            }).FirstOrDefault();

            ViewBag.Cust_name = sql.Relation_Issue_Cust.Select(a =>  a.Customer.cust_name+" ( "+a.cust_no+")" ).ToList();
            ViewBag.file_name = sql.DocFileBeforeAppr.Select(a => a.path_file).ToList();


            return PartialView();
        }

        public ActionResult _PreviewCompleteDoc(string issue_no)
        {
            ViewBag.emp_code = Session["emp_code"].ToString();
            ViewBag.user_lvl = int.Parse(Session["po_lvl"].ToString());
            ViewBag.g_id = int.Parse(Session["g_id"].ToString());
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => a).FirstOrDefault();
            return PartialView(sql);
        }

        public ActionResult _PreviewCanceledDoc(string issue_no)
        {
            ViewBag.emp_code = Session["emp_code"].ToString();
            ViewBag.user_lvl = int.Parse(Session["po_lvl"].ToString());
            ViewBag.g_id = int.Parse(Session["g_id"].ToString());
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => a).FirstOrDefault();
            return PartialView(sql);
        }

        public ActionResult _PreviewIssue(string issue_no)
        {
            var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => a).FirstOrDefault();
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
        [HttpPost]
        public ActionResult Approve(string emp_code, int user_lvl, int g_id, string issue_no, int seq, int doc_type_id, int?[] g_review, string[] remark)
        {
            var create_tran2 = new Transaction();

            var sql = ex_doc.Transaction.Where(a => a.seq == seq).FirstOrDefault();

            var check_status = sql.status_id;

            //check status transaction
            if (check_status == 5)// 5 = Mgr. Accept
            {
                sql.action_id = 4; // 4 = Accepted
            }
            else
            {
                sql.action_id = 2; // 2 = Approved
            }

            sql.actor = emp_code;
            sql.actor_date = DateTime.Now;

            //if (!(string.IsNullOrWhiteSpace(remark) || string.IsNullOrEmpty(remark)))
            //{
            //    sql.remark = remark;
            //}

            // check document type
            if (doc_type_id == 3) //Format
            {
                switch (check_status) //check status in last transaction for crate next transaction
                {

                    case 4: //QS Dept Appr
                        {
                            //create_tran2.status_id = 100; //complete
                            //create_tran2.action_id = 100;//completed

                            Add_transaction(issue_no, 100, 100, emp_code, DateTime.Now, 18, 1, null, null, null);

                        //select file form DocFileBeforeAppr table 
                        var doc_file = ex_doc.DocFileBeforeAppr.Where(a=>a.issue_no == issue_no).Select(a=>a).ToList();
                        
                        //select cust in this issue
                        var get_cust = ex_doc.Relation_Issue_Cust.Where(a => a.issue_id == issue_no).Select(a => a).ToList();

                        //loop customer
                        foreach (var item in get_cust)
                        {
                            //loop add file to new path
                            foreach (var item2 in doc_file)
                            {
                                //get file name ex. 412_dog.pdf
                                string fileName = Path.GetFileName(item2.path_file);

                                // MapPath ex. c/webapp/UploadFiles/TempFile/ use in Path.Combine
                                string PathFile = Server.MapPath("~/UploadFiles/TempFile/");

                                //create folder if not found
                                string targetPath1 = "~/UploadFiles/" + item.cust_no + "/";
                                if (!Directory.Exists(Server.MapPath(targetPath1)))
                                {
                                    Directory.CreateDirectory(Server.MapPath(targetPath1));
                                }

                                // use in Path.Combine
                                string targetPath = Server.MapPath("~/UploadFiles/" + item.cust_no + "/");

                                //create new file name
                                var new_fileName = DateTime.Now.Millisecond + "_" + fileName;

                                string sourceFile = Path.Combine(PathFile, fileName);
                                string destFile = Path.Combine(targetPath, new_fileName);

                                //copy file form sourceFile to destFile
                                System.IO.File.Copy(sourceFile, destFile, true);

                                //add data to DocFile table
                                var copy_file = new DocFile();
                                copy_file.file_name = targetPath1 + new_fileName;
                                copy_file.relation_id = item.id;
                                ex_doc.DocFile.Add(copy_file);

                            }
                        }
                        break;
                    }
                }// end switch case
            } // end if
            else // Standart&Manual
            {

                switch (sql.status_id) //check status in last transaction for crate next transaction
                {

                    case 4: //Dept. QS Appr
                        {


                            for (int i = 0; i < g_review.Length; i++)
                            {
                                //many_tran.status_id = 5; //5 = Mgr. Group Review
                                //many_tran.action_id = 0; //  0 = idle

                                if (!(string.IsNullOrWhiteSpace(remark[i]) || string.IsNullOrEmpty(remark[i])))
                                {
                                    Add_transaction(sql.issue_no, 5, 0, "0", null, g_review[i], 2, null, null, remark[i]);
                                }
                                else
                                {
                                    Add_transaction(sql.issue_no, 5, 0, "0", null, g_review[i], 2, null, null, null);
                                }
                                var add_group_review = new GroupReview();
                                add_group_review.group_id = g_review[i].Value;
                                add_group_review.issue_no = issue_no;
                                ex_doc.GroupReview.Add(add_group_review); 
                            }

                            break;
                        }

                    case 5: //Mgr Group Review
                        {

                            ex_doc.SaveChanges();//update before check

                            //check status all group mgr. review
                            //status_id = 5 ( Mgr Group Review )
                            //action_id = 0 ( idle )

                            var check_group_review = ex_doc.Transaction.Where(a => a.issue_no == issue_no && a.status_id == 5)
                                                                       .All(a => a.action_id != 0);

                            if (check_group_review)
                            {
                                var check_not_remark = ex_doc.Transaction.Where(a => a.issue_no == issue_no)
                                                                     .All(a=> a.remark == null );

                                var check_action_all = ex_doc.Transaction.Where(a => a.issue_no == issue_no)
                                     .All(a => a.action_id != 0);


                                if (check_action_all)
                                {
                                    if (check_not_remark)
                                    {
                                        //status_id = 100; //complete
                                        //action_id = 100;//completed

                                        Add_transaction(issue_no, 100, 100, "0", DateTime.Now, null, null, null, null, null);

                                        //select file form DocFileBeforeAppr table 
                                        var doc_file = ex_doc.DocFileBeforeAppr.Where(a => a.issue_no == issue_no).Select(a => a).ToList();

                                        //select cust in this issue
                                        var get_cust = ex_doc.Relation_Issue_Cust.Where(a => a.issue_id == issue_no).Select(a => a).ToList();

                                        //loop customer
                                        foreach (var item in get_cust)
                                        {
                                            //loop add file to new path
                                            foreach (var item2 in doc_file)
                                            {
                                                //get file name ex. 412_dog.pdf
                                                string fileName = Path.GetFileName(item2.path_file);

                                                // MapPath ex. c/webapp/UploadFiles/TempFile/ use in Path.Combine
                                                string PathFile = Server.MapPath("~/UploadFiles/TempFile/");

                                                //create folder if not found
                                                string targetPath1 = "~/UploadFiles/" + item.cust_no + "/";
                                                if (!Directory.Exists(Server.MapPath(targetPath1)))
                                                {
                                                    Directory.CreateDirectory(Server.MapPath(targetPath1));
                                                }

                                                // use in Path.Combine
                                                string targetPath = Server.MapPath("~/UploadFiles/" + item.cust_no + "/");

                                                //create new file name
                                                var new_fileName = DateTime.Now.Millisecond + "_" + fileName;

                                                string sourceFile = Path.Combine(PathFile, fileName);
                                                string destFile = Path.Combine(targetPath, new_fileName);

                                                //copy file form sourceFile to destFile
                                                System.IO.File.Copy(sourceFile, destFile, true);

                                                //add data to DocFile table
                                                var copy_file = new DocFile();
                                                copy_file.file_name = targetPath1 + new_fileName;
                                                copy_file.relation_id = item.id;
                                                ex_doc.DocFile.Add(copy_file);

                                            }
                                        }
                                    }
                                    else // have remark
                                    {

                                        //create_tran2.status_id = 7; //7  QS Dept last check
                                        //create_tran2.action_id = 0; //  0 = idle
                                        //create_tran2.org_id = 49; // 18 = qs
                                        //create_tran2.level_id = 3; // 3 = edpt.

                                        Add_transaction(issue_no, 7, 0, "0", null, 49, 3, null, null, null);

                                    }
                                }

                            }

                            break;
                        }
                    case 6: //Mgr Issuer Appr after reject
                        {
                            var get_mgr_notaccpet = ex_doc.Transaction.Where(a => a.issue_no == issue_no && a.status_id == 5 && a.action_id == 7).Select(a => a.org_id).Distinct();

                            foreach (var item in get_mgr_notaccpet.ToList())
                            {
                                //var many_tran = new Transaction();
                                //many_tran.issue_no = sql.issue_no;
                                //many_tran.status_id = 5; // Mgr. group review
                                //many_tran.action_id = 0;
                                //many_tran.actor = "0";
                                //many_tran.actor_date = null;
                                //many_tran.org_id = item;
                                //many_tran.level_id = 2; // 2 = Mgr. level
                                //many_tran.comment = null;
                                //ex_doc.Transaction.Add(many_tran);

                                Add_transaction(sql.issue_no, 5, 0, "0", null, item, 2, null, null, null);
                            }
                            break;
                        }

                    case 7: //QS Dept. Last check
                        {
                                 //create_tran2.issue_no = issue_no;
                                 //   create_tran2.status_id = 100; //complete
                                 //   create_tran2.action_id = 100;//completed
                                 //   create_tran2.actor = "0";
                                 //   create_tran2.actor_date = DateTime.Now;
                                 //   create_tran2.org_id = null;
                                 //   create_tran2.level_id = null;
                                 //   create_tran2.comment = null;
                                 //   ex_doc.Transaction.Add(create_tran2);

                            Add_transaction(issue_no, 100, 100, "0", DateTime.Now, null, null, null, null, null);

                                    //select file form DocFileBeforeAppr table 
                                    var doc_file = ex_doc.DocFileBeforeAppr.Where(a => a.issue_no == issue_no).Select(a => a).ToList();

                                    //select cust in this issue
                                    var get_cust = ex_doc.Relation_Issue_Cust.Where(a => a.issue_id == issue_no).Select(a => a).ToList();

                                    //loop customer
                                    foreach (var item in get_cust)
                                    {
                                        //loop add file to new path
                                        foreach (var item2 in doc_file)
                                        {
                                            //get file name ex. 412_dog.pdf
                                            string fileName = Path.GetFileName(item2.path_file);

                                            // MapPath ex. c/webapp/UploadFiles/TempFile/ use in Path.Combine
                                            string PathFile = Server.MapPath("~/UploadFiles/TempFile/");

                                            //create folder if not found
                                            string targetPath1 = "~/UploadFiles/" + item.cust_no + "/";
                                            if (!Directory.Exists(Server.MapPath(targetPath1)))
                                            {
                                                Directory.CreateDirectory(Server.MapPath(targetPath1));
                                            }

                                            // use in Path.Combine
                                            string targetPath = Server.MapPath("~/UploadFiles/" + item.cust_no + "/");

                                            //create new file name
                                            var new_fileName = DateTime.Now.Millisecond + "_" + fileName;

                                            string sourceFile = Path.Combine(PathFile, fileName);
                                            string destFile = Path.Combine(targetPath, new_fileName);

                                            //copy file form sourceFile to destFile
                                            System.IO.File.Copy(sourceFile, destFile, true);

                                            //add data to DocFile table
                                            var copy_file = new DocFile();
                                            copy_file.file_name = targetPath1 + new_fileName;
                                            copy_file.relation_id = item.id;
                                            ex_doc.DocFile.Add(copy_file);
                                        }
                                    }
                            break;
                        }

                }

            } //end else


           ex_doc.SaveChanges();//save to database

            return RedirectToAction("Index", "Issue");
        }

        [HttpPost]
        public ActionResult Cancel(string emp_code, int user_lvl, int g_id, string issue_no, int seq, int doc_type_id, string comment, HttpPostedFileBase pic)
        {

            var sql = ex_doc.Transaction.Where(a => a.seq == seq).FirstOrDefault();

            sql.action_id = 5; // 5 = canceled
            sql.actor = emp_code;
            sql.actor_date = DateTime.Now;
            ex_doc.SaveChanges();

            return RedirectToAction("Index", "Issue");
        }


        [Tnc_Auth]
        [HttpPost]
        public ActionResult Reject(string emp_code, int user_lvl, int g_id, string issue_no, int seq, int doc_type_id,string comment,HttpPostedFileBase pic)
        {

            var sql = ex_doc.Transaction.Where(a => a.seq == seq).FirstOrDefault();

            if (sql.status_id == 5) // mgr. group review
            {
                sql.action_id = 7; // 7 = not accepted
            }
            else
            {
                sql.action_id = 3; // 3 = rejected
            }

            sql.actor = emp_code;
            sql.actor_date = DateTime.Now;
            sql.comment = comment;
            sql.remark = null;

            if (pic != null)
            {
                string subPath = "~/CommentFile/" + issue_no + "/";
                if (!Directory.Exists(Server.MapPath(subPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(subPath));
                }

                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(pic.FileName.Replace('#', ' ').Replace('%', ' '));
                var path = Path.Combine(Server.MapPath(subPath), fileName);
                pic.SaveAs(path);

                sql.comment_pic = subPath + fileName;
            }

            var create_tran2 = new Transaction();
            var data_issuer = ex_doc.Transaction.Where(a => a.issue_no == issue_no).OrderBy(b => b.seq).FirstOrDefault();

            int get_doc_type = ex_doc.Issue.Where(a => a.issue_no == issue_no).Select(a => a.doc_type_id).FirstOrDefault();
            if (get_doc_type == 3)//check doc type
            {
                switch (sql.status_id)
                {
                    case 4:
                        {
                            Add_transaction(sql.issue_no, 0, 0, data_issuer.actor, null, data_issuer.org_id.Value, data_issuer.level_id.Value, null, null, null);
                            break;
                        }
                }
            }
            else
            {
                switch(sql.status_id)
                {
                    case 4:
                        {
                            Add_transaction(sql.issue_no, 0, 0, data_issuer.actor, null, data_issuer.org_id.Value, data_issuer.level_id.Value, null, null, null);
                            break;
                        }
                    case 5:
                        {

                            ex_doc.SaveChanges();

                            var find_reviewer = ex_doc.Transaction.Where(a => a.issue_no == issue_no && a.status_id == 5 && a.action_id == 0);

                            foreach (var item in find_reviewer.ToList())
                            {
                                item.action_id = 8; //auto not accepted
                            }

                            //ex_doc.SaveChanges();
                            //var check_action_all = ex_doc.Transaction.Where(a => a.issue_no == issue_no).All(a => a.action_id != 0);

                            //if (check_action_all)
                            //{
                                Add_transaction(sql.issue_no, 9, 0, "0", null, 49, 3, null, null, null);
                            //}
                            break;
                        }
                    case 7:
                        {
                            Add_transaction(sql.issue_no, 0, 0, data_issuer.actor, null, data_issuer.org_id.Value, data_issuer.level_id.Value, null, null, null);
                            break;
                        }
                }
            }

            ex_doc.SaveChanges();
            return RedirectToAction("Index", "Issue");

        }

        
        [HttpPost]
        public ActionResult SaveIssueV2(string[] customer, string doc_name, string doc_no, string doc_rev, string date_rec, string ch_point, string tnc_product, HttpPostedFileBase[] doc_file, string customer_type, int doc_type)
        {

            //setting runnumber issue
            string issue_no = run.GetRunNumber(programId); //get run nummber ex. 1706-0014
            run.SetRunNumber(programId); // set next run number

            //process doc file
            foreach (var doc in doc_file)
            {
                string subPath = "~/UploadFiles/TempFile/";//path wait all appr
                if (!Directory.Exists(Server.MapPath(subPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(subPath));
                }

                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(doc.FileName.Replace('#', ' ').Replace('%', ' '));
                var path = Path.Combine(Server.MapPath(subPath), fileName);
                doc.SaveAs(path);

                var filedb = new DocFileBeforeAppr();
                filedb.path_file = subPath + fileName;
                filedb.issue_no = issue_no;
                ex_doc.DocFileBeforeAppr.Add(filedb);
            }



            Issue save_issue = new Issue(); //define issue table
            DateTime d = Convert.ToDateTime(date_rec); // convert string date_rec to datetime type

            save_issue.issue_no = issue_no;
            save_issue.doc_name = doc_name;
            save_issue.doc_no = doc_no;
            save_issue.doc_rev = doc_rev;
            save_issue.rec_date = d;
            save_issue.tnc_product = tnc_product;
            save_issue.doc_type_id = doc_type;
            save_issue.issue_date = DateTime.Now;

            ex_doc.Issue.Add(save_issue); // add data to table 

            // create relation many customer to one issue
            foreach (var item in customer)
            {
                var indy_cust = new Relation_Issue_Cust();
                indy_cust.cust_no = item;
                indy_cust.issue_id = issue_no;
                ex_doc.Relation_Issue_Cust.Add(indy_cust);
            }

            //create issuer transaction
            //issue_no = issue_no;
            //status_id = 1; // 1 = Request Issue
            //action_id = 1; // 1 = Requested
            //actor = Session["emp_code"].ToString(); // emp_code issuer
            //org_id = int.Parse(Session["g_id"].ToString()); // group_id issuer
            //level_id = 1; // 1 = Issuer	,position_min = 1 ,	position_max = 4

            Add_transaction(issue_no, 1, 1, Session["emp_code"].ToString(), DateTime.Now, int.Parse(Session["g_id"].ToString()), 1, null, null, null);

            //create wait SQ Dept. Appr
            //status_id = 2; // 2 = Mgr. (Issuer)
            //action_id = 0; // 0 = idle
            //49 QS dept 
            //level_id = 3 = Dept. ,position_min = 6 ,	position_max = 6

            Add_transaction(issue_no, 4, 0, null, null, 49, 3, null, null, null);

            ex_doc.SaveChanges(); // save to database

            return RedirectToAction("Index", "Issue");
        }

    }



}
