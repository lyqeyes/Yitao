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
            
            //Response.AppendHeader("Cache-Control", "no-cache"); 
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoLoginAttribute), false).Length > 0)
                return;
            var v = HttpContext.Request.Cookies["LoginInfo"];
            if (v == null)
            {
                //说明: 
                //如果当前操作是要发送数据(即表单内容不空), 则登录后返回登录前页面.
                //如果是请求一个页面, 则登录后返回当前所请求的页面
                //大眼睛留  5.13
                if (Request.Form.ToString() == "")
                {
                    HttpRuntime.Cache.Insert("originalURL", Request.Url, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                }
                else
                {
                    HttpRuntime.Cache.Insert("originalURL", Request.UrlReferrer, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                }
                filterContext.Result = RedirectToAction("Login", new { controller = "Auth", area = "watch" });
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