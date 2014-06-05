using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YiTao.Web.Areas.Management.Common
{
    public class ShangPinBaseController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (db.Areas.Where(a=>a.State==1).Count()>0)
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = RedirectPermanent("/Management/AreaProvide/Index");
                return;
            }
        }
    }
}