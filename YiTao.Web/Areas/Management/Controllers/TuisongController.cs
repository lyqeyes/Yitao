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
using Webdiyer.WebControls.Mvc;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class TuisongController : BaseController
    {
        // GET: Management/Tuisong
        public ActionResult Index(int id=0)
        {
            return View(db.TuiSongs.OrderByDescending(a=>a.Id).ToPagedList(id,25));
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
            var query = db.TuiSongs.OrderByDescending(a=>a.Id).Select
                (a => new { a.Id, a.Title, a.CreateTime, a.Ok });
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
                tuisong.Ok = 0;
                tuisong.CreateTime = DateTime.Now;
                db.TuiSongs.Attach(tuisong);
                db.Entry(tuisong).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                tuisong.Ok = 0;
                tuisong.CreateTime = DateTime.Now;
                db.TuiSongs.Add(tuisong);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
            
        }

        public ActionResult Edit(int id)
        {
            var v = db.TuiSongs.Find(id);
            return View(v);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(TuiSong tuisong)
        {
            if (tuisong.Id > 0)
            {
                tuisong.Ok = 0;
                tuisong.CreateTime = DateTime.Now;
                db.TuiSongs.Attach(tuisong);
                db.Entry(tuisong).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                tuisong.Ok = 0;
                tuisong.CreateTime = DateTime.Now;
                db.TuiSongs.Add(tuisong);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        public string TuiSong(int id)
        {
            var tuisong = db.TuiSongs.Find(id);
            string s = JsonConvert.SerializeObject(new
            {
                msg = tuisong.Title,
                url = string.Format("http://{0}/Home/GetTuisong?id={1}", ConfigurationManager.AppSettings["hostname"], tuisong.Id)
            });
            string resule = PushSDK.pushMessageToApp(tuisong.Title, s);
            if (resule.Contains("ok"))
            {
                tuisong.Ok = 1;
                db.TuiSongs.Attach(tuisong);
                db.Entry(tuisong).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return "1";
            }
            else
            {
                return "0";
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

        public ActionResult Detial(int id)
        {
            return View(db.TuiSongs.Find(id));
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