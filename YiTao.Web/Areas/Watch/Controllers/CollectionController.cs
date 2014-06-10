using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Watch.Common;
using YiTao.Modules.Bll.Models;
using System.Data.Entity;
using YiTao.Modules.Bll;
using YiTao.Web.Areas.Watch.Models;

namespace YiTao.Web.Areas.Watch.Controllers
{
    public class CollectionController : BaseController
    {
        //收藏列表
        public ActionResult Index(int accountId)
        {
            ViewBag.AccountId = accountId;
            List<CollectionStatistic> data = (from c in db.Collections
                                             
                                             join j in db.JuZheKouItems
                                             on c.ShangPinId equals j.JuZheKouItemId
                                             where c.AccountId == accountId &&
                                                 c.Type == (int)EnumCollectionType.Juzhekou
                                             select new CollectionStatistic()
                                             {
                                                 type = c.Type,
                                                 juzhekouItem = j,
                                                 CreateTime = j.CreateTime
                                             }).ToList();
            List<CollectionStatistic> S = (from c in db.Collections
                                           where c.AccountId == accountId &&
                                            c.Type == (int)EnumCollectionType.NormalShangpin
                                           join s in db.ShangPins
                                           on c.ShangPinId equals s.ShangPinId
                                           select new CollectionStatistic()
                                           {
                                               type = c.Type,
                                               shangpin = s,
                                               CreateTime = s.CreateTime
                                           }).ToList();
            List<CollectionStatistic> Z = (from c in db.Collections
                                           where c.AccountId == accountId &&
                                            c.Type == (int)EnumCollectionType.ZhuantiItem
                                           join z in db.ZhuanTiItems
                                           on c.ShangPinId equals z.ZhuanTiItemId
                                           select new CollectionStatistic()
                                           {
                                               type = c.Type,
                                               zhuantiItem = z,
                                               CreateTime = z.CreateTime
                                           }).ToList();
            data.AddRange(S);
            data.AddRange(Z);
            return View(data.OrderByDescending(a => a.CreateTime).ToList());
        }


        //收藏
        [HttpPost]
        public string Collect(int AccountId, int ShangpinId, int type)
        {
            db.Collections.Add(new Collection()
            {
                AccountId = AccountId,
                ShangPinId = ShangpinId,
                CreateTime = DateTime.Now,
                Type = type
            });
            if (type == (int)EnumCollectionType.Juzhekou)
            {
                var data = db.JuZheKouItems.Find(ShangpinId);
                if(data.CollectCount.HasValue)
                {
                    data.CollectCount++;
                }
                else
                {
                    data.CollectCount = 0;
                }
                db.Entry(data).State = EntityState.Modified;
            }
            else if (type == (int)EnumCollectionType.NormalShangpin)
            {
                var data = db.ShangPins.Find(ShangpinId);
                if (data.CollectCount.HasValue)
                {
                    data.CollectCount ++;
                }
                else
                {
                    data.CollectCount = 0;
                }
                db.Entry(data).State = EntityState.Modified;
            }
            else if (type == (int)EnumCollectionType.ZhuantiItem)
            {
                var data = db.ZhuanTiItems.Find(ShangpinId);
                if (data.CollectCount.HasValue)
                {
                    data.CollectCount++;
                }
                else
                {
                    data.CollectCount = 0;
                }
                db.Entry(data).State = EntityState.Modified;
            }
            db.SaveChanges();
            return "ok";
        }

        //取消收藏
        /*
        public string anti_Collect(int CollectionId)
        {
            var c = db.Collections.FirstOrDefault( a => a.CollertionId == CollectionId);
            if (c == null)
            {
                return "error";
            }
            return "ok";
        }*/

        public string anti_Collect(int AccountId, int ShangpinId, int type)
        {
            var c = db.Collections.FirstOrDefault(a => a.Type == type && a.AccountId == AccountId &&  a.ShangPinId == ShangpinId);
            if (c == null)
            {
                return "error";
            }
            if (type == (int)EnumCollectionType.Juzhekou)
            {
                var data = db.JuZheKouItems.Find(ShangpinId);
                data.CollectCount -= 1;
                db.Entry(data).State = EntityState.Modified;
            }
            else if (type == (int)EnumCollectionType.NormalShangpin)
            {
                var data = db.ShangPins.Find(ShangpinId);
                data.CollectCount -= 1;
                db.Entry(data).State = EntityState.Modified;
            }
            else if (type == (int)EnumCollectionType.ZhuantiItem)
            {
                var data = db.ZhuanTiItems.Find(ShangpinId);
                data.CollectCount -= 1;
                db.Entry(data).State = EntityState.Modified;
            }
            var del = db.Collections.Remove(c);
            db.Entry(del).State = EntityState.Deleted;
            db.SaveChanges();
            return "ok";
        }

	}


}