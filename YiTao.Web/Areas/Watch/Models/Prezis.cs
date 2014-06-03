using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YiTao.Web.Areas.Watch.Models
{
    public class Prezis
    {
        public string firstPrize { get; set; }
        public string firstImg { get; set; }
        public string secondPrize { get; set; }
        public string secondImg { get; set; }
        public string thirdPrize { get; set; }
        public string thirdImg { get; set; }
        public Prezis()
        {
            firstPrize = "没有";
            firstImg = null;
            secondPrize = "没有";
            secondImg = null;
            thirdPrize = "没有";
            thirdImg = null;
        }
    }
}