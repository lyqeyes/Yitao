using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YiTao.Web.Areas.Watch.Common;
using YiTao.Modules.Bll.Models;
using Webdiyer.WebControls.Mvc;

namespace YiTao.Web.Areas.Watch.Models
{
    enum CollectStatus { Collected, Uncollect };
    public class JuzekouStatistic
    {
        public JuZheKouItem juzhekou { get; set; }
        public int IsCollected { get; set; }
    }
}