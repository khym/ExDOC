using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExDoc.Controllers
{
    public class Tnc_Auth : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["emp_code"] == null)
            {
                string loginpath = "~/Home/Index";
                if (HttpContext.Current.Request.Url != null)
                {
                    HttpContext.Current.Session["Redirect"] = HttpContext.Current.Request.Url;
                }
                filterContext.Result = new RedirectResult(loginpath);
            }
        }
    }
}