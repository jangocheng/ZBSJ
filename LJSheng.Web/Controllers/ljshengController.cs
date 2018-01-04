using EntityFramework.Extensions;
using LJSheng.Common;
using LJSheng.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LJSheng.Web.Controllers
{
    public class ljshengController : LJSController
    {
        #region 后台管理
        // 后台操作提示
        public ActionResult msg()
        {
            ViewBag.msg = Request.QueryString["msg"];
            ViewBag.title = Request.QueryString["title"];
            ViewBag.url = Request.QueryString["url"];
            return View();
        }
        // 后台登录
        public ActionResult dl()
        {
            return View();
        }
        // 后台
        public ActionResult houtai()
        {
            return View();
        }
        // 菜单
        //[Authorize(Roles = "admins")]
        public ActionResult caidan()
        {
            return View();
        }
        // 头部
        public ActionResult toubu()
        {
            return View();
        }

        /// <summary>
        /// 头部数据
        /// </summary>
        [HttpPost]
        public JsonResult tb()
        {
            using (EFDB db = new EFDB())
            {
                var b = db.lorder.Where(l => l.pay_status == 1).GroupJoin(db.product,
                    x => x.product_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.express_status,
                        y.FirstOrDefault().classify_gid
                    }).GroupJoin(db.classify,
                    x => x.classify_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.express_status,
                        ltype = Nullable<int>.Equals(y.FirstOrDefault().ltype, null) ? 1 : y.FirstOrDefault().ltype
                    });
                return Json(new AjaxResult(new
                {
                    member = db.member.Count(),
                    order = db.lorder.Where(l => l.pay_status == 1).Count(),
                    kecheng = b.Where(l => l.ltype == 3).Count(),
                    video = b.Where(l => l.ltype == 2).Count(),
                    zhenbo = b.Where(l => l.ltype == 1).Count(),
                    express = b.Where(l => l.ltype == 1 && l.express_status == 1).Count()
                }));
            }
        }
        // 后台导航
        public ActionResult title()
        {
            return View();
        }
        // 后台中心
        public ActionResult index()
        {
            return View();
        }
        //快递
        public ActionResult express()
        {
            return View();
        }
        #endregion

        #region 会员模块
        /// <summary>
        /// 增加编辑
        /// </summary>
        public ActionResult memberau()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["gid"]))
            {
                using (EFDB db = new EFDB())
                {
                    Guid gid = Guid.Parse(Request.QueryString["gid"]);
                    member b = db.member.Where(l => l.gid == gid).FirstOrDefault();
                    ViewBag.account = b.account;
                    ViewBag.pwd = b.pwd;
                    ViewBag.real_name = b.real_name;
                    ViewBag.gender = b.gender;
                    ViewBag.nickname = b.nickname;
                    ViewBag.province = b.province;
                    ViewBag.city = b.city;
                    ViewBag.area = b.area;
                    ViewBag.openid = b.openid;
                    ViewBag.jurisdiction = b.jurisdiction;
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult memberau(Guid? gid)
        {
            using (EFDB db = new EFDB())
            {
                member b;
                string account = Request.Form["account"];
                string pwd = Request.Form["pwd"];
                if (db.member.Where(l => l.account == account && l.gid != gid).Count() > 0)
                {
                    return Helper.WebRedirect("操作失败！", "history.go(-1);", "帐号已存在!");
                }
                else
                {
                    if (gid == null)
                    {
                        b = new member();
                        b.gid = Guid.NewGuid();
                        b.add_time = DateTime.Now;
                        b.account = account;
                        b.login_identifier = "0000000000";
                        b.ip = Helper.IP;
                    }
                    else
                    {
                        b = db.member.Where(l => l.gid == gid).FirstOrDefault();
                    }
                    b.real_name = Request.Form["real_name"];
                    if (b.pwd != pwd)
                    {
                        b.pwd = MD5.GetMD5ljsheng(pwd);
                    }
                    b.jurisdiction = Request.Form["jurisdiction"];
                    b.gender = Request.Form["gender"];
                    b.nickname = Request.Form["nickname"];
                    b.province = Request.Form["province"];
                    b.city = Request.Form["city"];
                    b.area = Request.Form["area"];
                    if (gid == null)
                    {
                        db.member.Add(b);
                    }
                    if (db.SaveChanges() == 1)
                    {
                        return Helper.WebRedirect("操作成功！", "history.go(-1);", "恭喜你,操作成功!");
                    }
                    else
                    {
                        return Helper.WebRedirect("操作失败！", "history.go(-1);", "操作失败,请检查录入的数据!");
                    }
                }
            }

        }

        // 列表管理
        //同时支持Get和Post
        //[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult memberlist()
        {
            return View();
        }
        [HttpPost]
        public JsonResult member()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                string stime = paramJson["stime"].ToString();
                string etime = paramJson["etime"].ToString();
                string account = paramJson["account"].ToString();
                string real_name = paramJson["real_name"].ToString();
                string nickname = paramJson["nickname"].ToString();
                string contact_number = paramJson["contact_number"].ToString();
                var b = db.member.AsQueryable();
                if (!string.IsNullOrEmpty(account))
                {
                    b = b.Where(l => l.account.Contains(account));
                }
                if (!string.IsNullOrEmpty(real_name))
                {
                    b = b.Where(l => l.real_name.Contains(real_name));
                }
                if (!string.IsNullOrEmpty(nickname))
                {
                    b = b.Where(l => l.nickname.Contains(nickname));
                }
                if (!string.IsNullOrEmpty(contact_number))
                {
                    b = b.Where(l => l.contact_number.Contains(contact_number));
                }
                //时间查询
                if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
                {
                    DateTime? st = null;
                    DateTime? et = null;
                    if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(stime);
                    }
                    else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(etime);
                    }
                    else
                    {
                        st = DateTime.Parse(stime);
                        et = DateTime.Parse(etime);
                    }
                    b = b.Where(l => l.add_time >= st && l.add_time <= et);
                }
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                int count = b.Count();//总行数
                return Json(new AjaxResult(new
                {
                    count = count,
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList()
                }));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult memberdelete(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    if (db.member.Where(l => l.gid == gid).Delete() == 1)
                    {
                        return Json(new AjaxResult("成功"));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        /// <summary>
        /// 重设密码
        /// </summary>
        [HttpPost]
        public JsonResult memberpwd(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    string pwd = RandStr.CreateValidateNumber(6);
                    var pwdmm = db.member.Where(l => l.gid == gid).FirstOrDefault();
                    pwdmm.pwd = MD5.GetMD5ljsheng(pwd);
                    pwdmm.login_identifier = LCommon.TimeToUNIX(DateTime.Now);
                    if (db.SaveChanges() == 1)
                    {
                        return Json(new AjaxResult("新密码为:" + pwd));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        #endregion

        #region 管理员模块
        /// <summary>
        /// 增加编辑
        /// </summary>
        public ActionResult ljshengau()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["gid"]))
            {
                using (EFDB db = new EFDB())
                {
                    Guid gid = Guid.Parse(Request.QueryString["gid"]);
                    var b = db.ljsheng.Where(l => l.gid == gid).FirstOrDefault();
                    ViewBag.account = b.account;
                    ViewBag.pwd = b.pwd;
                    ViewBag.real_name = b.real_name;
                    ViewBag.gender = b.gender;
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult ljshengau(Guid? gid)
        {
            using (EFDB db = new EFDB())
            {
                ljsheng b;
                string account = Request.Form["account"];
                string pwd = Request.Form["pwd"];
                string real_name = Request.Form["real_name"];
                string gender = Request.Form["gender"];
                if (db.ljsheng.Where(l => l.account == account && l.gid != gid).Count() > 0)
                {
                    return Helper.WebRedirect("操作失败！", "history.go(-1);", "帐号已存在!");
                }
                else
                {
                    if (gid == null)
                    {
                        b = new Data.ljsheng();
                        b.gid = Guid.NewGuid();
                        b.add_time = DateTime.Now;
                        b.account = account;
                        b.login_identifier = "0000000000";
                    }
                    else
                    {
                        b = db.ljsheng.Where(l => l.gid == gid).FirstOrDefault();
                    }
                    b.real_name = real_name;
                    if (b.pwd != pwd)
                    {
                        b.pwd = MD5.GetMD5ljsheng(pwd);
                    }
                    b.gender = gender;
                    b.jurisdiction = "管理员";
                    if (gid == null)
                    {
                        db.ljsheng.Add(b);
                    }
                    if (db.SaveChanges() == 1)
                    {
                        return Helper.WebRedirect("操作成功！", "history.go(-1);", "恭喜你,操作成功!");
                    }
                    else
                    {
                        return Helper.WebRedirect("操作失败！", "history.go(-1);", "操作失败,请检查录入的数据!");
                    }
                }
            }
        }

        // 列表管理
        public ActionResult ljshenglist()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ljsheng()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                string stime = paramJson["stime"].ToString();
                string etime = paramJson["etime"].ToString();
                string account = paramJson["account"].ToString();
                string real_name = paramJson["real_name"].ToString();
                var b = db.ljsheng.AsQueryable();
                if (!string.IsNullOrEmpty(account))
                {
                    b = b.Where(l => l.account.Contains(account));
                }
                if (!string.IsNullOrEmpty(real_name))
                {
                    b = b.Where(l => l.real_name.Contains(real_name));
                }
                //时间查询
                if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
                {
                    DateTime? st = null;
                    DateTime? et = null;
                    if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(stime);
                    }
                    else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(etime);
                    }
                    else
                    {
                        st = DateTime.Parse(stime);
                        et = DateTime.Parse(etime);
                    }
                    b = b.Where(l => l.add_time >= st && l.add_time <= et);
                }
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                int count = b.Count();//总行数
                return Json(new AjaxResult(new
                {
                    count = count,
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList()
                }));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult ljshengdelete(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    if (db.ljsheng.Where(l => l.gid == gid).Delete() == 1)
                    {
                        return Json(new AjaxResult("成功"));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        /// <summary>
        /// 重设密码
        /// </summary>
        [HttpPost]
        public JsonResult ljshengpwd(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    string pwd = RandStr.CreateValidateNumber(6);
                    var pwdmm = db.ljsheng.Where(l => l.gid == gid).FirstOrDefault();
                    pwdmm.pwd = MD5.GetMD5ljsheng(pwd);
                    if (db.SaveChanges() == 1)
                    {
                        return Json(new AjaxResult("新密码为:" + pwd));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        #endregion

        #region 商家分类模块
        /// <summary>
        /// 增加编辑
        /// </summary>
        public ActionResult classifyau()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["gid"]))
            {
                using (EFDB db = new EFDB())
                {
                    Guid gid = Guid.Parse(Request.QueryString["gid"]);
                    var b = db.classify.Where(l => l.gid == gid).FirstOrDefault();
                    ViewBag.name = b.name;
                    ViewBag.sort = b.sort;
                    ViewBag.show = b.show;
                    ViewBag.ltype = b.ltype;
                }
            }
            else
            {
                ViewBag.sort = 1;
            }
            return View();
        }
        [HttpPost]
        public ActionResult classifyau(Guid? gid)
        {
            using (EFDB db = new EFDB())
            {
                classify b;
                string name = Request.Form["name"];
                if (db.classify.Where(l => l.name == name && l.gid != gid).Count() > 0)
                {
                    return Helper.WebRedirect("名称已存在！", "history.go(-1);", "名称已存在");
                }
                else
                {
                    if (gid == null)
                    {
                        b = new classify();
                        b.gid = Guid.NewGuid();
                        b.add_time = DateTime.Now;
                    }
                    else
                    {
                        b = db.classify.Where(l => l.gid == gid).FirstOrDefault();
                    }
                    b.name = name;
                    b.sort = Int32.Parse(Request.Form["sort"]);
                    b.show = Int32.Parse(Request.Form["show"]);
                    b.ltype = Int32.Parse(Request.Form["ltype"]);
                    if (gid == null)
                    {
                        db.classify.Add(b);
                    }
                    if (db.SaveChanges() == 1)
                    {
                        return Helper.WebRedirect("操作成功！", "history.go(-1);", "操作成功!");
                    }
                    else
                    {
                        return Helper.WebRedirect("操作失败,请检查录入的数据！", "history.go(-1);", "操作失败,请检查录入的数据!");
                    }
                }
            }
        }

        // 列表管理
        public ActionResult classifylist()
        {
            return View();
        }
        [HttpPost]
        public JsonResult classify()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                string stime = paramJson["stime"].ToString();
                string etime = paramJson["etime"].ToString();
                string name = paramJson["name"].ToString();
                var b = db.classify.AsQueryable();
                if (!string.IsNullOrEmpty(name))
                {
                    b = b.Where(l => l.name.Contains(name));
                }
                //时间查询
                if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
                {
                    DateTime? st = null;
                    DateTime? et = null;
                    if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(stime);
                    }
                    else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(etime);
                    }
                    else
                    {
                        st = DateTime.Parse(stime);
                        et = DateTime.Parse(etime);
                    }
                    b = b.Where(l => l.add_time >= st && l.add_time <= et);
                }
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                int count = b.Count();//总行数
                return Json(new AjaxResult(new
                {
                    count = count,
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList()
                }));
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult classifydelete(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    if (db.classify.Where(l => l.gid == gid).Delete() == 1)
                    {
                        return Json(new AjaxResult("成功"));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        #endregion

        #region 清空推荐
        /// <summary>
        /// ljsheng
        /// </summary>
        /// <param name="ljsheng">传入参数</param>
        /// <returns>返回调用结果</returns>
        /// <para name="result">200 是成功其他失败</para>
        /// <para name="data">对象结果</para>
        /// <remarks>
        /// 2018-08-18 林建生
        /// </remarks>
        [HttpPost]
        public JsonResult updatevideo()
        {
            using (EFDB db = new EFDB())
            {
                db.product.Where(l => l.show == 3).Update(l => new product { show = 1 });
                return Json(new AjaxResult("取消成功,请重新设置需要推荐的视频!"));
            }
        }
        #endregion

        #region 产品模块
        /// <summary>
        /// 增加编辑
        /// </summary>
        public ActionResult productau()
        {
            if (Request.QueryString["ltype"] == "1")
            {
                ViewBag.title1 = "无须填写";
                ViewBag.title2 = "无须填写";
                ViewBag.title3 = "无须填写";
                ViewBag.title4 = "库存";
            }
            else if (Request.QueryString["ltype"] == "2")
            {
                ViewBag.title1 = "讲师";
                ViewBag.title2 = "视频时长";
                ViewBag.title3 = "播放地址";
                ViewBag.title4 = Request.QueryString["lx"] == "0" ? "视频数量" : "可看次数";
            }
            else
            {
                ViewBag.title1 = "讲师";
                ViewBag.title2 = "课程时间";
                ViewBag.title3 = "课程地址";
                ViewBag.title4 = "课程人数";
            }
            using (EFDB db = new EFDB())
            {
                if (!string.IsNullOrEmpty(Request.QueryString["gid"]))
                {
                    Guid gid = Guid.Parse(Request.QueryString["gid"]);
                    var b = db.product.Where(l => l.gid == gid).FirstOrDefault();
                    ViewBag.name = b.name;
                    ViewBag.price = b.price;
                    ViewBag.original_price = b.original_price;
                    ViewBag.subtitle = b.subtitle;
                    ViewBag.content = b.content;
                    ViewBag.sort = b.sort;
                    ViewBag.show = b.show;
                    ViewBag.picture = b.picture;
                    ViewBag.graphic_details = b.graphic_details;
                    ViewBag.classify_gid = b.classify_gid;
                    ViewBag.remarks = b.remarks;
                    ViewBag.extend1 = b.extend1;
                    ViewBag.extend2 = b.extend2;
                    ViewBag.extend3 = b.extend3;
                    ViewBag.extend4 = b.extend4;
                }
                else
                {
                    ViewBag.sort = 1;
                    ViewBag.extend4 = 99999;
                }
                int ltype = Int32.Parse(Request.QueryString["ltype"]);
                return View(db.classify.Where(l => l.ltype == ltype).ToList());
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult productau(Guid? gid)
        {
            using (EFDB db = new EFDB())
            {
                product b;
                string name = Request.Form["name"];
                Guid classify_gid = Guid.Parse(Request.Form["classify_gid"]);
                if (db.product.Where(l => l.name == name && l.gid != gid && l.classify_gid == classify_gid).Count() > 0)
                {
                    return Helper.WebRedirect("名称已存在！", "history.go(-1);", "名称已存在");
                }
                else
                {
                    if (gid == null)
                    {
                        b = new product();
                        b.gid = Guid.NewGuid();
                        b.add_time = DateTime.Now;
                    }
                    else
                    {
                        b = db.product.Where(l => l.gid == gid).FirstOrDefault();
                    }
                    b.classify_gid = classify_gid;
                    b.name = name;
                    b.price = decimal.Parse(Request.Form["price"]);
                    b.original_price = decimal.Parse(Request.Form["original_price"]);
                    b.subtitle = Request.Form["subtitle"];
                    b.content = Request.Form["content"];
                    b.remarks = Request.Form["remarks"];
                    b.extend1 = Request.Form["extend1"];
                    b.extend2 = Request.Form["extend2"];
                    b.extend3 = Request.Form["extend3"];
                    if (!string.IsNullOrEmpty(Request.Form["extend4"]))
                    {
                        b.extend4 = Int32.Parse(Request.Form["extend4"]);
                    }
                    b.show = Int32.Parse(Request.Form["show"]);
                    b.sort = Int32.Parse(Request.Form["sort"]); ;
                    if (!string.IsNullOrEmpty(Request.Form["picture"]))
                    {
                        b.picture = Request.Form["picture"];
                    }
                    if (!string.IsNullOrEmpty(Request.Form["graphic_details"]))
                    {
                        b.graphic_details = Request.Form["graphic_details"];
                    }
                    if (gid == null)
                    {
                        db.product.Add(b);
                    }
                    if (db.SaveChanges() == 1)
                    {
                        return Helper.WebRedirect("操作成功！", "history.go(-1);", "操作成功!");
                    }
                    else
                    {
                        return Helper.WebRedirect("操作失败,请检查录入的数据！", "history.go(-1);", "操作失败,请检查录入的数据!");
                    }
                }
            }
        }

        // 列表管理
        public ActionResult productlist()
        {
            using (EFDB db = new EFDB())
            {
                int ltype = Int32.Parse(Request.QueryString["ltype"]);
                return View(db.classify.Where(l => l.ltype == ltype).ToList());
            }
        }
        [HttpPost]
        public JsonResult product()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                string stime = paramJson["stime"].ToString();
                string etime = paramJson["etime"].ToString();
                string name = paramJson["name"].ToString();
                int ltype = Int32.Parse(paramJson["ltype"].ToString());
                var b = db.product.Where(l => l.sort < 10000).GroupJoin(db.classify,
                    x => x.classify_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.gid,
                        x.name,
                        x.picture,
                        x.show,
                        x.sort,
                        x.price,
                        x.original_price,
                        x.add_time,
                        x.classify_gid,
                        classify_name = y.FirstOrDefault().name,
                        y.FirstOrDefault().ltype
                    }).Where(l => l.ltype == ltype).AsQueryable();
                if (!string.IsNullOrEmpty(name))
                {
                    b = b.Where(l => l.name.Contains(name));
                }
                Guid? classify_gid = null;
                if (paramJson["classify_gid"].ToString() != "0")
                {
                    classify_gid = Guid.Parse(paramJson["classify_gid"].ToString());
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["classify_gid"]))
                    {
                        classify_gid = Guid.Parse(Request.QueryString["classify_gid"]);
                    }
                }
                if (classify_gid != null)
                {
                    b = b.Where(l => l.classify_gid == classify_gid);
                }
                //时间查询
                if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
                {
                    DateTime? st = null;
                    DateTime? et = null;
                    if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(stime);
                    }
                    else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(etime);
                    }
                    else
                    {
                        st = DateTime.Parse(stime);
                        et = DateTime.Parse(etime);
                    }
                    b = b.Where(l => l.add_time >= st && l.add_time <= et);
                }
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                int count = b.Count();//总行数
                return Json(new AjaxResult(new
                {
                    count = count,
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList()
                }));
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult productdelete(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    if (db.product.Where(l => l.gid == gid).Delete() == 1)
                    {
                        return Json(new AjaxResult("成功"));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        #endregion

        #region 图片上传
        /// <summary>
        /// layui图片上传
        /// </summary>
        [HttpPost]
        public JsonResult uploadpicture(string path)
        {
            HttpPostedFileBase picture = Request.Files["file"];
            string filename = "";
            if (picture != null)
            {
                filename = Helper.UploadFiles(path, picture);
            }
            return Json(new { code = 0, msg = "", data = new { src = path + filename, title = filename } });
        }
        #endregion

        #region 订单管理
        /// <summary>
        /// 增加编辑
        /// </summary>
        public ActionResult lorderau()
        {
            using (EFDB db = new EFDB())
            {
                if (!string.IsNullOrEmpty(Request.QueryString["gid"]))
                {
                    Guid gid = Guid.Parse(Request.QueryString["gid"]);
                    var b = db.lorder.Where(l => l.gid == gid).FirstOrDefault();
                    ViewBag.order_no = b.order_no;
                    ViewBag.trade_no = b.trade_no;
                    ViewBag.pay_price = b.pay_price;
                    ViewBag.remarks = b.remarks;
                    ViewBag.express_status = b.express_status;
                    ViewBag.express = b.express;
                    ViewBag.express_number = b.express_number;
                    ViewBag.address = b.address;
                    ViewBag.consignee = b.consignee;
                    ViewBag.contact_number = b.contact_number;
                    ViewBag.member = db.member.Where(m => m.gid == b.member_gid).FirstOrDefault().nickname;
                    ViewBag.product = b.product_gid != null ? db.product.Where(p => p.gid == b.product_gid).FirstOrDefault().name : "多商品订单";
                }
                else
                {
                    ViewBag.sort = 1;
                }
                return View(db.kd.Where(l => l.show == 1).OrderBy(l => l.sort).ToList());
            }
        }

        [HttpPost]
        public ActionResult lorderau(Guid gid)
        {
            using (EFDB db = new EFDB())
            {
                var b = db.lorder.Where(l => l.gid == gid).FirstOrDefault();
                b.remarks = Request.Form["remarks"];
                b.express = Request.Form["express"];
                b.express_number = Request.Form["express_number"];
                b.address = Request.Form["address"];
                b.consignee = Request.Form["consignee"];
                b.contact_number = Request.Form["contact_number"];
                b.express_status = Int32.Parse(Request.Form["express_status"]);
                if (db.SaveChanges() == 1)
                {
                    return Helper.WebRedirect("操作成功！", "history.go(-1);", "操作成功!");
                }
                else
                {
                    return Helper.WebRedirect("操作失败,请检查录入的数据！", "history.go(-1);", "操作失败,请检查录入的数据!");
                }
            }
        }

        public ActionResult lorderlist()
        {
            return View();
        }

        [HttpPost]
        public ActionResult lorder()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                string stime = paramJson["stime"].ToString();
                string etime = paramJson["etime"].ToString();
                var b = db.lorder.GroupJoin(db.member,
                    x => x.member_gid,
                    y => y.gid,
                    (l, j) => new
                    {
                        l.gid,
                        l.add_time,
                        l.member_gid,
                        l.product_gid,
                        l.order_no,
                        l.trade_no,
                        l.pay_type,
                        l.pay_status,
                        l.total_price,
                        l.pay_price,
                        l.price,
                        l.number,
                        l.express_status,
                        l.express,
                        l.express_number,
                        l.pay_time,
                        j.FirstOrDefault().nickname
                    }).GroupJoin(db.product,
                    x => x.product_gid,
                    y => y.gid,
                    (l, j) => new
                    {
                        l.gid,
                        l.add_time,
                        l.member_gid,
                        l.product_gid,
                        l.order_no,
                        l.trade_no,
                        l.pay_type,
                        l.pay_status,
                        l.total_price,
                        l.pay_price,
                        l.price,
                        l.number,
                        l.express_status,
                        l.express,
                        l.express_number,
                        l.pay_time,
                        l.nickname,
                        j.FirstOrDefault().classify_gid,
                        name = Nullable<Guid>.Equals(l.product_gid, null) ? "多商品订单" : j.FirstOrDefault().name
                    }).GroupJoin(db.classify,
                    x => x.classify_gid,
                    y => y.gid,
                    (l, j) => new
                    {
                        l.gid,
                        l.add_time,
                        l.member_gid,
                        l.product_gid,
                        l.order_no,
                        l.trade_no,
                        l.pay_type,
                        l.pay_status,
                        l.total_price,
                        l.pay_price,
                        l.price,
                        l.number,
                        l.express_status,
                        l.express,
                        l.express_number,
                        l.pay_time,
                        l.nickname,
                        l.classify_gid,
                        l.name,
                        ltype = Nullable<int>.Equals(j.FirstOrDefault().ltype, null) ? 1 : j.FirstOrDefault().ltype
                    }).ToList().AsQueryable();
                int ltype = int.Parse(paramJson["ltype"].ToString());
                if (ltype != 0)
                {
                    b = b.Where(l => l.ltype==ltype);
                }
                if (!string.IsNullOrEmpty(paramJson["order_no"].ToString()))
                {
                    b = b.Where(l => l.order_no.Contains(paramJson["order_no"].ToString()));
                }
                if (!string.IsNullOrEmpty(paramJson["trade_no"].ToString()))
                {
                    b = b.Where(l => l.trade_no == paramJson["trade_no"].ToString());
                }
                if (!string.IsNullOrEmpty(paramJson["nickname"].ToString()))
                {
                    b = b.Where(l => l.nickname.Contains(paramJson["nickname"].ToString()));
                }
                if (!string.IsNullOrEmpty(paramJson["name"].ToString()))
                {
                    b = b.Where(l => l.name.Contains(paramJson["name"].ToString()));
                }
                if (!string.IsNullOrEmpty(paramJson["member_gid"].ToString()))
                {
                    Guid member_gid = Guid.Parse(paramJson["member_gid"].ToString());
                    b = b.Where(l => l.member_gid == member_gid);
                }
                if (!string.IsNullOrEmpty(paramJson["product_gid"].ToString()))
                {
                    Guid product_gid = Guid.Parse(paramJson["product_gid"].ToString());
                    b = b.Where(l => l.product_gid == product_gid);
                }
                if (paramJson["pay_type"].ToString() != "0")
                {
                    int pay_type = int.Parse(paramJson["pay_type"].ToString());
                    b = b.Where(l => l.pay_type == pay_type);
                }
                if (paramJson["pay_status"].ToString() != "0")
                {
                    int pay_status = int.Parse(paramJson["pay_status"].ToString());
                    b = b.Where(l => l.pay_status == pay_status);
                }
                if (paramJson["express_status"].ToString() != "0")
                {
                    int express_status = int.Parse(paramJson["express_status"].ToString());
                    b = b.Where(l => l.express_status == express_status);
                }
                //时间查询
                if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
                {
                    DateTime? st = null;
                    DateTime? et = null;
                    if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(stime);
                    }
                    else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(etime);
                    }
                    else
                    {
                        st = DateTime.Parse(stime);
                        et = DateTime.Parse(etime);
                    }
                    b = b.Where(l => l.add_time >= st && l.add_time <= et);
                }
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                return Json(new AjaxResult(new
                {
                    count = b.Count(),
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize)
                }));
            }
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        public ActionResult order_detailslist()
        {
            return View();
        }
        [HttpPost]
        public ActionResult order_details()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                Guid gid = Guid.Parse(paramJson["gid"].ToString());
                var b = db.order_details.Where(l => l.order_gid == gid).GroupJoin(db.product,
                    x => x.product_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.price,
                        x.number,
                        x.add_time,
                        y.FirstOrDefault().name
                    }).AsQueryable();
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                return Json(new AjaxResult(new
                {
                    count = b.Count(),
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize)
                }));
            }
        }
        #endregion

        #region 评论管理
        /// <summary>
        /// 增加编辑
        /// </summary>
        public ActionResult commentau()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["gid"]))
            {
                using (EFDB db = new EFDB())
                {
                    Guid gid = Guid.Parse(Request.QueryString["gid"]);
                    var b = db.comment.Where(l => l.gid == gid).FirstOrDefault();
                    ViewBag.content = b.content;
                    ViewBag.reply = b.reply;
                    ViewBag.sort = b.sort;
                    ViewBag.show = b.show;
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult commentau(Guid? gid)
        {
            using (EFDB db = new EFDB())
            {
                string content = Request.Form["content"];
                string reply = Request.Form["reply"];
                string show = Request.Form["show"];
                var b = db.comment.Where(l => l.gid == gid).FirstOrDefault();
                b.content = Request.Form["content"];
                if (!string.IsNullOrEmpty(Request.Form["reply"]))
                {
                    b.reply = Request.Form["reply"];
                    b.reply_time = DateTime.Now;
                }
                b.show = show == "显示" ? 1 : 2;
                b.sort = int.Parse(Request.Form["sort"]);
                if (db.SaveChanges() == 1)
                {
                    return Helper.WebRedirect("操作成功！", "history.go(-1);", "操作成功!");
                }
                else
                {
                    return Helper.WebRedirect("操作失败,请检查录入的数据！", "history.go(-1);", "操作失败,请检查录入的数据!");
                }
            }
        }
        public ActionResult commentlist()
        {
            return View();
        }

        [HttpPost]
        public ActionResult comment()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                string stime = paramJson["stime"].ToString();
                string etime = paramJson["etime"].ToString();
                var b = db.comment.Select(l => new
                {
                    l.gid,
                    l.add_time,
                    l.member_gid,
                    l.product_gid,
                    l.content,
                    l.reply,
                    l.reply_time,
                    member = db.member.Where(m => m.gid == l.member_gid).FirstOrDefault().nickname,
                    product = db.product.Where(p => p.gid == l.product_gid).FirstOrDefault().name
                }).AsQueryable();
                if (!string.IsNullOrEmpty(paramJson["content"].ToString()))
                {
                    b = b.Where(l => l.content.Contains(paramJson["content"].ToString()) || l.reply.Contains(paramJson["content"].ToString()));
                }
                if (!string.IsNullOrEmpty(paramJson["member"].ToString()))
                {
                    b = b.Where(l => l.member.Contains(paramJson["member"].ToString()));
                }
                if (!string.IsNullOrEmpty(paramJson["product"].ToString()))
                {
                    b = b.Where(l => l.product.Contains(paramJson["product"].ToString()));
                }
                if (!string.IsNullOrEmpty(paramJson["member_gid"].ToString()))
                {
                    Guid member_gid = Guid.Parse(paramJson["member_gid"].ToString());
                    b = b.Where(l => l.member_gid == member_gid);
                }
                if (!string.IsNullOrEmpty(paramJson["product_gid"].ToString()))
                {
                    Guid product_gid = Guid.Parse(paramJson["product_gid"].ToString());
                    b = b.Where(l => l.product_gid == product_gid);
                }
                //时间查询
                if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
                {
                    DateTime? st = null;
                    DateTime? et = null;
                    if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(stime);
                    }
                    else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(etime);
                    }
                    else
                    {
                        st = DateTime.Parse(stime);
                        et = DateTime.Parse(etime);
                    }
                    b = b.Where(l => l.add_time >= st && l.add_time <= et);
                }
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                return Json(new AjaxResult(new
                {
                    count = b.Count(),
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList()
                }));
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult commentdelete(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    if (db.comment.Where(l => l.gid == gid).Delete() == 1)
                    {
                        return Json(new AjaxResult("成功"));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        #endregion

        #region 财务报表
        /// <summary>
        /// 报表
        /// </summary>
        /// <returns>返回调用结果</returns>
        /// <para name="result">200 是成功其他失败</para>
        /// <para name="data">对象结果</para>
        /// <remarks>
        /// 2018-08-18 林建生
        /// </remarks>
        public ActionResult baobiao()
        {
            string stime = ViewBag.stime = Request.Form["stime"];
            string etime = ViewBag.etime = Request.Form["etime"];
            //时间查询
            DateTime? st = DateTime.Now;
            DateTime? et = DateTime.Now;
            if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
            {
                if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                {
                    st = et = DateTime.Parse(stime);
                }
                else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                {
                    st = et = DateTime.Parse(etime);
                }
                else
                {
                    st = DateTime.Parse(stime);
                    et = DateTime.Parse(etime);
                }
            }
            using (EFDB db = new EFDB())
            {
                var order = db.lorder.Where(l => l.pay_status == 1).GroupJoin(db.product,
                    x => x.product_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.add_time,
                        x.pay_status,
                        x.price,
                        y.FirstOrDefault().classify_gid
                    }).GroupJoin(db.classify,
                    x => x.classify_gid,
                    y => y.gid,
                    (x, y) => new
                    {
                        x.add_time,
                        x.pay_status,
                        x.price,
                        y.FirstOrDefault().ltype
                    });
                if (order.Count() > 0)
                {
                    ViewBag.order = order.Select(l => l.price).DefaultIfEmpty(0m).Sum();
                    ViewBag.product = order.Where(l => l.ltype == 1).Select(l => l.price).DefaultIfEmpty(0m).Sum();
                    ViewBag.sp = order.Where(l => l.ltype == 2).Select(l => l.price).DefaultIfEmpty(0m).Sum();
                    ViewBag.kc = order.Where(l => l.ltype == 3).Select(l => l.price).DefaultIfEmpty(0m).Sum();
                }
                var ordertime = order.Where(l => l.add_time >= st && l.add_time <= et);
                if (ordertime.Count() > 0)
                {
                    ViewBag.ordertime = ordertime.Select(l => l.price).DefaultIfEmpty(0m).Sum();
                    ViewBag.producttime = ordertime.Where(l => l.ltype == 1).Select(l => l.price).DefaultIfEmpty(0m).Sum();
                    ViewBag.sptime = ordertime.Where(l => l.ltype == 2).Select(l => l.price).DefaultIfEmpty(0m).Sum();
                    ViewBag.kctime = ordertime.Where(l => l.ltype == 3).Select(l => l.price).DefaultIfEmpty(0m).Sum();
                }
            }
            return View();
        }
        #endregion

        #region 新闻资讯模块
        /// <summary>
        /// 增加编辑
        /// </summary>
        public ActionResult newsau()
        {
            using (EFDB db = new EFDB())
            {
                if (!string.IsNullOrEmpty(Request.QueryString["gid"]))
                {
                    Guid gid = Guid.Parse(Request.QueryString["gid"]);
                    var b = db.news.Where(l => l.gid == gid).FirstOrDefault();
                    ViewBag.title = b.title;
                    ViewBag.subtitle = b.subtitle;
                    ViewBag.content = b.content;
                    ViewBag.sort = b.sort;
                    ViewBag.show = b.show;
                    ViewBag.picture = b.picture;
                    ViewBag.graphic_details = b.graphic_details;
                    ViewBag.url = b.url;
                    ViewBag.author = b.author;
                    ViewBag.number = b.number;
                }
                else
                {
                    ViewBag.number = 1;
                    ViewBag.sort = 1;
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult newsau(Guid? gid)
        {
            using (EFDB db = new EFDB())
            {
                news b;
                string title = Request.Form["title"];
                if (db.news.Where(l => l.title == title && l.gid != gid).Count() > 0)
                {
                    return Helper.WebRedirect("名称已存在！", "history.go(-1);", "名称已存在");
                }
                else
                {
                    if (gid == null)
                    {
                        b = new news();
                        b.gid = Guid.NewGuid();
                        b.add_time = DateTime.Now;
                    }
                    else
                    {
                        b = db.news.Where(l => l.gid == gid).FirstOrDefault();
                    }
                    b.title = title;
                    b.number = int.Parse(Request.Form["number"]);
                    b.subtitle = Request.Form["subtitle"];
                    b.content = Request.Form["content"];
                    b.url = Request.Form["url"];
                    b.author = Request.Form["author"];
                    b.show = Int32.Parse(Request.Form["show"]);
                    b.sort = Int32.Parse(Request.Form["sort"]); ;
                    if (!string.IsNullOrEmpty(Request.Form["picture"]))
                    {
                        b.picture = Request.Form["picture"];
                    }
                    if (!string.IsNullOrEmpty(Request.Form["graphic_details"]))
                    {
                        b.graphic_details = Request.Form["graphic_details"];
                    }
                    if (gid == null)
                    {
                        db.news.Add(b);
                    }
                    if (db.SaveChanges() == 1)
                    {
                        return Helper.WebRedirect("操作成功！", "history.go(-1);", "操作成功!");
                    }
                    else
                    {
                        return Helper.WebRedirect("操作失败,请检查录入的数据！", "history.go(-1);", "操作失败,请检查录入的数据!");
                    }
                }
            }
        }

        // 列表管理
        public ActionResult newslist()
        {
            return View();
        }
        [HttpPost]
        public JsonResult news()
        {
            //查询的参数json
            string json = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                json = HttpUtility.UrlDecode(sr.ReadLine());
            }
            //解析参数
            JObject paramJson = JsonConvert.DeserializeObject(json) as JObject;
            using (EFDB db = new EFDB())
            {
                string stime = paramJson["stime"].ToString();
                string etime = paramJson["etime"].ToString();
                string title = paramJson["title"].ToString();
                var b = db.news.AsQueryable();
                if (!string.IsNullOrEmpty(title))
                {
                    b = b.Where(l => l.title.Contains(title));
                }
                //时间查询
                if (!string.IsNullOrEmpty(stime) || !string.IsNullOrEmpty(etime))
                {
                    DateTime? st = null;
                    DateTime? et = null;
                    if (!string.IsNullOrEmpty(stime) && string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(stime);
                    }
                    else if (string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
                    {
                        st = et = DateTime.Parse(etime);
                    }
                    else
                    {
                        st = DateTime.Parse(stime);
                        et = DateTime.Parse(etime);
                    }
                    b = b.Where(l => l.add_time >= st && l.add_time <= et);
                }
                int pageindex = Int32.Parse(paramJson["pageindex"].ToString());//当前页数
                int pagesize = Int32.Parse(paramJson["pagesize"].ToString()); ;//每页显示的数量
                int count = b.Count();//总行数
                return Json(new AjaxResult(new
                {
                    count = count,
                    pageindex = pageindex,
                    list = b.OrderByDescending(l => l.add_time).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList()
                }));
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult newsdelete(Guid gid)
        {
            if (gid != null)
            {
                using (EFDB db = new EFDB())
                {
                    if (db.news.Where(l => l.gid == gid).Delete() == 1)
                    {
                        return Json(new AjaxResult("成功"));
                    }
                    else
                    {
                        return Json(new AjaxResult(300, "失败"));
                    }
                }
            }
            else
            {
                return Json(new AjaxResult(300, "非法参数"));
            }
        }
        #endregion

        #region 快递相关
        /// <summary>
        /// 更新快递公司
        /// </summary>
        /// <returns>返回调用结果</returns>
        /// <para name="result">200 是成功其他失败</para>
        /// <para name="data">对象结果</para>
        /// <remarks>
        /// 2018-08-18 林建生
        /// </remarks>
        public JsonResult kd()
        {
            using (EFDB db = new EFDB())
            {
                //读取json文件  
                //string jsonPath = Server.MapPath("/kd.json");
                //string jsonstr = string.Empty;
                //using (FileStream fs = new FileStream(jsonPath, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
                //{
                //    using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("gb2312")))
                //    {
                //        jsonstr = sr.ReadToEnd().ToString();
                //    }
                //}
                string kd = KDAPI.request();
                if (kd.Length > 100)
                {
                    JObject paramJson = JsonConvert.DeserializeObject(kd) as JObject;
                    JArray json = (JArray)JsonConvert.DeserializeObject(paramJson["result"].ToString());
                    if (json.Count() > 0)
                    {
                        db.kd.Delete();
                        foreach (var j in json)
                        {
                            var b = new kd();
                            b.gid = Guid.NewGuid();
                            b.add_time = DateTime.Now;
                            b.sort = 1;
                            b.show = 1;
                            b.name = j["name"].ToString();
                            b.type = j["type"].ToString();
                            b.letter = j["letter"].ToString();
                            b.tel = j["tel"].ToString().Length >= 50 ? "" : j["tel"].ToString();
                            b.number = j["number"].ToString();
                            db.kd.Add(b);
                        }
                        if (db.SaveChanges() > 0)
                        {
                            return Json(new AjaxResult("更新成功"));
                        }
                    }
                }
                return Json(new AjaxResult(300, "失败,请查看快递接口次数是否已经用完=" + kd));
            }
        }
        #endregion
    }
}