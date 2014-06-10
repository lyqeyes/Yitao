using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Management.Common;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class TuisongController : BaseController
    {
        // GET: Management/Tuisong
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Watch(int id)
        {
            var v = db.TuiSongs.Find(id);
            if (v==null)
            {
                return HttpNotFound();
            }
            return View(v);
        }

        public ActionResult Get(int? secho)
        {
            var query = db.TuiSongs.Select
                (a => new { a.Id, a.Title, a.CreateTime, action = "" });
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

        public ActionResult Add()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Add(TuiSong tuisong)
        {
            if (tuisong.Id > 0)
            {
                db.TuiSongs.Attach(tuisong);
                db.Entry(tuisong).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                tuisong.CreateTime = DateTime.Now;
                db.TuiSongs.Add(tuisong);
                db.SaveChanges();
            }
            string s = JsonConvert.SerializeObject(new
            {
                msg = tuisong.Title,
                url = string.Format("http://{0}/Home/GetTuisong?id={1}", ConfigurationManager.AppSettings["hostname"], tuisong.Id)
            });
            string resule = PushSDK.pushMessageToApp(tuisong.Title,s);
            if (resule.Contains("ok"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var v = db.TuiSongs.Find(tuisong.Id);
                db.TuiSongs.Remove(v);
                db.SaveChanges();
                ViewBag.msg = resule;
                return View(tuisong);
            }
        }

        public ActionResult Del(int id)
        {
            var v = db.TuiSongs.Find(id);
            if (v == null)
            {
                return RedirectToAction("Index");
            }
            db.TuiSongs.Remove(v);
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