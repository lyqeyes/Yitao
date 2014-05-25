using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YiTao.Modules.Bll.Models;

namespace YiTao.Web.Areas.Watch.Models
{
    public class CollectionStatistic
    {
        public JuZheKouItem juzhekouItem {get; set; }
        public ShangPin shangpin { get; set; }
        public ZhuanTiItem zhuantiItem { get; set; }
        public int type { get; set; }

        public DateTime CreateTime { get; set; }
    }
}