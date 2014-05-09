using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using YiTao.Modules.Bll.Models;

namespace YiTao.Web.Areas.Management.Common
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
                filterContext.Result = RedirectPermanent("/Management/Auth/Login");
                return;
            }
            string UserName = v["UserName"];
            string Password = v["Password"];
            if (UserName == null || Password == null)
            {
                filterContext.Result = RedirectPermanent("/Management/Auth/Login");
                return;
            }
            if (db.Managements.FirstOrDefault(e => e.UserName == UserName && e.Password == Password) == null)
            {
                filterContext.Result = RedirectPermanent("/Management/Auth/Login");
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