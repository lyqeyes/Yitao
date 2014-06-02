using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YiTao.Web.Common
{
    public class ShangPin
    {
        public int ShangPinId { set; get; }
        public EnumShangPinType Type { set; get; }
        public string Name { set; get; }
        public string ShangPinIndex
        {
            get
            {
                return ShangPinId.ToString() + "____" + Type.ToString();
            }
        }
    }
}