//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace YiTao.Modules.Bll.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VoteItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public System.Guid ItemGuid { get; set; }
        public int Count { get; set; }
        public string ImgURL { get; set; }
        public Nullable<int> IsVoted { get; set; }
    }
}
