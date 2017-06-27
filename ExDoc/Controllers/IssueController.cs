﻿using System;
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

        public ActionResult _TransactionDetail(string issue_no)
        {

            //var sql = ex_doc.Issue.Where(a => a.issue_no.Contains(issue_no)).Select(a => new
            //{
            //    a.Transaction
            //}).FirstOrDefault();

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
                        status_id = v_trandetail[i].status_id

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

            //var ss = sql.Transaction.Join(tnc_admin.V_Employee_Info, a => a.actor, b => b.emp_code, (a, b) => new { a.Action.action_name, name = b.emp_fname + " " + b.emp_lname, a.actor_date, a.Status.status_name }).ToList();

            //var leftOuterJoinFluentSyntax = first.GroupJoin(second,
            //                          f => f,
            //                          s => s,
            //                          (f, s) => new { First = f, Second = s })
            //                       .SelectMany(temp => temp.Second.DefaultIfEmpty(),
            //                          (f, s) => new { First = f.First, Second = s });

            //var ss_groupjoin = sql.Transaction.GroupJoin(tnc_admin.V_Employee_Info,
            //                                f => f.actor,
            //                                s => s.emp_code,
            //                                (f, s) => new { f.action_id,f.Action.action_name,f.actor_date,f.Status.status_name,f.comment_pic,f.comment, s })
            //                                .SelectMany(temp => temp.s.DefaultIfEmpty(),
            //                                (f, s) => new
            //                                {
            //                                    f.action_id,
            //                                    f.action_name,
            //                                    f.actor_date,
            //                                    f.status_name,
            //                                    f.comment,
            //                                    f.comment_pic,
            //                                    Second = s
                                               
            //                                }).ToList();

            //List<TranVSEmpInfo> dd = new List<TranVSEmpInfo>();

            //for (int i = 0; i < ss_groupjoin.Count; i++)
            //{
            //    if (ss_groupjoin[i].Second != null)
            //    {
            //        dd.Add(new TranVSEmpInfo
            //        {
            //            action_id = ss_groupjoin[i].action_id.Value,
            //            action_name = ss_groupjoin[i].action_name,
            //            actor_name = ss_groupjoin[i].Second.emp_fname + " " + ss_groupjoin[i].Second.emp_lname,
            //            position_name = ss_groupjoin[i].Second.position_name,
            //            org_name = ss_groupjoin[i].Second.LeafOrgGroup,
            //            actor_date = ss_groupjoin[i].actor_date,
            //            status_name = ss_groupjoin[i].status_name,
            //            comment = ss_groupjoin[i].comment,
            //            comment_pic = ss_groupjoin[i].comment_pic
                        
            //        });
            //    }
            //    else
            //    {
            //        dd.Add(new TranVSEmpInfo
            //        {
            //            action_name = ss_groupjoin[i].action_name,
            //            actor_name = "-",
            //            actor_date = ss_groupjoin[i].actor_date,
            //            status_name = ss_groupjoin[i].status_name
            //        });
            //    }
            //}

            //for (int i = 0; i < ss.Count; i++)
            //{

            //    dd.Add(new TranVSEmpInfo { action_name = ss[i].action_name, actor_name = ss[i].name, actor_date = ss[i].actor_date, status_name = ss[i].status_name });
            //}



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

            //new transaction 
            if (sql.status_id == 8)
            {
                    //create wait Mgr. (Issuer) Appr after Mgr. group (not accept)
                    var create_tran2 = new Transaction();
                    create_tran2.issue_no = issue_no;
                    create_tran2.status_id = 6; // 6 = Mgr. (Issuer) After Mgr. Review (Not Accept)
                    create_tran2.action_id = 0; // 0 = idle
                    create_tran2.actor = null;
                    create_tran2.actor_date = null;
                    create_tran2.org_id = int.Parse(Session["g_id"].ToString()); // group_id Mgr. (Issuer) 
                    create_tran2.level_id = 2; // 2 = Mgr ,position_min = 5 ,	position_max = 5
                    create_tran2.comment = null;
                    ex_doc.Transaction.Add(create_tran2);
            }
            else
            {
                //create wait Mgr. (Issuer) Appr
                var create_tran2 = new Transaction();
                create_tran2.issue_no = issue_no;
                create_tran2.status_id = 2; // 2 = Mgr. (Issuer)
                create_tran2.action_id = 0; // 0 = idle
                create_tran2.actor = null;
                create_tran2.actor_date = null;
                create_tran2.org_id = int.Parse(Session["g_id"].ToString()); // group_id Mgr. (Issuer) 
                create_tran2.level_id = 2; // 2 = Mgr ,position_min = 5 ,	position_max = 5
                create_tran2.comment = null;
                ex_doc.Transaction.Add(create_tran2);
            }

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
            


            //var get_other = ex_doc.GroupReview.Where(a => a.issue_no.Contains(issue_no)).Select(a =>new { id = a.id, text = a.group_id});

            //var sql = ex_doc.GroupReview.Where(a => a.issue_no == issue_no).Select(a => a);

            //var group_review = sql.Join(tnc_admin.tnc_group_master, a => a.group_id, b => b.id, 
            //    (a, b) => new { id = a.group_id, text = b.group_name });

            //var group_review2 = from a in ex_doc.GroupReview.Where(a => a.issue_no.Contains(issue_no)).ToList()
            //                    join b in tnc_admin.tnc_group_master.ToList()
            //                    on a.group_id equals b.id select new { id= a.group_id , text = b.group_name };

            var test1 = ex_doc.GroupReview.Where(a => a.issue_no.Contains(issue_no)).ToList().Join(tnc_admin.tnc_group_master.ToList(),
                a => a.group_id, b => b.id, (a, b) => new { id = a.group_id, text = b.group_name });
                
                //.GroupJoin(tnc_admin.tnc_group_master,
                //        f => f.group_id,
                //        s => s.id,
                //        (f, s) => new { f, s })
                //        .SelectMany(temp => temp.s.DefaultIfEmpty(),
                //        (f, s) => new
                //        {
                //            first = f,
                //            second = s
                //        });

            //var test = ex_doc.GroupReview.Where(a => a.issue_no.Contains(issue_no)).GroupJoin(tnc_admin.tnc_group_master,
            //    f => f.group_id,
            //    s => s.id,
            //    (f, s) => new { f, s })
            //    .SelectMany(temp => temp.s.DefaultIfEmpty(),
            //    (f, s) => new
            //    {
            //        first = f,
            //        second = s
            //    });

                //GroupJoin(tnc_admin.tnc_group_master,
                //                 f => f.group_id,
                //                 s => s.id,
                //                 (f, s) => new { f.group_id, s })
                //                 .SelectMany(temp => temp.s.DefaultIfEmpty(),
                //                 (f, s) => new
                //                 {
                //                     f.group_id,
                //                     s.group_name

                //                 });


            return Json(test1, JsonRequestBehavior.AllowGet);
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
                doc_type_id = check_tran.Issue.doc_type_id

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

            DocDetail_V2 doc_detail = new DocDetail_V2()
            {
                issue_no = sql.issue_no,
                doc_type = sql.doc_type_name,
                doc_name = sql.doc_name,
                doc_no = sql.doc_no,
                doc_rev = sql.doc_rev,
                rec_date = sql.rec_date,
                change_point = sql.change_point,
                tnc_product = sql.tnc_product,
                check_tran = check_tran
            };



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
        [HttpPost]
        public ActionResult Approve(string emp_code, int user_lvl, int g_id, string issue_no, int seq, int doc_type_id, int?[] g_review, string remark)
        {
            var create_tran2 = new Transaction();

            //var sql = ex_doc.Transaction.Where(a => a.issue_no == issue_no).OrderByDescending(b => b.seq).Select(a=>a).FirstOrDefault();
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

            if (remark != null)
            {
                sql.remark = remark;
            }

            // check document type
            if (doc_type_id == 3) //Format
            {
                switch (check_status) //check status in last transaction for crate next transaction
                {
                    case 2: //Mgr. Isuuer Appr
                        {
                            create_tran2.issue_no = issue_no;
                            create_tran2.status_id = 3; // 3 = QS Officer
                            create_tran2.action_id = 0; //  0 = idle
                            create_tran2.actor = "0";
                            create_tran2.actor_date = null;
                            create_tran2.org_id = 18;
                            create_tran2.level_id = 1;
                            create_tran2.comment = null;
                            ex_doc.Transaction.Add(create_tran2);
                            break;
                        }
                    case 3: //QS Officer Appr
                        {
                            create_tran2.issue_no = issue_no;
                            create_tran2.status_id = 100; //complete
                            create_tran2.action_id = 100;//completed
                            create_tran2.actor = emp_code;
                            create_tran2.actor_date = DateTime.Now;
                            create_tran2.org_id = 18;
                            create_tran2.level_id = 1;
                            create_tran2.comment = null;
                            ex_doc.Transaction.Add(create_tran2);

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
                    case 2: //Mgr Issuer Appr
                        {
                            create_tran2.issue_no = sql.issue_no;
                            create_tran2.status_id = 3; // 3 = QS Officer
                            create_tran2.action_id = 0; //  0 = idle
                            create_tran2.actor = "0";
                            create_tran2.actor_date = null;
                            create_tran2.org_id = 18;
                            create_tran2.level_id = 1;
                            create_tran2.comment = null;
                            ex_doc.Transaction.Add(create_tran2);
                            break;
                        }
                    case 3: //QS Officer Appr
                        {

                            var remove_group_review = ex_doc.GroupReview.Where(a => a.issue_no == issue_no).Select(a => a).ToList();

                            if (remove_group_review != null)
                            {
                                foreach (var item in remove_group_review)
                                {
                                    ex_doc.GroupReview.Remove(item);
                                }
                                ex_doc.SaveChanges();
                            }
                            create_tran2.issue_no = issue_no;
                            create_tran2.status_id = 4; //4 = QS Dept.
                            create_tran2.action_id = 0; //  0 = idle
                            create_tran2.actor = "0";
                            create_tran2.actor_date = null;
                            create_tran2.org_id = 49; // 49 = qs system
                            create_tran2.level_id = 3;
                            create_tran2.comment = null;
                            ex_doc.Transaction.Add(create_tran2);

                            foreach (var item in g_review)
                            {
                                var add_group_review = new GroupReview();
                                add_group_review.group_id = item.Value;
                                add_group_review.issue_no = issue_no;
                                ex_doc.GroupReview.Add(add_group_review);
                            }

                            break;
                        }

                    case 4: //Dept. QS Appr
                        {

                            var remove_group_review = ex_doc.GroupReview.Where(a => a.issue_no == issue_no).Select(a => a).ToList();

                            foreach (var item in remove_group_review)
                            {
                                ex_doc.GroupReview.Remove(item);
                            }
                            ex_doc.SaveChanges();

                            foreach (var item in g_review)
                            {
                                var many_tran = new Transaction();
                                many_tran.issue_no = sql.issue_no;
                                many_tran.status_id = 5; //5 = Mgr. Group Review
                                many_tran.action_id = 0; //  0 = idle
                                many_tran.actor = "0";
                                many_tran.actor_date = null;
                                many_tran.org_id = item.Value;
                                many_tran.level_id = 2;
                                many_tran.comment = null;
                                ex_doc.Transaction.Add(many_tran);

                                var add_group_review = new GroupReview();
                                add_group_review.group_id = item.Value;
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
                                if (check_not_remark)
                                {
                                    create_tran2.issue_no = issue_no;
                                    create_tran2.status_id = 100; //complete
                                    create_tran2.action_id = 100;//completed
                                    create_tran2.actor = "0";
                                    create_tran2.actor_date = DateTime.Now;
                                    create_tran2.org_id = null;
                                    create_tran2.level_id = null;
                                    create_tran2.comment = null;
                                    ex_doc.Transaction.Add(create_tran2);

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
                                            //create_tran2.issue_no = issue_no;
                                            //create_tran2.status_id = 7; //7 last QS Offcer check
                                            //create_tran2.action_id = 0; //  0 = idle
                                            //create_tran2.actor = "0";
                                            //create_tran2.actor_date = null;
                                            //create_tran2.org_id = 18; // 18 = qs dcc
                                            //create_tran2.level_id = 1;
                                            //create_tran2.comment = null;
                                            //ex_doc.Transaction.Add(create_tran2);

                                            create_tran2.issue_no = issue_no;
                                            create_tran2.status_id = 7; //7  QS Dept last check
                                            create_tran2.action_id = 0; //  0 = idle
                                            create_tran2.actor = "0";
                                            create_tran2.actor_date = null;
                                            create_tran2.org_id = 49; // 18 = qs
                                            create_tran2.level_id = 3; // 3 = edpt.
                                            create_tran2.comment = null;
                                            ex_doc.Transaction.Add(create_tran2);

                                }

                            }

                            break;
                        }
                    case 6: //Mgr Issuer Appr after reject
                        {
                            var get_mgr_notaccpet = ex_doc.Transaction.Where(a => a.issue_no == issue_no && a.status_id == 5 && a.action_id == 3).Select(a => a.org_id).Distinct();

                            foreach (var item in get_mgr_notaccpet.ToList())
                            {
                                var many_tran = new Transaction();
                                many_tran.issue_no = sql.issue_no;
                                many_tran.status_id = 5; // Mgr. group review
                                many_tran.action_id = 0;
                                many_tran.actor = "0";
                                many_tran.actor_date = null;
                                many_tran.org_id = item;
                                many_tran.level_id = 2; // 2 = Mgr. level
                                many_tran.comment = null;
                                ex_doc.Transaction.Add(many_tran);
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
                sql.action_id = 7; // 7 = new accepted
            }
            else
            {
                sql.action_id = 3; // 3 = rejected
            }

            sql.actor = emp_code;
            sql.actor_date = DateTime.Now;
            sql.comment = comment;
            sql.remark = null;

            string subPath = "~/CommentFile/" + issue_no + "/";
            if (!Directory.Exists(Server.MapPath(subPath)))
            {
                Directory.CreateDirectory(Server.MapPath(subPath));
            }

            var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(pic.FileName.Replace('#', ' ').Replace('%', ' '));
            var path = Path.Combine(Server.MapPath(subPath), fileName);
            pic.SaveAs(path);

            sql.comment_pic = subPath + fileName;


            var create_tran2 = new Transaction();

            var data_issuer = ex_doc.Transaction.Where(a => a.issue_no == issue_no).OrderBy(b => b.seq).FirstOrDefault();


            if (sql.status_id == 5)
            {
                //Mgr. group not accept
                create_tran2.issue_no = sql.issue_no;
                create_tran2.status_id = 8; // 0 = Issuer edit
                create_tran2.action_id = 0; // 0 = Idle
                create_tran2.actor = data_issuer.actor;
                create_tran2.actor_date = null;
                create_tran2.org_id = data_issuer.org_id;
                create_tran2.level_id = data_issuer.level_id;
                create_tran2.comment = null;
                ex_doc.Transaction.Add(create_tran2);
            }
            else
            {
                //all reject
                create_tran2.issue_no = sql.issue_no;
                create_tran2.status_id = 0; // 0 = Issuer edit
                create_tran2.action_id = 0; // 0 = Idle
                create_tran2.actor = data_issuer.actor;
                create_tran2.actor_date = null;
                create_tran2.org_id = data_issuer.org_id;
                create_tran2.level_id = data_issuer.level_id;
                create_tran2.comment = null;
                ex_doc.Transaction.Add(create_tran2);
            }

            //// check document type
            //if (doc_type_id == 3) //Format
            //{
            //    switch (sql.status_id) //check status in last transaction for crate next transaction
            //    {
            //        case 2: //Mgr. Isuuer Reject
            //            create_tran2.issue_no = sql.issue_no;
            //            create_tran2.status_id = 0; // 0 = Issuer edit
            //            create_tran2.action_id = 0; // 0 = Idle
            //            create_tran2.actor = data_issuer.actor;
            //            create_tran2.actor_date = null;
            //            create_tran2.org_id = data_issuer.org_id;
            //            create_tran2.level_id = data_issuer.level_id;
            //            create_tran2.comment = null;
            //            ex_doc.Transaction.Add(create_tran2);
            //            break;
            //        case 3: //QS Officer Reject
            //            create_tran2.issue_no = sql.issue_no;
            //            create_tran2.status_id = 0;// 0 = Issuer edit
            //            create_tran2.action_id = 0; // 0 = Idle
            //            create_tran2.actor = data_issuer.actor;
            //            create_tran2.actor_date = null;
            //            create_tran2.org_id = data_issuer.org_id;
            //            create_tran2.level_id = data_issuer.level_id;
            //            create_tran2.comment = null;
            //            ex_doc.Transaction.Add(create_tran2);
            //            break;

            //    }
            //}
            //else // Standart&Manual
            //{

            //    switch (sql.status_id) //check status in last transaction for crate next transaction
            //    {
            //        case 2: //Mgr. Isuuer Reject
            //            create_tran2.issue_no = sql.issue_no;
            //            create_tran2.status_id = 0; // 0 = Issuer edit
            //            create_tran2.action_id = 0; // 0 = Idle
            //            create_tran2.actor = data_issuer.actor;
            //            create_tran2.actor_date = null;
            //            create_tran2.org_id = data_issuer.org_id;
            //            create_tran2.level_id = data_issuer.level_id;
            //            create_tran2.comment = null;
            //            ex_doc.Transaction.Add(create_tran2);
            //            break;
            //        case 3: //QS Officer Reject
            //            create_tran2.issue_no = sql.issue_no;
            //            create_tran2.status_id = 0;// 0 = Issuer edit
            //            create_tran2.action_id = 0; // 0 = Idle
            //            create_tran2.actor = data_issuer.actor;
            //            create_tran2.actor_date = null;
            //            create_tran2.org_id = data_issuer.org_id;
            //            create_tran2.level_id = data_issuer.level_id;
            //            create_tran2.comment = null;
            //            ex_doc.Transaction.Add(create_tran2);
            //            break;
            //        case 4: //QS Dept. Reject
            //            create_tran2.issue_no = sql.issue_no;
            //            create_tran2.status_id = 0;// 0 = Issuer edit
            //            create_tran2.action_id = 0; // 0 = Idle
            //            create_tran2.actor = data_issuer.actor;
            //            create_tran2.actor_date = null;
            //            create_tran2.org_id = data_issuer.org_id;
            //            create_tran2.level_id = data_issuer.level_id;
            //            create_tran2.comment = null;
            //            ex_doc.Transaction.Add(create_tran2);
            //            break;

            //        case 5: //Mgr. Group (Review) Reject
            //            create_tran2.issue_no = sql.issue_no;
            //            create_tran2.status_id = 0;// 0 = Issuer edit
            //            create_tran2.action_id = 0; // 0 = Idle
            //            create_tran2.actor = data_issuer.actor;
            //            create_tran2.actor_date = null;
            //            create_tran2.org_id = data_issuer.org_id;
            //            create_tran2.level_id = data_issuer.level_id;
            //            create_tran2.comment = null;
            //            ex_doc.Transaction.Add(create_tran2);
            //            break;

            //    }

            //}


            ex_doc.SaveChanges();

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
            var create_tran = new Transaction();
            create_tran.issue_no = issue_no;
            create_tran.status_id = 1; // 1 = Request Issue
            create_tran.action_id = 1; // 1 = Requested
            create_tran.actor = Session["emp_code"].ToString(); // emp_code issuer
            create_tran.actor_date = DateTime.Now;
            create_tran.org_id = int.Parse(Session["g_id"].ToString()); // group_id issuer
            create_tran.level_id = 1; // 1 = Issuer	,position_min = 1 ,	position_max = 4
            create_tran.comment = null;

            ex_doc.Transaction.Add(create_tran);

            //create wait Mgr. (Issuer) Appr
            var create_tran2 = new Transaction();
            create_tran2.issue_no = issue_no;
            create_tran2.status_id = 2; // 2 = Mgr. (Issuer)
            create_tran2.action_id = 0; // 0 = idle
            create_tran2.actor = null;
            create_tran2.actor_date = null;
            create_tran2.org_id = int.Parse(Session["g_id"].ToString()); // group_id Mgr. (Issuer) 
            create_tran2.level_id = 2; // 2 = Mgr ,position_min = 5 ,	position_max = 5
            create_tran2.comment = null;
            ex_doc.Transaction.Add(create_tran2);

            ex_doc.SaveChanges(); // save to database

            return RedirectToAction("Index", "Issue");
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
