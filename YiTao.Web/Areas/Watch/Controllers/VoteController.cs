using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Watch.Common;

namespace YiTao.Web.Areas.Watch.Controllers
{
    public class VoteController : BaseController
    {
        //
        // GET: /Watch/Vote/
        //public ActionResult VoteList()
        //{
        //    var topicList = db.VoteTopics.Where(a => a.IsStoped == 0);
        //    return View(topicList);
        //}
        public ActionResult VoteDetail()
        {
            var vote = db.VoteTopics.OrderByDescending(a => a.CreateTime).FirstOrDefault();
            if (vote == null)
                return View();
            else if (vote.EndTime < DateTime.Now)
                return View();
            else
            {
                //获取当前用户
                var userCookie = HttpContext.Request.Cookies["LoginInfo"];
                string UserName = HttpUtility.UrlDecode(userCookie["UserName"]);
                var user = db.Accounts.FirstOrDefault(a => a.Name == UserName);

                //获取当前客户给哪些选项投了票
                var votedListId = (from d in db.VoteAccounts
                                   where d.VoteGuid == vote.TopicGuid && d.AccountId == user.AccountId
                                   select d.VoteItemId).ToList();
                //获取当先投票所有选项
                var topicName = vote.TopicName;
                ViewBag.TipicName = topicName;
                ViewBag.TotalAble = vote.Type;
                ViewBag.Already = votedListId.Count();
                var itemList = db.VoteItems.Where(a => a.ItemGuid == vote.TopicGuid);
                foreach (var item in itemList)
                {
                    if (votedListId.Contains(item.ItemId))
                        item.IsVoted = 1;
                    else
                        item.IsVoted = 0;
                }
                return View(itemList);
            }
        }
        public string VoteById(int id)
        {
            //获取当前用户
            var userCookie = HttpContext.Request.Cookies["LoginInfo"];
            string UserName = HttpUtility.UrlDecode(userCookie["UserName"]);
            var user = db.Accounts.FirstOrDefault(a => a.Name == UserName);
            //获取投票的相关信息
            var voteitem = db.VoteItems.FirstOrDefault(a => a.ItemId == id);
            var topic = db.VoteTopics.FirstOrDefault(a => a.TopicGuid == voteitem.ItemGuid);
            //获取当前用户对于该项投票的投票情况
            var voteAccount = db.VoteAccounts.Where(a => a.AccountId == user.AccountId && a.VoteGuid == topic.TopicGuid);
            //如果投票总数没到
            if (voteAccount == null || voteAccount.Count() < topic.Type)
            {
                VoteAccount a = new VoteAccount()
                {
                    AccountId = user.AccountId,
                    VoteItemId = id,
                    VoteGuid = topic.TopicGuid
                };
                db.VoteAccounts.Add(a);
                voteitem.Count++;
                db.Entry(voteitem).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return "ok";
            }
            else
                return "full";
        }
    }
}