﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Watch.Common;
using YiTao.Web.Areas.Watch.Models;

namespace YiTao.Web.Areas.Watch.Controllers
{
    /// <summary>
    /// 积分兑换与抽奖模块
    /// </summary>
    public class DuihuanController : BaseController
    {
        // 
        // GET: /Watch/Duihuan/        
        public ActionResult Index()
        {            
            HttpCookie v = HttpContext.Request.Cookies["LoginInfo"];
            string UserName = v["UserName"].ToString();
            ViewData["account"] = db.Accounts.Where(a => a.Name == UserName).FirstOrDefault();
            return View(db.DuiHuanItems.Where(a=>a.Count > 0).ToList());
        }

        public ActionResult ChouJiang()
        {
            HttpCookie v = HttpContext.Request.Cookies["LoginInfo"];
            string UserName = v["UserName"].ToString();
            ViewData["account"] = db.Accounts.Where(a => a.Name == UserName).FirstOrDefault();

            //奖项
            Prezis p = new Prezis();
            p.firstPrize = db.ChouJiangItems.First(e => e.Type == 1).Name;
            p.secondPrize = db.ChouJiangItems.First(e => e.Type == 2).Name;
            p.thirdPrize = db.ChouJiangItems.First(e => e.Type == 3).Name;  
          
            return View(p);
        }

        public ActionResult HuanGou(int id)
        {
            DuiHuanItem dh = db.DuiHuanItems.Find(id);
            HttpCookie h = Request.Cookies["LoginInfo"];
            string name = h["UserName"];
            Account acc = db.Accounts.FirstOrDefault(a => a.Name == name);
            if (acc.JiFen > dh.JiFen && dh.Count > 0)
            {
                dh.Count--;   //数量减少
                acc.JiFen = acc.JiFen - dh.JiFen;
                TempData["infor"] = "兑换成功";
            }
            else {
                TempData["infor"] = acc.JiFen>dh.JiFen? "商品已经被兑换完毕":"您的积分不够. 先努力赚积分吧";
                return RedirectToAction("Index");
            }
            JiFenLiShi newOne = new JiFenLiShi()
            {
                CreateTime = DateTime.Now,
                AccountId = acc.AccountId,
                Description = "积分兑换",
                JiFen = dh.JiFen,
                Type=3,
                AccountName = acc.Name,
                ItemId = dh.DuiHuanItemId,
                WhetherDealed = 0
            };
            db.Entry(acc).State = EntityState.Modified;
            db.Entry(db).State = EntityState.Modified;
            db.JiFenLiShis.Add(newOne);
            db.SaveChanges();
            return RedirectToAction("index");
        }
        //抽奖结果
        public string ChoujiangResult()
        {
            HttpCookie h = Request.Cookies["LoginInfo"];
            string name = h["UserName"];
            Account acc = db.Accounts.FirstOrDefault(a => a.Name == name);
            //一次扣一分            
            if (acc.JiFen == 0)
                return "-1";
            acc.JiFen = acc.JiFen - 1;            
            db.Entry(acc).State = EntityState.Modified;
            Random rad = new Random();
            int val = rad.Next(1, 100);
            JiFenLiShi newOne = new JiFenLiShi()
            {
                CreateTime = System.DateTime.Now,
                AccountId = acc.AccountId,
                AccountName = acc.Name,
                Description = "抽奖减积分",
                Type = 2,
                JiFen = 1,
                WhetherDealed = 0
            };
            if (val == 1)
            {
                ChouJiangItem item = db.ChouJiangItems.FirstOrDefault(e => e.Type == 1);
                if (item==null || item.Count == 0)
                {
                    newOne.ItemId = null;
                    db.JiFenLiShis.Add(newOne);
                    db.SaveChanges();
                    return "0";
                }
                item.Count--;
                db.Entry(item).State = EntityState.Modified;

                newOne.ItemId = item.ChouJiangId;
                db.JiFenLiShis.Add(newOne);
                db.SaveChanges();
                return "1";
            }
            else if (val > 1 && val < 7)
            {
                ChouJiangItem item = db.ChouJiangItems.FirstOrDefault(e => e.Type == 2);
                if (item == null || item.Count == 0)
                {
                    newOne.ItemId = null;
                    db.JiFenLiShis.Add(newOne);
                    db.SaveChanges();
                    return "0";
                }
                item.Count--;
                db.Entry(item).State = EntityState.Modified;

                newOne.ItemId = item.ChouJiangId;
                db.JiFenLiShis.Add(newOne);
                db.SaveChanges();
                return "2";
            }
            else if (val >= 7 && val < 17)
            {
                ChouJiangItem item = db.ChouJiangItems.FirstOrDefault(e => e.Type == 3);
                if (item == null || item.Count == 0)
                {
                    newOne.ItemId = null;
                    db.JiFenLiShis.Add(newOne);
                    db.SaveChanges();
                    return "0";
                }
                item.Count--;
                db.Entry(item).State = EntityState.Modified;

                newOne.ItemId = item.ChouJiangId;
                db.JiFenLiShis.Add(newOne);
                db.SaveChanges();
                return "3";
            }
            else
            {
                newOne.ItemId = null;
                db.JiFenLiShis.Add(newOne);
                db.SaveChanges();
                return "0";
            }
        }
	}
}