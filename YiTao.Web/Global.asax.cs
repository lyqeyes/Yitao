using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using YiTao.Modules.Bll.Models;

namespace YiTao.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            using (var db = new YiTaoContext())
            {
                var area = HttpContext.Current.Request.Cookies["Area"];
                if (area == null)
                {
                    HttpCookie hc = new HttpCookie("Area");
                    var areafdb = db.Areas.Where(a => a.State == 1).OrderBy(a => a.Id).First();
                    hc.Values["Name"] = HttpUtility.UrlEncode(areafdb.Name);
                    hc.Values["Id"] = areafdb.Id.ToString();
                    HttpContext.Current.Response.Cookies.Add(hc);
                    HttpContext.Current.Items.Add("YitaoareaName", areafdb.Name);
                    HttpContext.Current.Items.Add("YitaoareaId", areafdb.Id);
                }
                else
                {
                    HttpContext.Current.Items.Add("YitaoareaName", HttpUtility.UrlDecode(area["Name"]));
                    HttpContext.Current.Items.Add("YitaoareaId", area["Id"]);
                }
            }  
        }

        protected void Application_EndRequest()
        {
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
