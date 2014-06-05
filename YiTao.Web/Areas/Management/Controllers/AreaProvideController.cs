using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Management.Common;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class AreaProvideController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 拿到用户数据
        /// </summary>
        /// <param name="secho"></param>
        /// <returns></returns>
        public ActionResult Get(int? secho)
        {
            var query = db.Areas.Where(a => a.State == 1).Select(a => new { a.Id, a.Name,a.CreateTime, a.State });
            var objs = new List<object>();
            foreach (var city in query)
            {
                objs.Add(GetPropertyList(city).ToArray());
            }
            return Json(new
            {
                sEcho = secho,
                iTotalRecords = query.Count(),
                aaData = objs,
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPari()
        {
            return View(db.Areas.Where(a=>a.State==1));
        }
        /// <summary>
        /// 新建用户页面
        /// </summary>
        /// <returns>返回页面</returns>
        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit(int id) 
        {
            var v = db.Areas.FirstOrDefault(a => a.Id == id);
            if (v == null)
            {
                return RedirectToAction("Index");
            }
            return View(v);
        }
        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="adminuser">传入用户模型类</param>
        /// <returns>重定向到UserIndex</returns>
        [HttpPost]
        public ActionResult Add(YiTao.Modules.Bll.Models.Area areas)
        {

            if (areas.Id==0)
            {

                areas.State = 1;
                areas.CreateTime = DateTime.Now;
                db.Areas.Add(areas);
                db.SaveChanges();
            }
            else
            {
                db.Areas.Attach(areas);
                db.Entry(areas).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Del(int id)
        {
            var v = db.Areas.FirstOrDefault(a => a.Id == id);
            if (v == null)
            {
                return RedirectToAction("Index");
            }
            v.State = 0;
            db.Areas.Attach(v);
            db.Entry(v).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private List<string> GetPropertyList(object obj)
        {
            var propertyList = new List<string>();
            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                object o = property.GetValue(obj, null);
                propertyList.Add(o == null ? "" : o.ToString());
            }
            return propertyList;
        }
    }
}