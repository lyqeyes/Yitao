﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using YiTao.Modules.Bll;
using YiTao.Modules.Bll.Models;
using YiTao.Web.Areas.Management.Common;
namespace YiTao.Web.Areas.Management.Controllers
{
    public class MainPageController : BaseController
    {

        #region 轮播
        [HttpGet]
        public ActionResult Lunbo()
        {
            return View(db.LunBoes.ToList());
        }
        [HttpPost]
        public string Lunbo(YiTao.Modules.Bll.Models.LunBo newLunbo)
        {
            try
            {
                var lunbo = db.LunBoes.Find(newLunbo.LunBoId);
                if (lunbo == null)
                {
                    return "100";
                }
                lunbo.ClickUrl = newLunbo.ClickUrl;
                lunbo.ImageUrl = newLunbo.ImageUrl;
                db.Entry(lunbo).State = EntityState.Modified;
                db.SaveChanges();
                return "200";
            }
            catch (Exception)
            {
                return "100";
            }
        }

        #endregion

        #region 商品及每日十件

        public string CreateShangPinAjax(int id, string url = null)
        {
            if (String.IsNullOrEmpty(url))
            {
                return "0";
            }
            else
            {
                var response = HttpWebResponseUtility.CreateGetHttpResponse(url, null, null, null);
                var encode = string.Empty;
                if (response.CharacterSet == "ISO-8859-1")
                    encode = "gb2312";
                else
                    encode = response.CharacterSet;

                Stream stream;

                if (response.ContentEncoding.ToLower() == "gzip")
                {
                    stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    stream = response.GetResponseStream();
                }
                var sr = new StreamReader(stream, Encoding.GetEncoding(encode));

                var html = sr.ReadToEnd();
                //实例化HtmlAgilityPack.HtmlDocument对象
                HtmlDocument doc = new HtmlDocument();
                //载入HTML
                doc.LoadHtml(html);
                string title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                if(!string.IsNullOrEmpty(title))
                {
                    title = title.Replace(title.Split('-').Last(),"");
                    title = title.Substring(0, title.Length - 1);
                }
                HtmlNode navNode = doc.GetElementbyId("J_ImgBooth");
                string src = String.Empty;
                string price = String.Empty;
                if (navNode != null)
                {
                    src = navNode.GetAttributeValue("src", "cuowudelianjie");
                    if (src.Contains("cuowudelianjie"))
                    {
                        src = navNode.GetAttributeValue("data-src", "cuowudelianjie");
                    }
                }
                else
                {
                    src = "cuowudelianjie";
                }
                var nodeprice = doc.GetElementbyId("J_StrPrice");
                if (nodeprice != null)
                {
                    price = nodeprice.LastChild.InnerText;
                }
                else
                {
                    price = "0";
                }
                return JsonConvert.SerializeObject(new { price = price, src = src, title = title, url = url });
            }
        }

