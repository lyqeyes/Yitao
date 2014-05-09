using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Management.Common;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class AppWelcomeController : BaseController
    {
        //
        // GET: /Management/AppWelcome/
        public ActionResult Index()
        {
            var first = db.AppImages.OrderByDescending(a => a.AppImageId).FirstOrDefault();
            if (first == null)
            {
                ViewBag.url = "null";
            }
            else
            {
                ViewBag.url = first.Url;
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateImage(string url)
        {
            db.AppImages.Add(new AppImage() { Url = url, CreateTime = DateTime.Now });
            db.SaveChanges();
            return RedirectToAction("Index");
        }
	}
}