using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Watch.Common;

namespace YiTao.Web.Areas.Watch.Controllers
{
    public class VoteController : BaseController
    {
        //
        // GET: /Watch/Vote/
        public ActionResult VoteList()
        {
            var topicList = db.VoteTopics.Where(a => a.IsStoped == 0);
            return View(topicList);
        }
	}
}