        public ActionResult CreateShangPin(int id)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var v = db.ZhuanTis.FirstOrDefault(i => i.Type == id);
            if (v == null)
            {
                return RedirectToAction("Lunbo");
            }
            else
            {
                ViewBag.guishudi = db.Areas.Where(a => a.State == 1).ToList();
                ViewBag.type = v.Type;
                ViewBag.ZhuanTiId = v.ZhuanTiId;
                return View(new ZhuanTiItem() { });
            }

        }
        [HttpPost]
        public ActionResult CreateShangPin(ZhuanTiItem shangpin)
        {
            var v = Request["guishudi"].Split(',');
            if (v.Length > 0)
            {
                if (shangpin.ZhuanTiId ==
                    db.ZhuanTis.FirstOrDefault(z => z.Type ==
                        (int)EnumZhuanTi.ShiJian).ZhuanTiId)
                {
                    if (shangpin.ZhuanTiItemId > 0)
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Attach(shangpin);
                        db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Add(shangpin);
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
                    }

                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion

                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.ShiJian });
                }
                else if (shangpin.ZhuanTiId ==
                    db.ZhuanTis.FirstOrDefault(z => z.Type ==
                        (int)EnumZhuanTi.Zhauti1).ZhuanTiId)
                {
                    if (shangpin.ZhuanTiItemId > 0)
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Attach(shangpin);
                        db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Add(shangpin);
                        db.SaveChanges();
                    }
                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion
                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.Zhauti1 });
                }
                else if (shangpin.ZhuanTiId == db.ZhuanTis.FirstOrDefault(z => z.Type == (int)EnumZhuanTi.Zhauti2).ZhuanTiId)
                {
                    if (shangpin.ZhuanTiItemId > 0)
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Attach(shangpin);
                        db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Add(shangpin);
                        db.SaveChanges();
                    }
                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion
                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.Zhauti2 });
                }
                else
                {
                    if (shangpin.ZhuanTiItemId > 0)
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Attach(shangpin);
                        db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        shangpin.CreateTime = DateTime.Now;
                        db.ZhuanTiItems.Add(shangpin);
                        db.SaveChanges();
                    }
                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion
                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.Zhauti3 });
                }
            }
            else
            {
                return View(shangpin);
            }
        }

        public ActionResult EditShangPin(int ZhuanTiItemId,int id)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var v = db.ZhuanTis.FirstOrDefault(i => i.Type == id);
            if (v == null)
            {
                return RedirectToAction("Lunbo");
            }
            else
            {
                ViewBag.guishudi = db.Areas.Where(a => a.State == 1).ToList();
                ViewBag.type = v.Type;
                ViewBag.ZhuanTiId = v.ZhuanTiId;
                ViewBag.guishudiNow = db.ShangPin2Areas.Where(a => a.ForId == ZhuanTiItemId &&
                        a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem).ToList();
                return View(db.ZhuanTiItems.First(e => e.ZhuanTiItemId == ZhuanTiItemId));
            }
        }

        [HttpPost]
        public ActionResult EditShangPin(ZhuanTiItem shangpin)
        {
            var v = Request["guishudi"].Split(',');
            if (v.Length > 0)
            {
                if (shangpin.ZhuanTiId ==
                    db.ZhuanTis.FirstOrDefault(z => z.Type ==
                        (int)EnumZhuanTi.ShiJian).ZhuanTiId)
                {

                    shangpin.CreateTime = DateTime.Now;
                    db.ZhuanTiItems.Attach(shangpin);
                    db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion

                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.ShiJian });
                }
                else if (shangpin.ZhuanTiId ==
                    db.ZhuanTis.FirstOrDefault(z => z.Type ==
                        (int)EnumZhuanTi.Zhauti1).ZhuanTiId)
                {
                    shangpin.CreateTime = DateTime.Now;
                    db.ZhuanTiItems.Attach(shangpin);
                    db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    
                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion
                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.Zhauti1 });
                }
                else if (shangpin.ZhuanTiId == db.ZhuanTis.FirstOrDefault(z => z.Type == (int)EnumZhuanTi.Zhauti2).ZhuanTiId)
                {
                    shangpin.CreateTime = DateTime.Now;
                    db.ZhuanTiItems.Attach(shangpin);
                    db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                  
                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion
                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.Zhauti2 });
                }
                else
                {
                    shangpin.CreateTime = DateTime.Now;
                    db.ZhuanTiItems.Attach(shangpin);
                    db.Entry<ZhuanTiItem>(shangpin).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    #region 创建地域信息
                    var lists2a = db.ShangPin2Areas.Where(a => a.ForId == shangpin.ZhuanTiItemId &&
                            a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                    if (lists2a.Count() > 0)
                    {
                        db.ShangPin2Areas.RemoveRange(lists2a);
                        db.SaveChanges();
                    }
                    foreach (var item in v)
                    {
                        db.ShangPin2Areas.Add(new ShangPin2Areas
                        {
                            AreaId = int.Parse(item),
                            CreateTime = DateTime.Now,
                            ForId = shangpin.ZhuanTiItemId,
                            Type = (int)EnumAreaType.ZhuantiItem
                        });
                    }
                    db.SaveChanges();

                    #endregion
                    #region 创建索引
                    Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                    {
                        ShangPinId = shangpin.ZhuanTiItemId,
                        Type = Web.Common.EnumShangPinType.ZhuanTi,
                        Name = shangpin.Name
                    };
                    Web.Common.Searcher.Add(SearchShangPin);
                    #endregion
                    return RedirectToAction("ShangPinItem", new { id = (int)EnumZhuanTi.Zhauti3 });
                }
            }
            else
            {
                return View(shangpin);
            }
        }

        public ActionResult DeleteShangPin(int id, int type)
        {
            var temp = db.ZhuanTiItems.FirstOrDefault(z => z.ZhuanTiItemId == id);
            if (temp != null)
            {
                #region 删除索引
                Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                {
                    ShangPinId = temp.ZhuanTiItemId,
                    Type = Web.Common.EnumShangPinType.ZhuanTi,
                    Name = temp.Name
                };
                Web.Common.Searcher.Del(SearchShangPin);
                #endregion

                var lists2a = db.ShangPin2Areas.Where(a => a.ForId == temp.ZhuanTiItemId &&
                       a.Type == (int)YiTao.Modules.Bll.EnumAreaType.ZhuantiItem);
                if (lists2a.Count() > 0)
                {
                    db.ShangPin2Areas.RemoveRange(lists2a);
                    db.SaveChanges();
                }
                db.ZhuanTiItems.Remove(temp);
                db.SaveChanges();

            }
            return RedirectToAction("ShangPinItem", new { id = type });
        }

        public ActionResult ShangPinItem(int type, int? id)
        {
            ViewBag.id = type;
            var temp = db.ZhuanTis.FirstOrDefault(z => z.Type == type);
            if (temp == null)
            {
                return View();
            }
            else
            {
                PagedList<ZhuanTiItem> m = db.ZhuanTiItems.Where(z => z.ZhuanTiId == temp.ZhuanTiId).ToList().ToPagedList(id ?? 1, 5);
                return View(m);
            }
        }
        #endregion

        #region 每日 十件

        [HttpGet]
        public ActionResult MeiRiShiJianCover()
        {
            var temp = db.ZhuanTis.FirstOrDefault(z => z.Type == (int)EnumZhuanTi.ShiJian);
            if (temp == null)
            {
                return View(new ZhuanTi() { });
            }
            else
            {
                return View(temp);
            }

        }

        [HttpPost]
        public ActionResult MeiRiShiJianCover(YiTao.Modules.Bll.Models.ZhuanTi zhuanTi)
        {
            if (zhuanTi.ZhuanTiId > 0)
            {
                zhuanTi.Type = (int)EnumZhuanTi.ShiJian;
                zhuanTi.CreateTime = DateTime.Now;
                db.ZhuanTis.Attach(zhuanTi);
                db.Entry<ZhuanTi>(zhuanTi).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                zhuanTi.Type = (int)EnumZhuanTi.ShiJian;
                zhuanTi.CreateTime = DateTime.Now;
                db.ZhuanTis.Add(zhuanTi);
                db.SaveChanges();
            }
            return RedirectToAction("MeiRiShiJianCover");
        }

        #endregion

        #region 专题1
        [HttpGet]
        public ActionResult ZhuanTi1Cover()
        {
            var temp = db.ZhuanTis.FirstOrDefault(z => z.Type == (int)EnumZhuanTi.Zhauti1);
            if (temp == null)
            {
                return View(new ZhuanTi() { });
            }
            else
            {
                return View(temp);
            }

        }

        [HttpPost]
        public ActionResult ZhuanTi1Cover(YiTao.Modules.Bll.Models.ZhuanTi zhuanTi)
        {
            if (zhuanTi.ZhuanTiId > 0)
            {
                zhuanTi.Type = (int)EnumZhuanTi.Zhauti1;
                zhuanTi.CreateTime = DateTime.Now;
                db.ZhuanTis.Attach(zhuanTi);
                db.Entry<ZhuanTi>(zhuanTi).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                zhuanTi.CreateTime = DateTime.Now;
                zhuanTi.Type = (int)EnumZhuanTi.Zhauti1;
                db.ZhuanTis.Add(zhuanTi);
                db.SaveChanges();
            }
            return RedirectToAction("ZhuanTi1Cover");
        }
        #endregion

        #region 专题2
        [HttpGet]
        public ActionResult ZhuanTi2Cover()
        {
            var temp = db.ZhuanTis.FirstOrDefault(z => z.Type == (int)EnumZhuanTi.Zhauti2);
            if (temp == null)
            {
                return View(new ZhuanTi() { });
            }
            else
            {
                return View(temp);
            }

        }

        [HttpPost]
        public ActionResult ZhuanTi2Cover(YiTao.Modules.Bll.Models.ZhuanTi zhuanTi)
        {
            if (zhuanTi.ZhuanTiId > 0)
            {
                zhuanTi.Type = (int)EnumZhuanTi.Zhauti2;
                zhuanTi.CreateTime = DateTime.Now;
                db.ZhuanTis.Attach(zhuanTi);
                db.Entry<ZhuanTi>(zhuanTi).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                zhuanTi.Type = (int)EnumZhuanTi.Zhauti2;
                zhuanTi.CreateTime = DateTime.Now;
                db.ZhuanTis.Add(zhuanTi);
                db.SaveChanges();
            }
            return RedirectToAction("ZhuanTi2Cover");
        }
        #endregion

        #region 专题3
        [HttpGet]
        public ActionResult ZhuanTi3Cover()
        {
            var temp = db.ZhuanTis.FirstOrDefault(z => z.Type == (int)EnumZhuanTi.Zhauti3);
            if (temp == null)
            {
                return View(new ZhuanTi() { });
            }
            else
            {
                return View(temp);
            }

        }

        [HttpPost]
        public ActionResult ZhuanTi3Cover(YiTao.Modules.Bll.Models.ZhuanTi zhuanTi)
        {
            if (zhuanTi.ZhuanTiId > 0)
            {
                zhuanTi.Type = (int)EnumZhuanTi.Zhauti3;
                zhuanTi.CreateTime = DateTime.Now;
                db.ZhuanTis.Attach(zhuanTi);
                db.Entry<ZhuanTi>(zhuanTi).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                zhuanTi.Type = (int)EnumZhuanTi.Zhauti3;
                zhuanTi.CreateTime = DateTime.Now;
                db.ZhuanTis.Add(zhuanTi);
                db.SaveChanges();
            }
            return RedirectToAction("ZhuanTi3Cover");
        }
        #endregion

        #region 聚折扣
        public ActionResult JuZheKouItem(int? id)
        {
            PagedList<JuZheKouItem> m = db.JuZheKouItems.ToList().ToPagedList(id ?? 1, 5);
            return View(m);
        }

        public ActionResult CreateJuZheKouItem()
        {
            ViewBag.guishudi = db.Areas.Where(a => a.State == 1).ToList();
            return View(new JuZheKouItem() { });
        }
        [HttpPost]
        public ActionResult CreateJuZheKouItem(JuZheKouItem jzk)
        {
            var v = Request["guishudi"].Split(',');
            if (v.Length > 0)
            {
                if (jzk.JuZheKouItemId > 0)
                {
                    db.JuZheKouItems.Attach(jzk);
                    db.Entry<JuZheKouItem>(jzk).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    jzk.CreateTime = DateTime.Now;
                    db.JuZheKouItems.Add(jzk);
                    db.SaveChanges();
                }

                #region 创建地域信息
                var lists2a = db.ShangPin2Areas.Where(a => a.ForId == jzk.JuZheKouItemId &&
                        a.Type == (int)YiTao.Modules.Bll.EnumAreaType.Juzhekou);
                if (lists2a.Count() > 0)
                {
                    db.ShangPin2Areas.RemoveRange(lists2a);
                    db.SaveChanges();
                }
                foreach (var item in v)
                {
                    db.ShangPin2Areas.Add(new ShangPin2Areas
                    {
                        AreaId = int.Parse(item),
                        CreateTime = DateTime.Now,
                        ForId = jzk.JuZheKouItemId,
                        Type = (int)EnumAreaType.Juzhekou
                    });
                }
                db.SaveChanges();

                #endregion

                #region 创建索引
                Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                {
                    ShangPinId = jzk.JuZheKouItemId,
                    Type = Web.Common.EnumShangPinType.JuZheKou,
                    Name = jzk.Name
                };
                Web.Common.Searcher.Add(SearchShangPin);
                #endregion
                return RedirectToAction("JuZheKouItem");
            }
            else
            {
                return View(jzk);
            }
        }

        public ActionResult EditJuZheKouItem(int JuZheKouItemId)
        {
            ViewBag.guishudi = db.Areas.Where(a => a.State == 1).ToList();
            ViewBag.guishudiNow = db.ShangPin2Areas.Where(a => a.ForId == JuZheKouItemId &&
                        a.Type == (int)YiTao.Modules.Bll.EnumAreaType.Juzhekou).ToList();
            return View(db.JuZheKouItems.First(e=>e.JuZheKouItemId == JuZheKouItemId));
        }
        [HttpPost]
        public ActionResult EditJuZheKouItem(JuZheKouItem jzk)
        {
            var v = Request["guishudi"].Split(',');
            if (v.Length > 0)
            {
                //聚折扣基本信息修改保存
                db.JuZheKouItems.Attach(jzk);
                db.Entry<JuZheKouItem>(jzk).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                #region 创建地域信息
                var lists2a = db.ShangPin2Areas.Where(a => a.ForId == jzk.JuZheKouItemId &&
                        a.Type == (int)YiTao.Modules.Bll.EnumAreaType.Juzhekou);
                if (lists2a.Count() > 0)
                {
                    db.ShangPin2Areas.RemoveRange(lists2a);
                    db.SaveChanges();
                }
                foreach (var item in v)
                {
                    db.ShangPin2Areas.Add(new ShangPin2Areas
                    {
                        AreaId = int.Parse(item),
                        CreateTime = DateTime.Now,
                        ForId = jzk.JuZheKouItemId,
                        Type = (int)EnumAreaType.Juzhekou
                    });
                }
                db.SaveChanges();

                #endregion

                #region 修改索引（与创建通用）
                Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                {
                    ShangPinId = jzk.JuZheKouItemId,
                    Type = Web.Common.EnumShangPinType.JuZheKou,
                    Name = jzk.Name
                };
                Web.Common.Searcher.Add(SearchShangPin);
                #endregion
                return RedirectToAction("JuZheKouItem");
            }
            else
            {
                return View(jzk);
            }
        }

        public ActionResult DeleteJuZheKo(int id)
        {
            var temp = db.JuZheKouItems.FirstOrDefault(j => j.JuZheKouItemId == id);
            if (temp != null)
            {
                var lists2a = db.ShangPin2Areas.Where(a => a.ForId == temp.JuZheKouItemId &&
                       a.Type == (int)YiTao.Modules.Bll.EnumAreaType.Juzhekou);
                if (lists2a.Count() > 0)
                {
                    db.ShangPin2Areas.RemoveRange(lists2a);
                    db.SaveChanges();
                }

                #region 删除索引
                Web.Common.ShangPin SearchShangPin = new Web.Common.ShangPin()
                {
                    ShangPinId = temp.JuZheKouItemId,
                    Type = Web.Common.EnumShangPinType.JuZheKou,
                    Name = temp.Name
                };
                Web.Common.Searcher.Del(SearchShangPin);
                #endregion
                db.JuZheKouItems.Remove(temp);
                db.SaveChanges();
            }
            return RedirectToAction("JuZheKouItem");
        }

        public string CreateJuZheKouAjax(string url = null)
        {
            if (String.IsNullOrEmpty(url))
            {
                return "0";
            }
            else
            {
                var response = HttpWebResponseUtility.CreateGetHttpResponse(url, null, null, null);
                var encode = string.Empty;
                if (response.CharacterSet == "ISO-8859-1")
                    encode = "gb2312";
                else
                    encode = response.CharacterSet;

                Stream stream;

                if (response.ContentEncoding.ToLower() == "gzip")
                {
                    stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    stream = response.GetResponseStream();
                }
                var sr = new StreamReader(stream, Encoding.GetEncoding(encode));

                var html = sr.ReadToEnd();
                //实例化HtmlAgilityPack.HtmlDocument对象
                HtmlDocument doc = new HtmlDocument();
                //载入HTML
                doc.LoadHtml(html);
                string title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                if (!string.IsNullOrEmpty(title))
                {
                    title = title.Replace(title.Split('-').Last(), "");
                    title = title.Substring(0, title.Length - 1);
                }
                HtmlNode navNode = doc.GetElementbyId("J_ImgBooth");
                string src = String.Empty;
                string price = String.Empty;
                if (navNode != null)
                {
                    src = navNode.GetAttributeValue("src", "cuowudelianjie");
                    if (src.Contains("cuowudelianjie"))
                    {
                        src = navNode.GetAttributeValue("data-src", "cuowudelianjie");
                    }
                }
                else
                {
                    src = "cuowudelianjie";
                }
                var nodeprice = doc.GetElementbyId("J_StrPrice");
                if (nodeprice != null)
                {
                    price = nodeprice.LastChild.InnerText;
                }
                else
                {
                    price = "0";
                }
                return JsonConvert.SerializeObject(new { price = price, src = src, title = title, url = url });
            }
        }

        #endregion

        #region 好店管理
        [HttpGet]
        public ActionResult HaoDian()
        {
            return View(db.HaoDians.ToList()[0]);
        }

        [HttpPost]
        public string HaoDian(YiTao.Modules.Bll.Models.HaoDian newHaoDian)
        {
            try
            {
                var haodian = db.HaoDians.Find(newHaoDian.HaoDianId);
                if (haodian == null)
                {
                    return "100";
                }
                haodian.Url = newHaoDian.Url;
                haodian.Description = newHaoDian.Description;
                haodian.Name = newHaoDian.Name;
                haodian.ImageUrl = newHaoDian.ImageUrl;
                db.Entry(haodian).State = EntityState.Modified;
                db.SaveChanges();
                return "200";
            }
            catch (Exception)
            {
                return "100";
            }
        }

        //[HttpPost]
        //public ActionResult HaoDian(HaoDian hd)
        //{
        //    if (hd.HaoDianId > 0)
        //    {
        //        db.HaoDians.Attach(hd);
        //        db.Entry<HaoDian>(hd).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    else
        //    {
        //        hd.CreateTime = DateTime.Now;
        //        db.HaoDians.Add(hd);
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("HaoDian");
        //}
        #endregion

        #region 其他连接

        public ActionResult OtherUrl(int? id)
        {
            PagedList<OtherUrl> m = db.OtherUrls.ToList().ToPagedList(id ?? 1, 5);
            return View(m);
        }
        public ActionResult OtherUrlCreate(OtherUrl otherUrl)
        {
            otherUrl.CreateTime = DateTime.Now;
            db.OtherUrls.Add(otherUrl);
            db.SaveChanges();
            return RedirectToAction("OtherUrl");
        }
        public ActionResult OtherUrlDel(OtherUrl newOtherUrl)
        {
            var otherUrl = db.OtherUrls.Find(newOtherUrl.OtherUrlId);
            if (otherUrl == null)
                return RedirectToAction("OtherUrl");
            if (otherUrl != null)
            {
                db.OtherUrls.Remove(otherUrl);
                db.SaveChanges();
            }
            return RedirectToAction("OtherUrl");
        }

        #endregion
    }
}