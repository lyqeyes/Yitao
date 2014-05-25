using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YiTao.Web.Areas.Watch.Common;
using YiTao.Modules.Bll;
using YiTao.Modules.Bll.Models;
using Webdiyer.WebControls.Mvc;
using YiTao.Web.Areas.Watch.Models;
namespace YiTao.Web.Areas.Watch.Controllers
{
    public class YitaoController : BaseController
    {
        //
        // GET: /Watch/Home/
        [NoLogin]
        public ActionResult Index()
        {
            //读取轮播区信息
            ViewData["lunbolist"] = db.LunBoes.ToList();
            //十件与专题
            ViewData["zhuantilist"] = db.ZhuanTis.ToList();

            //好店
            var Haodian = db.HaoDians.FirstOrDefault();
            if (Haodian != null)
                ViewBag.Haodian = Haodian.Url;
            else
                ViewBag.Haodian = "#";

            return View();
        }
        
        //每日十件 以及 专题的商品信息展示
        [NoLogin]
        public ActionResult Productlist(int id)
        {
            ViewData["lunbolist"] = db.LunBoes.ToList();

            /*var temp = db.ZhuanTis.FirstOrDefault(z => z.Type == id);
            if (temp == null)
            {
                return RedirectToAction("index");
            }
            else
            {
                return View(db.ZhuanTiItems.Where(z => z.ZhuanTiId == temp.ZhuanTiId).ToList());
            }*/
            var temp = db.ZhuanTis.FirstOrDefault(z => z.Type == id);
            if (temp == null)
            {
                return RedirectToAction("index");
            }
            else
            {
                //收藏处理
                //判断是否已登陆
                var logininfo = HttpContext.Request.Cookies["LoginInfo"];
                List<Statistic<ZhuanTiItem>> data;
                if (logininfo == null)
                {
                    //未登录直接全部显示未收藏
                    ViewBag.AllCollectStatus = 0;
                    data = (from z in db.ZhuanTiItems
                            where z.ZhuanTiId == temp.ZhuanTiId
                            select new Statistic<ZhuanTiItem>
                            {
                                data = z,
                                IsCollected = 0
                            }).ToList();
                }
                else
                {
                    string name = logininfo["UserName"];
                    Account account = db.Accounts.FirstOrDefault(a => a.Name == name);
                    ViewBag.AccountId = account.AccountId;
                    ViewBag.AllCollectStatus = 1;
                    data = (from z in db.ZhuanTiItems
                            where z.ZhuanTiId == temp.ZhuanTiId
                            select new Statistic<ZhuanTiItem>
                            {
                                data = z,
                                //处理每一个聚折扣商品的是否被当前用户收藏
                                IsCollected = db.Collections.Where(b => b.Type == (int)EnumCollectionType.ZhuantiItem && b.AccountId == account.AccountId && b.ShangPinId == z.ZhuanTiItemId).Count()
                            }).ToList();
                }
                return View(data.OrderByDescending(a => a.data.ZhuanTiItemId).ToList());
            }
        }

        /// <summary>
        /// 展示专题商品的详细信息
        /// 包括每日十件和三大专题的 item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NoLogin]
        public ActionResult ZhuantiItemDetail(int id)
        {
            /*
            return View(db.ZhuanTiItems.Find(id));
             * */
            //收藏处理
            //判断是否已登陆
            var logininfo = HttpContext.Request.Cookies["LoginInfo"];
            var item = db.ZhuanTiItems.Find(id);
            ViewBag.IsCollected = 0;
            if (logininfo == null)
            {
                //未登录直接全部显示未收藏
                ViewBag.AllCollectStatus = 0;
            }
            else
            {
                string name = logininfo["UserName"];
                Account account = db.Accounts.FirstOrDefault(a => a.Name == name);
                ViewBag.AccountId = account.AccountId;
                ViewBag.AllCollectStatus = 1;
                ViewBag.IsCollected = db.Collections.Where(b => b.Type == (int)EnumCollectionType.ZhuantiItem && b.AccountId == account.AccountId && b.ShangPinId == id).Count();
            }
            return View(item);
        }

        //商品item的展示页面
        [NoLogin]
        public ActionResult ShangpinDetail(int id)
        {
            /*
            return View(db.ShangPins.Find(id));
             */
            //收藏处理
            //判断是否已登陆
            var logininfo = HttpContext.Request.Cookies["LoginInfo"];
            var item = db.ShangPins.Find(id);
            Statistic<ShangPin> data = new Statistic<ShangPin>()
            {
                data = item,
                IsCollected = 0
            };
            if (logininfo == null)
            {
                //未登录直接全部显示未收藏
                ViewBag.AllCollectStatus = 0;
            }
            else
            {
                string name = logininfo["UserName"];
                Account account = db.Accounts.FirstOrDefault(a => a.Name == name);
                ViewBag.AccountId = account.AccountId;
                ViewBag.AllCollectStatus = 1;
                data.IsCollected = db.Collections.Where(b => b.Type == (int)EnumCollectionType.NormalShangpin && b.AccountId == account.AccountId && b.ShangPinId == id).Count();
            }
            return View(data);
             
        }

