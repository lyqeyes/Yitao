using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Watch.Common;
using YiTao.Modules.Bll.Models;
using System.Data.Entity;
using YiTao.Web.Areas.Watch.Models;
namespace YiTao.Web.Areas.Watch.Controllers
{
    public class PersonInforController : BaseController
    {
        //
        // GET: /Watch/PersonInfor/
        public ActionResult Index()
        {
            //根据cookie读取个人信息
            var v = HttpContext.Request.Cookies["LoginInfo"];
            string UserName = v["UserName"];
            string Password = v["Password"];
            Account acc = db.Accounts.FirstOrDefault(e => e.Name == UserName && e.Password == Password); 

            //检测今日是否签到
            JiFenLiShi qiandaoLast = (from d in db.JiFenLiShis where d.AccountId == acc.AccountId && d.Type == 1 select d).OrderByDescending(a => a.CreateTime).Take(1).FirstOrDefault();
            if (qiandaoLast == null || System.DateTime.Now.Day > qiandaoLast.CreateTime.Day)
            {
                ViewBag.whetherQiandao = "0";
            }
            else {
                ViewBag.whetherQiandao = "1";
            }
            return View(acc);
        }

        /// <summary>
        /// 用户的收藏信息  参数id为当前用户的id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Collects(int id)
        {
            List<int> colletListNum = (from d in db.Collections where d.AccountId == id select d.ShangPinId).ToList();
            List<ShangPin> colletList = (from d in db.ShangPins where colletListNum.Contains(d.ShangPinId) select d).ToList();
            return View(colletList);
        }
        /// <summary>
        /// 用户的奖品信息  参数id为当前用户的id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Prizes(int id)
        {
            List<JiangpinDetail> prezis = (from d in db.JiFenLiShis where d.AccountId == id && d.Type == 2 && d.ItemId != null select new JiangpinDetail{CreateTime = d.CreateTime, itemId=(int)d.ItemId}).ToList();
            foreach (JiangpinDetail item in prezis)
            {
                var temp = db.ChouJiangItems.FirstOrDefault(a => a.ChouJiangId == item.itemId);
                item.ImageUrl = temp.ImageUrl;
                item.Name = temp.Name;
            }
            return View(prezis);
        }
        /// <summary>
        /// 用户的积分记录信息  参数id为当前用户的id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult JifenHistories(int id)
        {
            List<JiFenLiShi> jifenList = (from d in db.JiFenLiShis where d.AccountId == id select d).ToList();
            return View(jifenList);
        }
        /// <summary>
        /// 用户的订单信息  参数id为当前用户的id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Orders(int id)
        {
            List<DingDan> dingdanList = (from d in db.DingDans where d.AccountId == id select d).ToList();
            return View(dingdanList);
        }

        //签到
        public ActionResult Qiandao()
        {
            var v = HttpContext.Request.Cookies["LoginInfo"];
            string UserName = v["UserName"];
            string Password = v["Password"];
            Account acc = db.Accounts.FirstOrDefault(e => e.Name == UserName && e.Password == Password);
            //签到加多少分?
            acc.JiFen += 1;

            JiFenLiShi newOne = new JiFenLiShi()
            {
                AccountId = acc.AccountId,
                AccountName = acc.Name,
                Description = "签到加积分",
                Type = 1,
                JiFen = 1,
                CreateTime = System.DateTime.Now,
            };
            db.Entry(acc).State = EntityState.Modified;
            db.JiFenLiShis.Add(newOne);
            db.SaveChanges();
            return RedirectToAction("index");
        }

        public ActionResult Info()
        {
            return View();
        }
	}
}