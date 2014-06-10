using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Management.Common;
using Webdiyer.WebControls.Mvc;

namespace YiTao.Web.Areas.Management.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult CommonManage(int id= 0)
        {
            return View(db.Accounts.OrderByDescending(a => a.AccountId).ToPagedList(id, 20));
        }

        [HttpGet]
        public ActionResult JiFenManage(int? id)
        {
            return View(db.Accounts.OrderByDescending(a=>a.AccountId).ToPagedList(id ?? 1, 20));
        }

        //注册新会员
        [HttpPost]
        public string Register(YiTao.Modules.Bll.Models.Account account)
        {
            if (db.Accounts.Count(e => e.Name == account.Name) != 0)
            {
                return "100";
            }
            else
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return "200";
            }
        }
        //编辑会员基本资料
        public string CommonEdit(YiTao.Modules.Bll.Models.Account newAccount)
        {
            var account = db.Accounts.FirstOrDefault(e => e.Name == newAccount.Name);
            if (account == null)
            {
                return "100";
            }
            else
            {
                account.Password = newAccount.Password;
                account.Email = newAccount.Email;
                account.Phone = newAccount.Phone;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return "200";
            }
        }

        [HttpPost]
        public string Add(string name, int add)
        {
            var account = db.Accounts.FirstOrDefault(e => e.Name == name);
            if (account == null)
            {
                return "100";
            }
            else
            {
                var jfls = new YiTao.Modules.Bll.Models.JiFenLiShi();
                jfls.AccountId = account.AccountId;
                jfls.Type = 1;
                jfls.JiFen = add;
                jfls.AccountName = name;
                jfls.Description = "人为添加积分";
                jfls.CreateTime = DateTime.Now;
                jfls.WhetherDealed = 0;
                db.JiFenLiShis.Add(jfls);
                account.JiFen += add;
                db.Accounts.Attach(account);
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return "200";
            }
        }

        [HttpPost]
        public string Reduce(string name, int reduce)
        {
            var account = db.Accounts.FirstOrDefault(e => e.Name == name);
            if (account == null)
            {
                return "100";
            }
            else
            {
                var jfls = new YiTao.Modules.Bll.Models.JiFenLiShi();
                jfls.AccountId = account.AccountId;
                jfls.Type = 1;
                jfls.JiFen = reduce;
                jfls.Description = "人为减少积分";
                jfls.AccountName = name;
                jfls.CreateTime = DateTime.Now;
                jfls.WhetherDealed = 0;
                db.JiFenLiShis.Add(jfls);
                account.JiFen -= reduce;
                if (account.JiFen < 0)
                    account.JiFen = 0;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return "200";
            }
        }

        [HttpGet]
        public ActionResult JiFenLiShi(int? id)
        {
            return View(db.JiFenLiShis.OrderByDescending(a=>a.JiFenLiShiId).ToPagedList(id ?? 1, 20));
        }
	}    
}