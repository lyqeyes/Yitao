using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Watch.Common;
using YiTao.Modules.Bll.Models;
using System.Data.Entity;

namespace YiTao.Web.Areas.Watch.Controllers
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
        public ActionResult Login(YiTao.Modules.Bll.Models.Account newAccount)
        {
            var account = db.Accounts.FirstOrDefault(e => e.Name == newAccount.Name && e.Password == newAccount.Password);
            if (account != null)
            {
                //关闭浏览器Cookie即失效
                HttpCookie hc = new HttpCookie("LoginInfo");
                //hc.Values["UserName"] = HttpUtility.UrlDecode(account.Name);
                hc.Values["UserName"] = HttpUtility.UrlEncode(account.Name);
                hc.Values["Password"] = account.Password;
                Response.Cookies.Add(hc);
                //TODO: 重定向位置得改
                Response.AppendHeader("Cache-Control", "no-cache");

                //eyes 返回到原始页面
                var temp = HttpRuntime.Cache.Get("originalURL");
                if(temp != null)
                {
                    string[] divideAddress = temp.ToString().Split("/".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (divideAddress.Length == 4)
                    {
                        return RedirectToAction("index", new { controller = divideAddress[3], area = divideAddress[2] });
                    }
                    else {
                        return RedirectToAction(divideAddress[4], new { controller = divideAddress[3], area = divideAddress[2] });
                    }
                }

                return RedirectToAction("index", new { controller = "yitao", area = "watch" });
            }
            else
            {
                ViewBag.Error = "用户名或密码错误";
                return View(newAccount);
            }
        }

        //注册新会员
        [HttpGet]
        [NoLogin]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [NoLogin]
        public ActionResult Register(YiTao.Modules.Bll.Models.Account account)
        {
            if (db.Accounts.Count(e => e.Name == account.Name) != 0)
            {
                TempData["register"] = "用户名已被注册";
                return View();
            }
            else
            {
                account.CreateTime = DateTime.Now;
                account.JiFen = 0;
                TempData["register"] = "注册成功!请登录";
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("login");
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
            //return RedirectPermanent("/Watch/yitao/index");
            Response.AppendHeader("Cache-Control", "no-cache");
            return RedirectToAction("index", "yitao", new { area = "Watch" });
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Loggering");

        [NoLogin]
        public ActionResult ForgetPassword(string email)
        {
            var account = db.Accounts.FirstOrDefault(a => a.Email == email);
            if (account!=null)
            {
                
                Guid g = Guid.NewGuid();
                HttpRuntime.Cache.Insert(g.ToString(), email, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                //log.Info(@"这里是重置密码连接 " + String.Format("http://yitao.oureda.cn/Home/FindPassword/" + "{0}", g.ToString()) + "，30分钟内有效，请尽快更改密码");
                string msg = "这里是重置密码连接 " + String.Format("http://yitao.oureda.cn/Home/FindPassword/" + "{0}", g.ToString()) + "，30分钟内有效，请尽快更改密码";
                //AuctionStation.Framework.Utility.MailHelper.SendEmail("smtp.qq.com", "2064760020@qq.com", email, "亿淘", "yitao123", "亿淘密码重置", msg);
                Email sendemail = new Email();
                sendemail.mailFrom = "yitao_service@163.com";
                sendemail.mailPwd = "yitao123";
                sendemail.mailSubject = "亿淘重置密码连接";
                sendemail.mailBody = msg;
                sendemail.isbodyHtml = true;    //是否是HTML
                sendemail.host = "smtp.163.com";//如果是QQ邮箱则：smtp:qq.com,依次类推
                sendemail.mailToArray = new string[] { email };//接收者邮件集合
                sendemail.Send();
                return View();
            }
            ViewBag.msg = "1";
            return View();
        }
    }
}