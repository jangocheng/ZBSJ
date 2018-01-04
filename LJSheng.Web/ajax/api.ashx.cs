using System;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LJSheng.Data;
using EntityFramework.Extensions;
using LJSheng.Common;
using LJSheng.WX;
using System.Collections.Generic;
using System.IO;

namespace LJSheng.Web.ajax
{
    /// <summary>
    /// api 的摘要说明
    /// </summary>
    public class api : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            object returnstr = "当前设备被禁止访问";
            switch (context.Request.QueryString["ff"])
            {
                case "oauth":
                    returnstr = oauth(context);
                    break;
                case "getIndex":
                    returnstr = getIndex();
                    break;
                case "getVideoList":
                    returnstr = getVideoList(context);
                    break;
                case "getVideo":
                    returnstr = getVideo(context);
                    break;
                case "getCommentList":
                    returnstr = getCommentList(context);
                    break;
                case "addComment":
                    returnstr = addComment(context);
                    break;
                case "getNewsList":
                    returnstr = getNewsList(context);
                    break;
                case "getNews":
                    returnstr = getNews(context);
                    break;
                case "getOrderList":
                    returnstr = getOrderList(context);
                    break;
                case "getShopList":
                    returnstr = getShopList(context);
                    break;
                case "getShop":
                    returnstr = getShop(context);
                    break;
                case "MVideo":
                    returnstr = MVideo(context);
                    break;
                case "videoNumber":
                    returnstr = videoNumber(context);
                    break;
                case "order":
                    returnstr = order(context);
                    break;
                case "payOrder":
                    returnstr = payOrder(context);
                    break;
                case "getODList":
                    returnstr = getODList(context);
                    break;
                case "getVRList":
                    returnstr = getVRList(context);
                    break;
                case "kd":
                    returnstr = kd(context);
                    break;
                case "getAddr":
                    returnstr = getAddr(context);
                    break;
                default:
                    break;
            }
            context.Response.Write(JsonConvert.SerializeObject(returnstr));
            context.Response.End();
        }

        #region 商城相关

        /// <summary> 
        /// 获取视频详情
        /// </summary> 
        /// <param name="gid">视频的gid</param> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getShop(HttpContext context)
        {
            Guid gid = Guid.Parse(context.Request.Form["searchKeyword"]);
            using (EFDB db = new EFDB())
            {
                var list = db.product.Where(l => l.gid == gid).Select(l => new
                {
                    l.gid,
                    l.name,
                    l.picture,
                    l.price,
                    l.graphic_details,
                    l.subtitle,
                    l.number,
                    l.add_time,
                    l.extend1,
                    l.extend2,
                    l.extend3,
                    l.extend4
                }).FirstOrDefault();
                return new AjaxResult(new { url = Help.ApiUrl + Help.Product, list });
            }
        }

        /// <summary> 
        /// 获取商城列表
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getShopList(HttpContext context)
        {
            int pageSize = int.Parse(context.Request.Form["pageSize"]);
            int pageIndex = int.Parse(context.Request.Form["pageIndex"]);
            string searchKeyword = context.Request.Form["searchKeyword"];
            using (EFDB db = new EFDB())
            {
                //var list = db.product.Where(l => l.show == 1).GroupJoin(db.classify,
                //    x => x.classify_gid,
                //    y => y.gid,
                //    (x, y) => new
                //    {
                //        x.gid,
                //        x.name,
                //        x.picture,
                //        x.sort,
                //        x.add_time,
                //        y.FirstOrDefault().ltype,
                //        cname = y.FirstOrDefault().name
                //    }).Where(l => l.ltype == 1).AsQueryable();
                //if (!string.IsNullOrEmpty(searchKeyword))
                //{
                //    list = list.Where(l => l.name.Contains(searchKeyword));
                //}
                var list = db.classify.Where(l => l.ltype != 2).Select(l => new
                {
                    l.name,
                    l.sort,
                    list = db.product.Where(p => p.show != 2 && p.classify_gid == l.gid && p.name.Contains(searchKeyword)).Select(p => new { p.gid, p.name, p.picture, p.price, p.sort }).OrderByDescending(p => p.sort)
                }).OrderBy(l => l.sort).ToList();

                return new AjaxResult(
                    new
                    {
                        url = Help.ApiUrl + Help.Product,
                        list
                    });
            }
        }

        #endregion

        #region 订单相关

        /// <summary> 
        /// 获取个人中心的订单
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getOrderList(HttpContext context)
        {
            int pageSize = int.Parse(context.Request.Form["pageSize"]);
            int pageIndex = int.Parse(context.Request.Form["pageIndex"]);
            Guid member_gid = Guid.Parse(context.Request.Form["member_gid"]);
            //0=全部 1=真柏 2=视频 3=课程
            int ltype = int.Parse(context.Request.Form["searchKeyword"]);
            using (EFDB db = new EFDB())
            {
                var list = db.lorder.Where(l => l.pay_status == 1&&l.member_gid==member_gid).GroupJoin(db.product,
                    x => x.product_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.gid,
                        x.order_no,
                        x.add_time,
                        x.pay_status,
                        x.price,
                        x.express_status,
                        x.express,
                        x.express_number,
                        x.consignee,
                        x.contact_number,
                        x.address,
                        x.product_gid,
                        y.FirstOrDefault().name,
                        y.FirstOrDefault().extend1,
                        y.FirstOrDefault().extend3,
                        y.FirstOrDefault().classify_gid
                    }).GroupJoin(db.classify,
                    x => x.classify_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.gid,
                        x.order_no,
                        x.add_time,
                        x.pay_status,
                        x.price,
                        x.express_status,
                        x.express,
                        x.express_number,
                        x.consignee,
                        x.contact_number,
                        x.address,
                        x.name,
                        x.classify_gid,
                        x.product_gid,
                        ltype = Nullable<int>.Equals(y.FirstOrDefault().ltype, null)?1: y.FirstOrDefault().ltype
                    });
                if (ltype != 0)
                {
                    list = list.Where(l => l.ltype == ltype);
                }
                return new AjaxResult(
                    new
                    {
                        list = list.OrderByDescending(l => l.add_time).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList()
                    });
            }
        }

        /// <summary> 
        /// 获取商品列表
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getODList(HttpContext context)
        {
            Guid order_gid = Guid.Parse(context.Request.Form["order_gid"]);
            using (EFDB db = new EFDB())
            {
                var list = db.order_details.Where(l => l.order_gid == order_gid).GroupJoin(db.product,
                    x => x.product_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.price,
                        x.number,
                        x.add_time,
                        y.FirstOrDefault().name
                    }).AsQueryable();
                return new AjaxResult(
                    new
                    {
                        list = list.OrderByDescending(l => l.add_time).ToList()
                    });
            }
        }

        /// <summary> 
        /// 下订单返回支付参数
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object order(HttpContext context)
        {
            try
            {
                string json = "";
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    json = HttpUtility.UrlDecode(sr.ReadLine()); //Server.UrlDecode
                }
                //解析参数
                JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
                //商品列表
                string product = paramJson["product"].ToString();
                //购买类型[1=真柏 2=视频 3=课程]
                int type = int.Parse(paramJson["type"].ToString());
                //会员的gid
                Guid member_gid = Guid.Parse(paramJson["member_gid"].ToString());
                //会员的登录标识
                string login_identifier = paramJson["login_identifier"].ToString();
                //收货地址
                string contact_number = paramJson["contact_number"].ToString();
                string consignee = paramJson["consignee"].ToString();
                string address = paramJson["address"].ToString();
                string remarks = paramJson["remarks"].ToString();
                using (EFDB db = new EFDB())
                {
                    var member = db.member.Where(l => l.gid == member_gid).FirstOrDefault();
                    if (member.login_identifier == login_identifier)
                    {
                        string order_no = RandStr.CreateOrderNO();
                        string _package = Helper.wxpay(order_no, product, member_gid, member.openid, type, remarks, address, consignee, contact_number);
                        string APPID = Help.appid;
                        string PARTNER_KEY = Help.api_key;
                        SortedDictionary<string, string> pay_dic = new SortedDictionary<string, string>();
                        string wx_timeStamp = WXPay.getTimestamp();
                        string wx_nonceStr = WXPay.getNoncestr();
                        pay_dic.Add("appId", APPID);
                        pay_dic.Add("timeStamp", wx_timeStamp);
                        pay_dic.Add("nonceStr", wx_nonceStr);
                        pay_dic.Add("package", _package);
                        pay_dic.Add("signType", "MD5");
                        string paySign = WXPay.BuildRequest(pay_dic, PARTNER_KEY);
                        return new AjaxResult(new
                        {
                            timeStamp = wx_timeStamp,
                            nonceStr = wx_nonceStr,
                            signType = "MD5",
                            paySign = paySign,
                            package = _package,
                            order_no = order_no
                        });
                    }
                    else
                    {
                        return new AjaxResult(300, "请重新登录!");
                    }
                }
            }
            catch (Exception err)
            {
                return new AjaxResult(300, new { Message = err.Message, Source = err.Source, StackTrace = err.StackTrace });
            }
        }

        /// <summary> 
        /// 支付成功更新订单
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object payOrder(HttpContext context)
        {
            try
            {
                string json = "";
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    json = HttpUtility.UrlDecode(sr.ReadLine()); //Server.UrlDecode
                }
                //解析参数
                JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
                if (Helper.payOrder(paramJson["order_no"].ToString()))
                {
                    return new AjaxResult("支付成功!");
                }
                else
                {
                    return new AjaxResult(300, "购买失败,如果你已被扣款请查看你的微信付款的[商户单号]告知客服处理!");
                }
            }
            catch (Exception err)
            {
                return new AjaxResult(300, new { Message = err.Message, Source = err.Source, StackTrace = err.StackTrace });
            }
        }

        /// <summary> 
        /// 获取默认收货地址
        /// </summary> 
        /// <param name="member_gid">会员的gid</param> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getAddr(HttpContext context)
        {
            Guid member_gid = Guid.Parse(context.Request.Form["member_gid"]);
            using (EFDB db = new EFDB())
            {
                var list = db.member.Where(l => l.gid == member_gid).Select(l => new
                {
                    l.address,
                    l.contact_number,
                    l.real_name
                }).FirstOrDefault();
                return new AjaxResult(list);
            }
        }
        #endregion

        #region 视频付费播放相关

        /// <summary> 
        /// 检查用户是否可以观看视频
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object MVideo(HttpContext context)
        {
            try
            {
                //会员的gid
                Guid member_gid = Guid.Parse(context.Request.Form["member_gid"]);
                //会员的登录标识
                string login_identifier = context.Request.Form["login_identifier"];
                //当前产品的gid
                Guid product_gid = Guid.Parse(context.Request.Form["searchKeyword"]);
                using (EFDB db = new EFDB())
                {
                    //返回类型 type[1=vip 2=个数 3=次数]
                    var member = db.member.Where(l => l.gid == member_gid).FirstOrDefault();
                    if (member.login_identifier == login_identifier)
                    {
                        //如果是VIP直接观看
                        if (member.number >= 9999)
                        {
                            return new AjaxResult(new { type = 1, member.number });
                        }
                        else
                        {
                            //检查用户是否还有当前视频观看次数
                            var b = db.video_record.Where(l => l.member_gid == member_gid && l.product_gid == product_gid).FirstOrDefault();
                            if (b != null)
                            {
                                if (b.number > 0)
                                {
                                    return new AjaxResult(new { type = 3, b.number });
                                }
                            }
                            else
                            {
                                //当前视频没有观看次数判断是否有可看数量
                                if (member.number > 0)
                                {
                                    return new AjaxResult(new { type = 2, member.number });
                                }
                            }
                        }
                        //什么都没有
                        return new AjaxResult(new { type = 0, number = 0 });
                    }
                    else
                    {
                        return new AjaxResult(300, "请重新登录!");
                    }
                }
            }
            catch (Exception err)
            {
                return new AjaxResult(300,new { Message = err.Message, Source = err.Source, StackTrace = err.StackTrace });
            }
        }

        /// <summary> 
        /// 扣除播放数量
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object videoNumber(HttpContext context)
        {
            try
            {
                //会员的gid
                Guid member_gid = Guid.Parse(context.Request.Form["member_gid"]);
                //会员的登录标识
                string login_identifier = context.Request.Form["login_identifier"];
                //当前产品的gid
                Guid product_gid = Guid.Parse(context.Request.Form["searchKeyword"]);
                //当前扣除的类型 type[1=vip 2=个数 3=次数]
                int type = int.Parse(context.Request.Form["type"]);
                using (EFDB db = new EFDB())
                {
                    var member = db.member.Where(l => l.gid == member_gid).FirstOrDefault();
                    if (member.login_identifier == login_identifier)
                    {
                        if (type == 3)
                        {
                            var b = db.video_record.Where(l => l.member_gid == member_gid && l.product_gid == product_gid).FirstOrDefault();
                            if (b != null)
                            {
                                b.number = b.number - 1;
                                b.video_time = DateTime.Now;
                                if (db.SaveChanges() == 1)
                                {
                                    if (b.number <= 0)
                                    {
                                        db.video_record.Where(l => l.member_gid == member_gid && l.product_gid == product_gid).Delete();
                                    }
                                    return new AjaxResult("ok");
                                }
                                else
                                {
                                    LogManager.WriteLog("扣除观看次数失败", "product_gid=" + product_gid.ToString() + ",member_gid=" + member_gid.ToString());
                                }
                            }
                            else
                            {
                                LogManager.WriteLog("用户没有观看记录", "product_gid=" + product_gid.ToString() + ",member_gid=" + member_gid.ToString());
                            }
                        }
                        else if (type == 2)
                        {
                            if (db.member.Where(l => l.gid == member_gid).Update(l => new member { number = l.number - 1 }) > 0)
                            {
                                var vr = new video_record();
                                vr.gid = Guid.NewGuid();
                                vr.add_time = DateTime.Now;
                                vr.product_gid = product_gid;
                                vr.member_gid = member_gid;
                                vr.video_time = DateTime.Now;
                                vr.number = (int)db.product.Where(l => l.gid == product_gid).FirstOrDefault().extend4 - 1;
                                db.video_record.Add(vr);
                                if (db.SaveChanges() != 1)
                                {
                                    LogManager.WriteLog("扣除个数成功增加记录失败", "product_gid=" + product_gid.ToString() + ",member_gid=" + member_gid.ToString());
                                }
                                return new AjaxResult("ok");
                            }
                        }
                        else
                        {
                            return new AjaxResult(300, "未知数据=" + type.ToString());
                        }

                        return new AjaxResult(300, "扣除操作失败");
                    }
                    else
                    {
                        return new AjaxResult(300, "请重新登录!");
                    }
                }
            }
            catch (Exception err)
            {
                return new AjaxResult(300,new { Message = err.Message, Source = err.Source, StackTrace = err.StackTrace });
            }
        }
        #endregion

        #region 资讯相关

        /// <summary> 
        /// 获取资讯详情
        /// </summary> 
        /// <param name="gid">视频的gid</param> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getNews(HttpContext context)
        {
            Guid gid = Guid.Parse(context.Request.Form["searchKeyword"]);
            using (EFDB db = new EFDB())
            {
                var list = db.news.Where(l => l.gid == gid).Select(l => new
                {
                    l.gid,
                    name = l.title,
                    l.picture,
                    l.graphic_details,
                    l.subtitle,
                    l.number,
                    l.author,
                    l.add_time
                }).FirstOrDefault();
                return new AjaxResult(new { url = Help.ApiUrl + Help.News, list });
            }
        }

        /// <summary> 
        /// 获取资讯列表
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getNewsList(HttpContext context)
        {
            int pageSize = int.Parse(context.Request.Form["pageSize"]);
            int pageIndex = int.Parse(context.Request.Form["pageIndex"]);
            string searchKeyword = context.Request.Form["searchKeyword"];
            using (EFDB db = new EFDB())
            {
                var list = db.news.Where(l => l.show == 1).AsQueryable();
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    list = list.Where(l => l.title.Contains(searchKeyword));
                }
                return new AjaxResult(
                    new
                    {
                        url = Help.ApiUrl + Help.News,
                        list = list.OrderByDescending(l => l.add_time).Select(l => new
                        {
                            l.gid,
                            l.title,
                            l.picture,
                            l.subtitle,
                            l.add_time
                        }).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList()
                    });
            }
        }

        #endregion

        #region 视频相关

        /// <summary> 
        /// 获取视频详情
        /// </summary> 
        /// <param name="gid">视频的gid</param> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getVideo(HttpContext context)
        {
            Guid gid = Guid.Parse(context.Request.Form["searchKeyword"]);
            using (EFDB db = new EFDB())
            {
                var list = db.product.Where(l => l.gid == gid).Select(l => new
                {
                    l.gid,
                    l.name,
                    l.picture,
                    l.graphic_details,
                    l.subtitle,
                    l.number,
                    l.price,
                    tprice = db.product.Where(p => p.sort == 10000 && p.show == 2).Select(p => new { p.gid, p.price, p.subtitle }).FirstOrDefault(),
                    vip = db.product.Where(p => p.sort == 20000 && p.show == 2).Select(p => new { p.gid, p.price, p.subtitle }).FirstOrDefault(),
                    l.add_time,
                    l.extend1,
                    l.extend2,
                    l.extend3,
                    l.extend4
                }).FirstOrDefault();
                return new AjaxResult(new { url = Help.ApiUrl + Help.Product, list });
            }
        }

        /// <summary> 
        /// 获取视频列表
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getVideoList(HttpContext context)
        {
            int pageSize = int.Parse(context.Request.Form["pageSize"]);
            int pageIndex = int.Parse(context.Request.Form["pageIndex"]);
            string searchKeyword = context.Request.Form["searchKeyword"];
            using (EFDB db = new EFDB())
            {
                var list = db.product.Where(l => l.show != 2).GroupJoin(db.classify,
                    x => x.classify_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.gid,
                        x.name,
                        x.picture,
                        x.add_time,
                        y.FirstOrDefault().ltype
                    }).Where(l => l.ltype == 2).AsQueryable();
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    list = list.Where(l => l.name.Contains(searchKeyword));
                }
                return new AjaxResult(new { url = Help.ApiUrl + Help.Product, list = list.OrderByDescending(l => l.add_time).Select(l => new { l.gid, l.name, l.picture }).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList() });
            }
        }

        /// <summary> 
        /// 获取商品列表
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getVRList(HttpContext context)
        {
            int pageSize = int.Parse(context.Request.Form["pageSize"]);
            int pageIndex = int.Parse(context.Request.Form["pageIndex"]);
            Guid member_gid = Guid.Parse(context.Request.Form["member_gid"]);
            using (EFDB db = new EFDB())
            {
                var list = db.video_record.Where(l => l.member_gid == member_gid).GroupJoin(db.product,
                    x => x.product_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.product_gid,
                        x.number,
                        x.add_time,
                        y.FirstOrDefault().name
                    }).AsQueryable();
                return new AjaxResult(
                    new
                    {
                        list = list.OrderByDescending(l => l.add_time).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList()
                    });
            }
        }

        #endregion

        #region 获取首页数据
        /// <summary> 
        /// 获取首页数据
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getIndex()
        {
            using (EFDB db = new EFDB())
            {
                //宣传片
                var xcp = db.product.Where(l => l.sort == 9999).Select(l => new { l.gid, l.subtitle, l.picture }).FirstOrDefault();
                //首页推荐视频
                var video = db.product.Where(l => l.show == 3).GroupJoin(db.classify,
                    x => x.classify_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.gid,
                        x.name,
                        x.picture,
                        x.sort,
                        y.FirstOrDefault().ltype
                    }).Where(l => l.ltype == 2).OrderByDescending(l => l.sort).Select(l => new { l.gid, l.name, l.picture }).ToList();
                return new AjaxResult(new { url = Help.ApiUrl + Help.Product, xcp, video });
            }
        }
        #endregion

        #region 评论相关
        /// <summary> 
        /// 根据产品gid获取评论
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object getCommentList(HttpContext context)
        {
            int pageSize = int.Parse(context.Request.Form["pageSize"]);
            int pageIndex = int.Parse(context.Request.Form["pageIndex"]);
            Guid gid = Guid.Parse(context.Request.Form["searchKeyword"]);
            using (EFDB db = new EFDB())
            {
                var list = db.comment.Where(l => l.product_gid == gid).GroupJoin(db.member,
                    x => x.member_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.content,
                        x.add_time,
                        x.reply,
                        x.reply_time,
                        y.FirstOrDefault().nickname,
                        mp = y.FirstOrDefault().picture
                    }).OrderByDescending(l => l.add_time).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                return new AjaxResult(new { list });
            }
        }

        /// <summary> 
        /// 添加评论
        /// </summary> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object addComment(HttpContext context)
        {
            Guid product_gid = Guid.Parse(context.Request.Form["product_gid"]);
            Guid member_gid = Guid.Parse(context.Request.Form["member_gid"]);
            string content = context.Request.Form["content"];
            using (EFDB db = new EFDB())
            {
                comment b = new comment();
                b.gid = Guid.NewGuid();
                b.add_time = DateTime.Now;
                b.show = 1;
                b.sort = 0;
                b.content = content;
                b.product_gid = product_gid;
                b.member_gid = member_gid;
                db.comment.Add(b);
                if (db.SaveChanges() == 1)
                {
                    return new AjaxResult("成功");
                }
                else
                {
                    return new AjaxResult(300, "失败");
                }
            }
        }
        #endregion

        #region 小程序相关
        /// <summary> 
        /// 根据code获取小程序登录用户的信息
        /// </summary> 
        /// <param name="逻辑说明"></param> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object oauth(HttpContext context)
        {
            Guid gid = Guid.NewGuid();
            Boolean TF = false;
            string login_identifier = "";
            string code = context.Request.Form["code"];
            if (!String.IsNullOrEmpty(code))
            {
                try
                {
                    var url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", Help.appid, Help.appsecret, code);
                    string json = Helper.Post(url, "");
                    LogManager.WriteLog("json", json);
                    JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
                    string openid = paramJson["openid"].ToString();
                    if (!string.IsNullOrEmpty(openid))
                    {
                        using (EFDB db = new EFDB())
                        {
                            member member;
                            member = db.member.Where(l => l.openid == openid).FirstOrDefault();
                            if (member == null)
                            {
                                member = new member();
                                member.gid = gid;
                                member.add_time = DateTime.Now;
                                member.account = "微信帐号";
                                member.login_identifier = LCommon.TimeToUNIX(DateTime.Now);
                                member.ip = Helper.IP;
                                member.pwd = MD5.GetMD5ljsheng("654123");
                                member.jurisdiction = "正常";
                                member.openid = openid;
                                member.nickname = context.Request.Form["nickName"];
                                member.picture = context.Request.Form["avatarUrl"];
                                member.gender = context.Request.Form["gender"] == "1" ? "男" : "女";
                                member.province = context.Request.Form["province"];
                                member.city = context.Request.Form["city"];
                                //member.area = null;
                                //member.real_name = null;
                                member.number = 0;
                                member.balance = 0;
                                db.member.Add(member);
                            }
                            else
                            {
                                TF = true;
                                gid = member.gid;
                                member.login_identifier = LCommon.TimeToUNIX(DateTime.Now);
                                member.nickname = context.Request.Form["nickName"];
                                member.picture = context.Request.Form["avatarUrl"];
                                member.gender = context.Request.Form["gender"] == "1" ? "男" : "女";
                                member.province = context.Request.Form["province"];
                                member.city = context.Request.Form["city"];
                            }
                            //添加新账号
                            if (db.SaveChanges() == 1)
                            {
                                TF = true;
                            }
                            login_identifier = member.login_identifier;
                        }
                    }
                }
                catch (Exception err)
                {
                    LogManager.WriteLog("err", err.Message);
                }
            }
            if (TF)
            {
                return new AjaxResult(new { gid, login_identifier });
            }
            else
            {
                return new AjaxResult(300, "登录异常,请退出微信在登录!");
            }
        }
        #endregion

        #region 快递
        /// <summary> 
        /// 快递查询
        /// </summary> 
        /// <param name="逻辑说明"></param> 
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public object kd(HttpContext context)
        {
            string json = "";
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine()); //Server.UrlDecode
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            string express = paramJson["express"].ToString();
            string express_number = paramJson["express_number"].ToString();
            if (!string.IsNullOrEmpty(express) && !string.IsNullOrEmpty(express_number))
            {
                string kd = KDAPI.requestKD("type=" + express + "&number=" + express_number);
                if (kd.Length > 10)
                {
                    JObject KDJson = JsonConvert.DeserializeObject(kd) as JObject;
                    LogManager.WriteLog("wuliu", KDJson["result"]["list"].ToString());
                    return new AjaxResult(new { list = KDJson["result"]["list"] });
                }
                else
                {
                    return new AjaxResult(300, "获取物流信息失败=" + kd);
                }
            }
            else
            {
                return new AjaxResult(300, "未发货!!!");
            }
        }
        #endregion
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}