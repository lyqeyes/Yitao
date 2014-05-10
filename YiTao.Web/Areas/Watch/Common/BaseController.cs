using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using YiTao.Modules.Bll.Models;

namespace YiTao.Web.Areas.Watch.Common
{
    public class BaseController : Controller
    {
        protected YiTaoContext db;

        public BaseController()
        {
            db = new YiTaoContext();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Response.AppendHeader("Cache-Control", "no-cache"); 
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoLoginAttribute), false).Length > 0)
                return;
            var v = HttpContext.Request.Cookies["LoginInfo"];
            if (v == null)
            {
                filterContext.Result = RedirectToAction("Login", new { controller = "Auth", area = "watch" });
                //filterContext.Result = RedirectPermanent("/Watch/Auth/Login");
                return;
            }
            string sss = HttpUtility.UrlDecode(v.Value);
            string UserName = HttpUtility.UrlDecode(v["UserName"]);
            string Password = v["Password"];
            if (UserName == null || Password == null)
            {
                filterContext.Result = RedirectToAction("Login", new { controller = "Auth", area = "watch" });
                return;
            }
            if (db.Accounts.FirstOrDefault(e => e.Name == UserName && e.Password == Password) == null)
            {
                filterContext.Result = RedirectToAction("Login", new { controller = "Auth", area = "watch" });
                return;
            }
            return;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}