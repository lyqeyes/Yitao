using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YiTao.Web.Areas.Watch.Models
{
    public class Statistic<T>
    {
        public T data { get; set; }
        public int IsCollected { get; set; }
    }
}