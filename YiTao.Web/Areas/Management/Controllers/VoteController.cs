using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Management.Common;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class VoteController : BaseController
    {
        //
        // GET: /Management/Vote/
        [HttpGet]
        public ActionResult AddVote()
        {
            return View();
        }       
        [HttpPost]
        public ActionResult AddVotePost()
        {
            //1. 获取数据
            var votetopic = Request.Form["VoteTopic"];
            var votetype = Request.Form["Type"];  //可选的数量
            var itemnames = Request.Form["ItemNames"];
            var ImgURLs = Request.Form["ImgURLs"];
            DateTime endTime = DateTime.Parse(Request.Form["EndTime"]);
            var itemnameList = itemnames.ToString().Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var ImgURLList = ImgURLs.ToString().Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            //2.设置统一的GUID
            Guid code = Guid.NewGuid();
            //3.在VoteTopic表中添加数据
            db.VoteTopics.Add(new VoteTopic()
            {
                TopicGuid = code,
                Type = System.Int32.Parse(votetype),
                CreateTime = DateTime.Now,
                TopicName = votetopic,
                EndTime = endTime
            });
            //4.在VoteItem表中添加数据
            for (int i = 0; i < ImgURLList.Length; i++)
            {
                db.VoteItems.Add(new VoteItem()
                {
                    ItemGuid = code,
                    Count = 0,
                    ImgURL = ImgURLList[i],
                    ItemName = itemnameList[i]
                });
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder errors = new StringBuilder();
                IEnumerable<DbEntityValidationResult> validationResult = ex.EntityValidationErrors;
                foreach (DbEntityValidationResult result in validationResult)
                {
                    ICollection<DbValidationError> validationError = result.ValidationErrors;
                    foreach (DbValidationError err in validationError)
                    {
                        errors.Append(err.PropertyName + ":" + err.ErrorMessage + "\r\n");
                    }
                }
                Debug.WriteLine(errors.ToString());
            }
            return RedirectToAction("VoteList");
        }

        public ActionResult VoteList()
        {
            var topicList = db.VoteTopics.OrderByDescending(a => a.CreateTime);
            return View(topicList);
        }
        public ActionResult VoteDetail(Guid id)
        {
            var topic = db.VoteTopics.FirstOrDefault(a => a.TopicGuid == id);
            ViewBag.TopicName = topic.TopicName;
            var list = db.VoteAccounts.Where(a => a.VoteGuid == id);
            ViewBag.TotleVote = list.Count();
            var itemList = db.VoteItems.Where(a => a.ItemGuid == id).OrderByDescending(a => a.Count);
            return View(itemList);
        }        
    }
}