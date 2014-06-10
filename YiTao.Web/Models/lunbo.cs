using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace YiTao.Web.Models
{
    public class lunbo
    {
        [XmlAttribute("fangxiang")]
        public string fangxiang
        { set; get; }

    }
}