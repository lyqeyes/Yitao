using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YiTao.Modules.Bll.Models;

namespace YiTao.Web.Areas.Watch.Common
{
    public class CityContext
    {
        public string CityName
        {
            get
            {
                return HttpContext.Current.Items["YitaoareaName"].ToString();
            }
        }

        public int CityId
        {
            get
            {
                return int.Parse(HttpContext.Current.Items["YitaoareaId"].ToString());
            }
        }

        public static CityContext Current
        {
            get 
            {
                return new CityContext();
            }
        }
    }
}