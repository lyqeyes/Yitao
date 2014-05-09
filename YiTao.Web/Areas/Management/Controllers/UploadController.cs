using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YiTao.Web.Areas.Management.Common
{
    public class UploadController : BaseController
    {
        public string Upload()
        {
            try
            {
                HttpPostedFileBase uplode = Request.Files[0];
                if ((uplode == null))
                {
                    return ("error");
                }
                string FileName = Guid.NewGuid().ToString()+Path.GetExtension(Path.GetFileName(uplode.FileName));
                string fullUrl = Path.Combine(Server.MapPath(@"~/file/image" ));
                if (Directory.Exists(fullUrl) == false)
                {
                    Directory.CreateDirectory(fullUrl); //如果文件夹不存在，直接创建文件夹。
                }
                var filepath = Path.Combine(fullUrl, FileName);
                uplode.SaveAs(filepath);
                return @"/file/image/" + FileName;
            }
            catch (Exception)
            {
                return "error";
            }
        }
	}
}