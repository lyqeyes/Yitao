using System;
using System.Collections.Generic;
using System.Configuration;
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
                    if (image != null)
                    {
                        String hostname = ConfigurationManager.AppSettings["hostname"];
                        if (!string.IsNullOrEmpty(hostname))
                        {
                            return hostname + image.Url;
                        }
                        
                    }
                    return "http://ouredaapk.qiniudn.com/4dbe05e3-7404-4002-babb-e9121b0ac5c5.jpg";
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

        public ActionResult PasswordOk(string name, string password, string newpassword)
        {
            using (YiTaoContext db = new YiTaoContext())
            {
                var account = db.Accounts.FirstOrDefault(a => a.Name == name);
                account.Password = password;
                db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return View();
            }
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

        public string Uploadapp()
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


        public ActionResult GetTuisong(int id)
        {
            using(YiTaoContext db = new YiTaoContext())
	{
        return View(db.TuiSongs.Find(id));
	}
            
        }

        public ActionResult download()
        {
            return View();
        }
    }
}