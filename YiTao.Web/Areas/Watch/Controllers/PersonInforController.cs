using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Watch.Common;
using YiTao.Modules.Bll.Models;
using System.Data.Entity;
using YiTao.Web.Areas.Watch.Models;
using Webdiyer.WebControls.Mvc;
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
            string UserName = HttpUtility.UrlDecode(v["UserName"]);
            string Password = v["Password"];
            Account acc = db.Accounts.FirstOrDefault(e => e.Name == UserName && e.Password == Password);

            //检测今日是否签到
            JiFenLiShi qiandaoLast = (from d in db.JiFenLiShis where d.AccountId == acc.AccountId && d.Type == 1 select d).OrderByDescending(a => a.CreateTime).Take(1).FirstOrDefault();
            if (qiandaoLast == null || System.DateTime.Now.Day > qiandaoLast.CreateTime.Day)
            {
                ViewBag.whetherQiandao = "0";
            }
            else
            {
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
            List<JiangpinDetail> prezis = (from d in db.JiFenLiShis where d.AccountId == id && d.Type == 2 && d.ItemId != null select new JiangpinDetail { CreateTime = d.CreateTime, itemId = (int)d.ItemId }).ToList();
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
            string UserName = HttpUtility.UrlDecode(v["UserName"]);
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
            var v = HttpContext.Request.Cookies["LoginInfo"];
            string UserName = HttpUtility.UrlDecode(v["UserName"]);
            string Password = v["Password"];
            ViewBag.Name = UserName;
            Account acc = db.Accounts.FirstOrDefault(e => e.Name == UserName && e.Password == Password);
            ViewBag.JiFen = (acc.JiFen / 100);
            ViewBag.NowJiFen = acc.JiFen;
            ViewBag.Phone = acc.Phone;
            return View();
        }

        public ActionResult ChangePassWord()
        {
            return View();

        }

        public ActionResult SetPassWord(string password, string newpassword, string checkpassword)
        {
            if (newpassword != checkpassword)
            {
                ModelState.AddModelError("error", "两次输入的密码不对");
            }
            else
            {
                var v = HttpContext.Request.Cookies["LoginInfo"];
                string name = HttpUtility.UrlDecode(v.Values["UserName"]);
                var account = db.Accounts.FirstOrDefault(a => a.Name == name && password == a.Password);
                if (account == null)
                {
                    ModelState.AddModelError("error", "原密码不正确");
                }
                else
                {
                    account.Password = newpassword;
                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Info");
                }
            }
            return View();
        }

        public ActionResult SerAddress()
        {
            var v = HttpContext.Request.Cookies["LoginInfo"];
            string name = HttpUtility.UrlDecode(v.Values["UserName"]);
            var account = db.Accounts.FirstOrDefault(a => a.Name == name);
            var address = db.UserAddresses.Where(u => u.UserId == account.AccountId);
            return View(address.ToList());
        }

        public ActionResult DelAddress(int id)
        {
            db.UserAddresses.Remove(db.UserAddresses.FirstOrDefault(u => u.UserAddressId == id));
            db.SaveChanges();
            return RedirectToAction("SerAddress");
        }

        public ActionResult SetAddress(UserAddress useraddress)
        {
            if (useraddress.UserAddressId>0)
            {
                useraddress.CreateTime = DateTime.Now;
                db.UserAddresses.Attach(useraddress);
                db.Entry(useraddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Info");
            }
            useraddress.CreateTime = DateTime.Now;
            var v = HttpContext.Request.Cookies["LoginInfo"];
            string name = HttpUtility.UrlDecode(v.Values["UserName"]);
            var account = db.Accounts.FirstOrDefault(a => a.Name == name);
            useraddress.UserId = db.Accounts.FirstOrDefault(a => a.Name == name).AccountId;
            db.UserAddresses.Add(useraddress);
            db.SaveChanges();
            return RedirectToAction("Info");
        }
        /// <summary>
        /// 用户的积分历史记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult JiFenHistory(int? id,int userId)
        {
            var jifenlishiList = db.JiFenLiShis.Where(a => a.AccountId == userId).OrderByDescending(a => a.CreateTime);
            PagedList<JiFenLiShi> m = jifenlishiList.ToPagedList(id ?? 1, 10);
            return View(m);
        }
    }
}