using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Management.Common;
using YiTao.Web.Areas.Management.Models;
using Webdiyer.WebControls.Mvc;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class JiangpinDealController : BaseController
    {
        //
        // GET: /Management/JiangpinDeal/
        public ActionResult ZhongjiangList(int? id)
        {
            var list = (from d in db.JiFenLiShis
                        join n in db.UserAddresses
                        on d.AccountId equals n.UserId
                        join m in db.ChouJiangItems
                        on d.ItemId equals m.ChouJiangId
                        where d.Type == 2 && d.ItemId != null && d.WhetherDealed == 0
                        select new JiangpinInfor()
                        {
                            JifenlishiId = d.JiFenLiShiId,
                            AccountName = d.AccountName,
                            Phone = n.Phone,
                            Address = n.Address,
                            ShouhuorenName = n.ConsigneeName,
                            JiangpinName = m.Name,
                            JiangPinImg = m.ImageUrl
                        }).OrderByDescending(a=>a.JifenlishiId).ToPagedList(id ?? 1, 20);
            return View(list);
        }
        public ActionResult ZhongjiangDealedList(int? id)
        {
            var list = (from d in db.JiFenLiShis
                        join n in db.UserAddresses
                        on d.AccountId equals n.UserId
                        join m in db.ChouJiangItems
                        on d.ItemId equals m.ChouJiangId
                        where d.Type == 2 && d.ItemId != null && d.WhetherDealed == 1
                        select new JiangpinInfor()
                        {
                            JifenlishiId = d.JiFenLiShiId,
                            AccountName = d.AccountName,
                            Phone = n.Phone,
                            Address = n.Address,
                            ShouhuorenName = n.ConsigneeName,
                            JiangpinName = m.Name,
                            JiangPinImg = m.ImageUrl
                        }).OrderByDescending(a => a.JifenlishiId).ToPagedList(id ?? 1, 20);
            return View(list);
        }
        public ActionResult DuihuanList(int? id)
        {
            var list = (from d in db.JiFenLiShis
                        join n in db.UserAddresses
                        on d.AccountId equals n.UserId
                        join m in db.DuiHuanItems
                        on d.ItemId equals m.DuiHuanItemId
                        where d.Type == 3 && d.ItemId != null && d.WhetherDealed == 0
                        select new JiangpinInfor()
                        {
                            JifenlishiId = d.JiFenLiShiId,
                            AccountName = d.AccountName,
                            Phone = n.Phone,
                            Address = n.Address,
                            ShouhuorenName = n.ConsigneeName,
                            JiangpinName = m.Name,
                            JiangPinImg = m.ImageUrl
                        }).OrderByDescending(a => a.JifenlishiId).ToPagedList(id ?? 1, 20);
            return View(list);
        }
        public ActionResult DuihuanDealedList(int? id)
        {
            var list = (from d in db.JiFenLiShis
                        join n in db.UserAddresses
                        on d.AccountId equals n.UserId
                        join m in db.DuiHuanItems
                        on d.ItemId equals m.DuiHuanItemId
                        where d.Type == 3 && d.ItemId != null && d.WhetherDealed == 1
                        select new JiangpinInfor()
                        {
                            JifenlishiId = d.JiFenLiShiId,
                            AccountName = d.AccountName,
                            Phone = n.Phone,
                            Address = n.Address,
                            ShouhuorenName = n.ConsigneeName,
                            JiangpinName = m.Name,
                            JiangPinImg = m.ImageUrl
                        }).OrderByDescending(a => a.JifenlishiId).ToPagedList(id ?? 1, 20);
            return View(list);
        }
        public ActionResult DealItem(int id)
        {
            JiFenLiShi item = db.JiFenLiShis.FirstOrDefault(a => a.JiFenLiShiId == id);
            item.WhetherDealed = 1;
            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            if (item.Type == 2)
                return RedirectToAction("ZhongjiangList");
            else
                return RedirectToAction("DuihuanList");
        }
	}
}