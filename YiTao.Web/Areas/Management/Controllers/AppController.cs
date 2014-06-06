using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Management.Common;


namespace YiTao.Web.Areas.Management.Controllers
{
    public class AppController : BaseController
    {
        // GET: Management/App
        public ActionResult Index()
        {
            return View();
        }
    }
}