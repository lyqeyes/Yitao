using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using YiTao.Modules.Bll.Models;

namespace YiTao.Web.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 哈哈
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Index", "YiTao", new { Area = "Watch" });
        }

        public string GetImage()
        {
            try
            {
                using (YiTaoContext db = new YiTaoContext())
                {
                    var image = db.AppImages.OrderByDescending(a => a.AppImageId).FirstOrDefault();
                    if (image == null)
                    {
                        return "404";
                    }
                    else
                    {
                        return Request.Url.Host.ToLower() + image.Url;
                    }
                }
            }
            catch (Exception)
            {
                return "500";
            }
        }

        public ActionResult FindPassword(string id)
        {
            var temp = HttpRuntime.Cache.Get(id);
            if (temp!=null)
            {
                string s = temp.ToString();
                using (YiTaoContext db = new YiTaoContext())
                {
                    var account = db.Accounts.FirstOrDefault(a => a.Email == s);
                    if (account!=null)
                    {
                        return View(account);
                    }
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Loggering");

        public ActionResult test()
        {
            log.Info(@"这里是重置密码连接 "+" <a href='http://www.baicu.com'>重置密码连接</a> "+"，30分钟内有效，请尽快更改密码");
            return Content("1");
        }

        public string Upload()
        {
            try
            {
                HttpPostedFileBase uplode = Request.Files[0];
                if ((uplode == null))
                {
                    return ("error");
                }
                string FileName = Guid.NewGuid().ToString() + Path.GetExtension(Path.GetFileName(uplode.FileName));
                string fullUrl = Path.Combine(Server.MapPath(@"~/file/image"));
                if (Directory.Exists(fullUrl) == false)
                {
                    Directory.CreateDirectory(fullUrl); //如果文件夹不存在，直接创建文件夹。
                }
                var filepath = Path.Combine(fullUrl, FileName);
                uplode.SaveAs(filepath);
                return @"/file/image/" + FileName;
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public ActionResult Getinfo() 
        {
            return View();
        }
            
    }
}