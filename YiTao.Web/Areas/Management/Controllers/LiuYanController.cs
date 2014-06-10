using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Management.Common;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class LiuYanController : BaseController
    {
        //显示留言列表
        public ActionResult Index(int? id)
        {
            var liuYanList = db.LiuYans.Where(e => true).OrderByDescending(e => e.LiuYanId).ToList();
            PagedList<LiuYan> m = liuYanList.ToPagedList(id ?? 1, 5);
            return View(m);
        }

        //建立新留言
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LiuYan liuYan)
        {
            if (liuYan.Title == null)
                liuYan.Title = "无标题";
            if (liuYan.Detail == null)
                liuYan.Detail = "无内容";
            liuYan.CreateTime = DateTime.Now;
            liuYan.Count = 0;

            //将之前的全部设为结束
            var liuYanList = db.LiuYans.Where(e => true);
            foreach(var item in liuYanList)
            {
                item.EndTime = DateTime.Now;
                db.Entry(item).State = EntityState.Modified;
            }
            db.LiuYans.Add(liuYan);
            db.SaveChanges();
            //永久重定向
            Response.AppendHeader("Cache-Control", "no-cache");
            return RedirectPermanent("/Management/LiuYan/Index");
        }

        //显示留言详情
        public ActionResult Detail(int? id, int LiuYanId)
        {
            var liuYan = db.LiuYans.First(e => e.LiuYanId == LiuYanId);
            var liuYanCommentList = db.LiuYanComments.Where(e => e.LiuYanId == LiuYanId).OrderByDescending(e => e.LiuYanCommentId).ToList();
            PagedList<LiuYanComment> m = liuYanCommentList.ToPagedList(id ?? 1, 5);
            ViewData["LiuYan"] = liuYan;
            ViewData["LiuYanComment"] = m;
            return View();
        }

        //编辑留言
        [HttpGet]
        public ActionResult Edit(int LiuYanId)
        {
            return View(db.LiuYans.Find(LiuYanId));
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LiuYan newLiuYan)
        {
            if (newLiuYan.Title == null)
                newLiuYan.Title = "无标题";
            if (newLiuYan.Detail == null)
                newLiuYan.Detail = "无内容";

            var liuYan = db.LiuYans.Find(newLiuYan.LiuYanId);
            liuYan.Title = newLiuYan.Title;
            liuYan.Detail = newLiuYan.Detail;
            liuYan.StartTime = newLiuYan.StartTime;
            liuYan.EndTime = newLiuYan.EndTime;
            liuYan.CreateTime = DateTime.Now;
            db.Entry(liuYan).State = EntityState.Modified;
            db.SaveChanges();
            //永久重定向
            Response.AppendHeader("Cache-Control", "no-cache");
            return RedirectPermanent("/Management/LiuYan/Detail?LiuYanId=" + liuYan.LiuYanId);
        }
        //删除留言
        public ActionResult Del(int LiuYanId)
        {
            db.LiuYans.Remove(db.LiuYans.Find(LiuYanId));
            db.LiuYanComments.RemoveRange(db.LiuYanComments.Where(e => e.LiuYanId == LiuYanId));
            db.SaveChanges();
            //永久重定向
            Response.AppendHeader("Cache-Control", "no-cache");
            return RedirectPermanent("/Management/LiuYan/Index");
        }

        //删除留言的某条评论
        public ActionResult DelComment(int LiuYanCommentId)
        {
            var liuYanComment = db.LiuYanComments.Find(LiuYanCommentId);
            var liuYan = db.LiuYans.Find(liuYanComment.LiuYanId);
            db.LiuYanComments.Remove(liuYanComment);
            liuYan.Count -= 1;
            db.Entry(liuYan).State = EntityState.Modified;
            db.SaveChanges();
            //永久重定向
            Response.AppendHeader("Cache-Control", "no-cache");
            return RedirectPermanent("/Management/LiuYan/Detail?LiuYanId=" + liuYan.LiuYanId);
        }
	}
}