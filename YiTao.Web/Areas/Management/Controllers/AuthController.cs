using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Management.Common;
using YiTao.Modules.Bll.Models;
using System.Data.Entity;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class AuthController : BaseController
    {
        //登陆账户
        [HttpGet]
        [NoLogin]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [NoLogin]
        public ActionResult Login(YiTao.Modules.Bll.Models.Management newManagement)
        {
            var management = db.Managements.FirstOrDefault(e => e.UserName == newManagement.UserName && e.Password == newManagement.Password);
            if (management != null)
            {
                //关闭浏览器Cookie即失效
                HttpCookie hc = new HttpCookie("LoginInfo");
                hc.Values["UserName"] = management.UserName;
                hc.Values["Password"] = management.Password;
                Response.Cookies.Add(hc);
                Response.AppendHeader("Cache-Control", "no-cache"); 
                //TODO: 重定向位置得改
                return RedirectPermanent("/Management/MainPage/Lunbo");
            }
            else
            {
                ViewBag.Error = "用户名或密码错误";
                return View(newManagement);
            }
        }

        [HttpGet]
        public ActionResult Manage()
        {
            return View(db.Managements.ToList());
        }

        //注册新管理员
        [HttpPost]
        public string Register(YiTao.Modules.Bll.Models.Management management)
        {
            if (db.Managements.Count(e => e.UserName == management.UserName) != 0)
            {
                return "100";
            }
            else
            {
                db.Managements.Add(management);
                db.SaveChanges();
                return "200";
            }
        }

        //修改管理员密码
        [HttpPost]
        public string Edit(YiTao.Modules.Bll.Models.Management newManagement)
        {
            var management = db.Managements.FirstOrDefault(e => e.UserName == newManagement.UserName);
            if (management == null)
            {
                return "100";
            }
            else
            {
                management.Password = newManagement.Password;
                db.Entry(management).State = EntityState.Modified;
                db.SaveChanges();
                return "200";
            }     
        }

        //删除管理员账户
        [HttpPost]
        public string Del(YiTao.Modules.Bll.Models.Management newManagement)
        {
            if (newManagement.UserName == HttpContext.Request.Cookies["LoginInfo"]["UserName"])
            {
                return "50";
            }
            var management = db.Managements.FirstOrDefault(e => e.UserName == newManagement.UserName && e.Password == newManagement.Password);
            if (management == null)
            {
                return "100";
            }
            else
            {
                db.Managements.Remove(management);
                db.SaveChanges();
                return "200";
            }     
        }

        //注销
        public ActionResult Logout()
        {
            //清除Cookie并永久重定向
            var c = new HttpCookie("LoginInfo")
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Add(c);
            Response.AppendHeader("Cache-Control", "no-cache"); 
            return RedirectPermanent("/Management/Auth/Login");
        }
	}
}