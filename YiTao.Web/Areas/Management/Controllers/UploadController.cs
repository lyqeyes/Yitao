using System;
using System.Collections;
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

        public JsonResult EditorUpload()
        {
            try
            {
                HttpPostedFileBase uplode = Request.Files["imgFile"];
                if ((uplode == null))
                {
                    return Json(new { error = 1, message = "请选择文件。" });
                }
                //定义允许上传的文件扩展名
                Hashtable extTable = new Hashtable();
                extTable.Add("image", "gif,jpg,jpeg,png,bmp");

                String fileName = uplode.FileName;
                String fileExt = Path.GetExtension(fileName).ToLower();
                if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable["image"]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                {
                    return Json(new { error = 1, message = "上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable["image"]) + "格式。" });
                }
                //最大文件大小
                int maxSize = 1000000;
                if (uplode.InputStream == null || uplode.InputStream.Length > maxSize)
                {
                    return Json(new { error = 1, message = "上传文件大小超过限制。" });
                }

                string FileName = Guid.NewGuid().ToString() + Path.GetExtension(Path.GetFileName(uplode.FileName));
                string fullUrl = Path.Combine(Server.MapPath(@"~/file/image"));
                if (Directory.Exists(fullUrl) == false)
                {
                    Directory.CreateDirectory(fullUrl); //如果文件夹不存在，直接创建文件夹。
                }
                var filepath = Path.Combine(fullUrl, FileName);
                uplode.SaveAs(filepath);
                return Json(new { error = 0, message = "", url = @"/file/image/" + FileName });
            }
            catch (Exception)
            {
                return Json(new { error = 1, message = "未知异常。" });
            }
        }
	}
}