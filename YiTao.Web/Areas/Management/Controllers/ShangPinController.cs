using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Modules.Bll;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Management.Common;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class ShangPinController : ShangPinBaseController
    {
        #region 普通商品管理


        #region 大类
        public ActionResult DaLei()
        {
            return View(db.DaLeis.ToList());
        }

        public ActionResult DaLeiCreate(DaLei dalei)
        {
            dalei.CreateTime = System.DateTime.Now;
            if (db.DaLeis.Count(e => e.Name == dalei.Name) != 0)
                return RedirectToAction("DaLei");
            db.DaLeis.Add(dalei);
            db.SaveChanges();
            return RedirectToAction("DaLei");
        }

        public ActionResult DaLeiEdit(DaLei newdalei)
        {
            var dalei = db.DaLeis.Find(newdalei.DaLeiId);
            if (dalei == null)
                return RedirectToAction("Dalei");
            if (dalei.Name != newdalei.Name && db.DaLeis.Count(e => e.Name == newdalei.Name) != 0)
                return RedirectToAction("Dalei");
            dalei.Name = newdalei.Name;
            db.Entry(dalei).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Dalei");
        }

        public ActionResult DaLeiDel(int id)
        {
            var dalei = db.DaLeis.First(a => a.DaLeiId == id);
            if (dalei == null)
                return RedirectToAction("Dalei");
            db.DaLeis.Remove(dalei);
            db.SaveChanges();
            return RedirectToAction("Dalei");
        }
        #endregion

        #region 二级分类

        public ActionResult XiaoLei(int DaLeiId)
        {
            TempData["daleiID"] = DaLeiId;
            TempData["daleiName"] = db.DaLeis.Find(DaLeiId).Name;
            return View(db.TowLeis.Where(e => e.DaLeiId == DaLeiId).ToList());
        }

        public ActionResult XiaoLeiCreate(YiTao.Modules.Bll.Models.TowLei xiaolei)
        {
            xiaolei.DaLeiId = (int)TempData["daleiID"];
            xiaolei.CreateTime = System.DateTime.Now;
            if (db.DaLeis.Count(e => e.Name == xiaolei.Name) != 0)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            db.TowLeis.Add(xiaolei);
            db.SaveChanges();
            return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
        }

        public ActionResult XiaoLeiEdit(YiTao.Modules.Bll.Models.TowLei newxiaolei)
        {
            var xiaolei = db.TowLeis.Find(newxiaolei.TwoLeiId);
            if (xiaolei == null)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            if (xiaolei.Name != newxiaolei.Name && db.TowLeis.Count(e => e.Name == newxiaolei.Name) != 0)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            xiaolei.ImageUrl = xiaolei.ImageUrl;
            xiaolei.Name = xiaolei.Name;
            db.Entry(xiaolei).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
        }

        public ActionResult XiaoLeiDel(int id)
        {
            var xiaolei = db.TowLeis.Find(id);
            if (xiaolei == null)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            db.TowLeis.Remove(xiaolei);
            db.SaveChanges();
            return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
        }

        #endregion

        #region 三类
        public ActionResult ThreeLei(int DaLeiId)
        {
            TempData["daleiID"] = DaLeiId;
            TempData["daleiName"] = db.TowLeis.Find(DaLeiId).Name;
            return View(db.ThreeLeis.Where(e => e.TwoLeiId == DaLeiId).ToList());
        }

        public ActionResult ThreeLeiCreate(YiTao.Modules.Bll.Models.ThreeLei xiaolei)
        {
            xiaolei.TwoLeiId = (int)TempData["daleiID"];
            xiaolei.CreateTime = System.DateTime.Now;
            if (db.TowLeis.Count(e => e.Name == xiaolei.Name) != 0)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            db.ThreeLeis.Add(xiaolei);
            db.SaveChanges();
            return RedirectToAction("ThreeLei", new { DaLeiId = TempData["daleiID"] });
        }

        public ActionResult ThreeLeiEdit(YiTao.Modules.Bll.Models.ThreeLei newxiaolei)
        {
            var xiaolei = db.ThreeLeis.Find(newxiaolei.TwoLeiId);
            if (xiaolei == null)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            if (xiaolei.Name != newxiaolei.Name && db.ThreeLeis.Count(e => e.Name == newxiaolei.Name) != 0)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            xiaolei.ImageUrl = xiaolei.ImageUrl;
            xiaolei.Name = xiaolei.Name;
            db.Entry(xiaolei).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ThreeLei", new { daleiID = TempData["daleiID"] });
        }

        public ActionResult ThreeLeiDel(int id)
        {
            var xiaolei = db.ThreeLeis.Find(id);
            if (xiaolei == null)
                return RedirectToAction("xiaolei", new { daleiID = TempData["daleiID"] });
            db.ThreeLeis.Remove(xiaolei);
            db.SaveChanges();
            return RedirectToAction("ThreeLei", new { daleiID = TempData["daleiID"] });
        }
        #endregion


        public ActionResult CommonShangPin(int XiaoLeiId)
        {
            ViewBag.id = XiaoLeiId;
            return View(db.ShangPins.Where(e => e.XiaoLeiId == XiaoLeiId).ToList());
        }

        //public string CommonShangPinCreate(YiTao.Modules.Bll.Models.ShangPin shangpin)
        //{
        //    if (db.ShangPins.Count(e => e.Name == shangpin.Name 
        //        && e.XiaoLeiId == shangpin.XiaoLeiId) != 0)
        //        return "100";
        //    db.ShangPins.Add(shangpin);
        //    db.SaveChanges();

        //    #region 创建索引
        //    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
        //    {
        //        ShangPinId = shangpin.ShangPinId,
        //        Type = Web.Common.EnumShangPinType.PuTong,
        //        Name = shangpin.Name
        //    };
        //    Web.Common.Searcher.Add(SearchShangPin);
        //    #endregion

        //    return "200";
        //}

        public ActionResult CreateShangPin(int XiaoLeiId)
        {
            ViewBag.guishudi = db.Areas.Where(a => a.State == 1).ToList();
            return View(new ShangPin() { XiaoLeiId = XiaoLeiId });
        }

        [HttpPost]
        public ActionResult CreateShangPin(YiTao.Modules.Bll.Models.ShangPin shangpin)
        {
            var v = Request["guishudi"].Split(',');
            if (v.Length > 0)
            {
                if (shangpin.ShangPinId > 0)
                {
                    shangpin.CreateTime = DateTime.Now;
                    db.ShangPins.Attach(shangpin);
                    db.Entry<ShangPin>(shangpin).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    shangpin.CreateTime = DateTime.Now;
                    db.ShangPins.Add(shangpin);
                    db.SaveChanges();
                }

                var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ShangPinId &&
                a.Type == (int)YiTao.Modules.Bll.EnumAreaType.NormalShangpin);
                if (lists2a.Count() > 0)
                {
                    db.ShangPin2Areas.RemoveRange(lists2a);
                    db.SaveChanges();
                }
                foreach (var item in v)
                {
                    db.ShangPin2Areas.Add(new ShangPin2Areas
                    {
                        AreaId = int.Parse(item),
                        CreateTime = DateTime.Now,
                        ForId = shangpin.ShangPinId,
                        Type = (int)EnumAreaType.NormalShangpin
                    });
                }
                db.SaveChanges();

            }

            #region 创建索引
            Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
            {
                ShangPinId = shangpin.ShangPinId,
                Type = Web.Common.EnumShangPinType.PuTong,
                Name = shangpin.Name
            };
            Web.Common.Searcher.Add(SearchShangPin);
            #endregion

            return RedirectToAction("CommonShangPin", new { XiaoLeiId = shangpin.XiaoLeiId });
        }

        //public string CommonShangPinEdit(YiTao.Modules.Bll.Models.ShangPin newshangpin)
        //{
        //    var shangpin = db.ShangPins.Find(newshangpin.ShangPinId);
        //    if (shangpin == null)
        //        return "100";
        //    if (shangpin.Name != shangpin.Name && db.ShangPins.Count(e => e.Name == newshangpin.Name && e.XiaoLeiId == newshangpin.XiaoLeiId) != 0)
        //        return "50";

        //    shangpin.Name = newshangpin.Name;
        //    shangpin.ImageUrl = newshangpin.ImageUrl;
        //    shangpin.BaoYou = newshangpin.BaoYou;
        //    shangpin.CollectCount = newshangpin.CollectCount;
        //    shangpin.Description = newshangpin.Description;
        //    shangpin.FanJiFen = newshangpin.FanJiFen;
        //    shangpin.MonthlySales = newshangpin.MonthlySales;
        //    shangpin.Price = newshangpin.Price;
        //    shangpin.ToWhere = newshangpin.ToWhere;
        //    shangpin.TransportationPrice = newshangpin.TransportationPrice;
        //    shangpin.Url = newshangpin.Url;
        //    shangpin.YuanPrice = newshangpin.YuanPrice;
        //    db.Entry(shangpin).State = EntityState.Modified;
        //    db.SaveChanges();

        //    #region 更新索引
        //    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
        //    {
        //        ShangPinId = shangpin.ShangPinId,
        //        Type = Web.Common.EnumShangPinType.PuTong,
        //        Name = shangpin.Name
        //    };
        //    Web.Common.Searcher.Add(SearchShangPin);
        //    #endregion

        //    return "200";
        //}

        public ActionResult CommonShangPinDel(int ShangPinId)
        {
            int xiaoleiID = (int)db.ShangPins.Find(ShangPinId).XiaoLeiId;
            var shangpin = db.ShangPins.Find(ShangPinId);
            if (shangpin == null)
                return RedirectToAction("DaLei");

            var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ShangPinId
                && a.Type == (int)EnumAreaType.NormalShangpin);
            db.ShangPin2Areas.RemoveRange(lists2a);

            #region 删除索引
            Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
            {
                ShangPinId = shangpin.ShangPinId,
                Type = Web.Common.EnumShangPinType.PuTong,
                Name = shangpin.Name
            };
            Web.Common.Searcher.Del(SearchShangPin);
            #endregion

            db.ShangPins.Remove(shangpin);
            db.SaveChanges();
            return RedirectToAction("CommonShangPin", new { XiaoLeiId = xiaoleiID });
        }

        #endregion

        #region 兑换商品管理
        public ActionResult DuiHuanShangPin()
        {
            return View(db.DuiHuanItems.ToList());
        }

        public ActionResult DuiHuanShangPinCreate(DuiHuanItem shangpin)
        {
            if (db.DuiHuanItems.Count(e => e.Name == shangpin.Name) != 0)
                return RedirectToAction("DuiHuanShangPin");
            shangpin.CreateTime = System.DateTime.Now;
            db.DuiHuanItems.Add(shangpin);
            db.SaveChanges();
            return RedirectToAction("DuiHuanShangPin");
        }

        public ActionResult DuiHuanShangPinEdit(DuiHuanItem newshangpin)
        {
            var shangpin = db.DuiHuanItems.Find(newshangpin.DuiHuanItemId);
            if (shangpin == null)
                return RedirectToAction("DuiHuanShangPin");
            if (shangpin.Name != newshangpin.Name && db.DuiHuanItems.Count(e => e.Name == newshangpin.Name) != 0)
                return RedirectToAction("DuiHuanShangPin");
            shangpin.CreateTime = System.DateTime.Now;
            shangpin.Name = newshangpin.Name;
            shangpin.Description = newshangpin.Description;
            shangpin.Count = newshangpin.Count;
            db.Entry(shangpin).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DuiHuanShangPin");
        }

        public ActionResult DuiHuanShangPinDel(DuiHuanItem delItem)
        {
            var shangpin = db.DuiHuanItems.Find(delItem.DuiHuanItemId);
            if (shangpin == null)
                return RedirectToAction("DuiHuanShangPin");
            db.DuiHuanItems.Remove(shangpin);
            db.SaveChanges();
            return RedirectToAction("DuiHuanShangPin");
        }
        #endregion

        #region 抽奖商品管理
        public ActionResult ChouJiangShangPin()
        {
            var v = db.ChouJiangItems.ToList();
            return View(v);
        }

        [HttpPost]
        public string ChouJiangShangPin(YiTao.Modules.Bll.Models.ChouJiangItem newChouJiangItem)
        {
            try
            {
                var choujiangitem = db.ChouJiangItems.Find(newChouJiangItem.ChouJiangId);
                if (choujiangitem == null)
                {
                    return "100";
                }
                choujiangitem.Count = newChouJiangItem.Count;
                choujiangitem.Description = newChouJiangItem.Description;
                choujiangitem.Name = newChouJiangItem.Name;
                choujiangitem.Type = newChouJiangItem.Type;
                choujiangitem.ImageUrl = newChouJiangItem.ImageUrl;
                db.Entry(choujiangitem).State = EntityState.Modified;
                db.SaveChanges();
                return "200";
            }
            catch (Exception)
            {
                return "100";
            }
        }

        //public ActionResult ChouJiangShangPinCreate(YiTao.Modules.Bll.Models.ChouJiangItem shangpin)
        //{
        //    shangpin.CreateTime = DateTime.Now;
        //    db.ChouJiangItems.Add(shangpin);
        //    db.SaveChanges();
        //    return RedirectToAction("ChouJiangShangPin");
        //}

        //public ActionResult ChouJiangShangPinEdit(YiTao.Modules.Bll.Models.ChouJiangItem newshangpin)
        //{
        //    var shangpin = db.ChouJiangItems.Find(newshangpin.ChouJiangId);
        //    if (shangpin == null)
        //        return RedirectToAction("ChouJiangShangPin");
        //    if (shangpin.Name != newshangpin.Name && db.ChouJiangItems.Count(e => e.Name == newshangpin.Name) != 0)
        //        return RedirectToAction("ChouJiangShangPin");    
        //    //只能修改下面三项
        //    shangpin.Name = newshangpin.Name;
        //    shangpin.Description = newshangpin.Description;
        //    shangpin.Count = newshangpin.Count;
        //    db.Entry(shangpin).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("ChouJiangShangPin");
        //}

        //[HttpPost]
        //public ActionResult ChouJiangShangPinDel(ChouJiangItem delItem)
        //{            
        //    var shangpin = db.ChouJiangItems.First(a => a.ChouJiangId == delItem.ChouJiangId);
        //    if (shangpin == null)
        //        return RedirectToAction("ChouJiangShangPin");
        //    db.ChouJiangItems.Remove(shangpin);
        //    db.SaveChanges();
        //    return RedirectToAction("ChouJiangShangPin");
        //}
        #endregion
    }
}