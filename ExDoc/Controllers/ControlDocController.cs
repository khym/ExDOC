using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExDoc.Controllers
{
    public class ControlDocController : Controller
    {
        //
        // GET: /ControlDoc/
        [Tnc_Auth]
        public ActionResult Index()
        {
            return View();
        }

    }
}
