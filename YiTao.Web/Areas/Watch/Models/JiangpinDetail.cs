using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YiTao.Web.Areas.Watch.Models
{
    public class JiangpinDetail
    {
        public int itemId { get; set; } 
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateTime { get; set; }
    }
}