        //商品item的展示页面
        [NoLogin]
        public ActionResult JuzekouDetail(int id)
        {
            /*return View(db.JuZheKouItems.Find(id));*/
            //收藏处理
            //判断是否已登陆
            var logininfo = HttpContext.Request.Cookies["LoginInfo"];
            ViewBag.IsCollected = 0;
            if (logininfo == null)
            {
                //未登录直接全部显示未收藏
                ViewBag.AllCollectStatus = 0;
            }
            else
            {
                string name = logininfo["UserName"];
                Account account = db.Accounts.FirstOrDefault(a => a.Name == name);
                ViewBag.AccountId = account.AccountId;
                ViewBag.AllCollectStatus = 1;
                ViewBag.IsCollected = db.Collections.Where(b => b.Type == (int)EnumCollectionType.Juzhekou && b.AccountId == account.AccountId && b.ShangPinId == id).Count();
            }
            return View(db.JuZheKouItems.Find(id));
        }

        //大类
        [NoLogin]
        public ActionResult DaleiList()
        {
            //轮播
            ViewData["lunbolist"] = db.LunBoes.ToList();
            return View(db.DaLeis.ToList());
        }

        // 小类列表 参数为大类ID
        [NoLogin]
        public ActionResult XiaoleiList(int id)
        {
            ViewBag.DaleiName = db.DaLeis.FirstOrDefault(d => d.DaLeiId == id).Name;
            return View(db.TowLeis.Where(t=>t.DaLeiId==id).ToList());
        }
        [NoLogin]
        public ActionResult ThreeLei(int id)
        {
            var v = from d in db.ThreeLeis where d.TwoLeiId == id select d;
            ViewData["xiaoleiList"] = (v).ToList();
            List<ShangPin> allShangpin = new List<ShangPin>();
            foreach (var item in v)
            {
                allShangpin.AddRange(db.ShangPins.Where(s => s.XiaoLeiId == item.ThreeLeiId));
            }            
            ViewBag.DaleiName = db.TowLeis.Where(a => a.TwoLeiId == id).FirstOrDefault().Name;
            return View(allShangpin);
        }


        [NoLogin]
        public ActionResult Fuwu()
        {
            List<OtherUrl> urlList = db.OtherUrls.OrderByDescending(a => a.Weight).Take(3).ToList();
            return View(urlList);
        }
        [NoLogin]
        public ActionResult MoreFuwu()
        {
            List<OtherUrl> urlList = db.OtherUrls.OrderByDescending(a => a.Weight).ToList();
            return View(urlList);
        }

        [NoLogin]
        public ActionResult Juzekou()
        {
            /*
            List<JuZheKouItem> list = db.JuZheKouItems.OrderByDescending(a=>a.JuZheKouItemId).Take(3).ToList();
            return View(list);
             * */
            //收藏处理
            //判断是否已登陆
            var logininfo = HttpContext.Request.Cookies["LoginInfo"];
            List<JuzekouStatistic> data;
            if (logininfo == null)
            {
                //未登录直接全部显示未收藏
                ViewBag.AllCollectStatus = 0;
                data = (from a in db.JuZheKouItems
                        orderby a.JuZheKouItemId descending
                        select new JuzekouStatistic
                        {
                            juzhekou = a,
                            IsCollected = 0
                        }).Take(3).ToList();
            }
            else
            {
                string name = logininfo["UserName"];
                Account account = db.Accounts.FirstOrDefault(a => a.Name == name);
                ViewBag.AccountId = account.AccountId;
                ViewBag.AllCollectStatus = 1;
                data = (from a in db.JuZheKouItems
                        orderby a.JuZheKouItemId descending
                        select new JuzekouStatistic
                        {
                            juzhekou = a,
                            //处理每一个聚折扣商品的是否被当前用户收藏
                            IsCollected = db.Collections.Where(b => b.Type == (int)EnumCollectionType.Juzhekou && b.AccountId == account.AccountId && b.ShangPinId == a.JuZheKouItemId).Count()
                        }).Take(3).ToList();
            }

            //List<JuZheKouItem> list = db.JuZheKouItems.OrderByDescending(a=>a.JuZheKouItemId).Take(3).ToList();
            return View(data);
        }
        [NoLogin]
        public ActionResult MoreJuzekou()
        {
            List<JuZheKouItem> list = db.JuZheKouItems.ToList();
            return View(list);
        }

        [NoLogin]
        public ActionResult CommentList(int? id,int Shangpinid, int type)
        {
            List<Comment> comments = (from d in db.Comments where d.ForId == Shangpinid && d.Type == type select d).ToList();
            comments = comments.OrderByDescending(a => a.CreateTime).ToList();
            PagedList<Comment> m = comments.ToPagedList(id ?? 1, 15);
            if (m.Count() == 0)
            {
                m.Add(new Comment() {
                    ForId = Shangpinid,
                    Type = type,
                    UserName = "无名氏",
                    CommentContent= "暂时还无人评论~"
                });
            }
            return View(m);
        }

        [HttpPost]
        public ActionResult CommentList(Comment newCom)
        {
            newCom.CreateTime = DateTime.Now;
            var v = HttpContext.Request.Cookies["LoginInfo"];
            newCom.UserName = HttpUtility.UrlDecode(v["UserName"]);
            db.Comments.Add(newCom);
            db.SaveChanges();
            if (newCom.Type == 1)
            {
                return RedirectToAction("JuzekouDetail", new { id = newCom.ForId });
            }
            else if (newCom.Type == 2)
            {
                return RedirectToAction("ZhuantiItemDetail", new { id = newCom.ForId });
            }
            else
            {
                return RedirectToAction("ShangpinDetail", new { id = newCom.ForId });
            }
        }
	}
}