using System;
using System.Linq;
using System.Text;
using System.Web;
using EntityFramework.Extensions;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using LJSheng.Data;
using static LJSheng.Web.LJShengHelper;
using LJSheng.WX;
using LJSheng.Common;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LJSheng.Web
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public static class Helper
    {
        #region 订单操作
        /// <summary>
        /// 支付成功更新订单
        /// </summary>
        /// <param name="order_no">网站订单号</param>
        /// <param name="trade_no">网银订单号</param>
        /// <param name="pay_type">支付类型</param>
        /// <param name="pay_amount">在线支付金额</param>
        /// <param name="remarks">返回的支付备注信息</param>
        /// <returns>返回调用结果</returns>
        public static bool payOrder(string order_no)
        {
            using (EFDB db = new EFDB())
            {
                bool isok = false;
                string pay = WXPay.Get_Order(order_no);
                if (!string.IsNullOrEmpty(pay))
                {
                    //获取返回的支付参数
                    JObject payJson = JsonConvert.DeserializeObject(pay) as JObject;
                    string trade_no = payJson["trade_no"].ToString();
                    string attach = payJson["attach"].ToString();
                    int pay_type = Int32.Parse(payJson["pay_type"].ToString());
                    decimal pay_amount = decimal.Parse(payJson["pay_amount"].ToString());
                    //解析备注信息
                    JObject paramJson = JsonConvert.DeserializeObject(attach) as JObject;
                    int type = Int32.Parse(paramJson["type"].ToString());
                    string LogMsg = "订单号:" + order_no + ",网银订单号:" + trade_no + ",支付类型:" + pay_type.ToString() + ",网上支付金额:" + pay_amount.ToString() + ",备注:" + attach;
                    //支付类型
                    string payname = ((PayType)Enum.Parse(typeof(LJShengHelper.PayType), pay_type.ToString())).ToString();

                    var b = db.lorder.Where(l => l.order_no == order_no).FirstOrDefault();
                    if (b != null && b.pay_status == 2)
                    {
                        if (b.price == pay_amount)
                        {
                            b.pay_status = 1;
                        }
                        else
                        {
                            b.pay_status = 5;
                        }
                        b.trade_no = trade_no;
                        b.pay_time = DateTime.Now;
                        b.pay_type = pay_type;
                        b.pay_price = pay_amount;
                        b.express_status = 1;
                        //b.remarks = remarks;
                        if (db.SaveChanges() == 1)
                        {
                            //type[1 = 真柏 2 = 视频 3 = 课程]
                            if (type == 2)
                            {
                                Guid product_gid = (Guid)b.product_gid;
                                Guid member_gid = b.member_gid;
                                var p = db.product.Where(l => l.gid == product_gid).FirstOrDefault();
                                if (p != null)
                                {
                                    //套餐逻辑
                                    if (p.sort >= 10000)
                                    {
                                        int number = (int)p.extend4;
                                        if (db.member.Where(l => l.gid == member_gid).Update(l => new member { number = l.number + number }) == 1)
                                        {
                                            isok = true;
                                        }
                                        else
                                        {
                                            LogManager.WriteLog("对账增加会员次数失败", "product_gid=" + product_gid.ToString() + ",member_gid=" + member_gid.ToString());
                                        }
                                    }
                                    else
                                    {
                                        //单独购买视频
                                        var vr = new video_record();
                                        vr.gid = Guid.NewGuid();
                                        vr.add_time = DateTime.Now;
                                        vr.product_gid = product_gid;
                                        vr.member_gid = member_gid;
                                        vr.video_time = DateTime.Now;
                                        vr.number = (int)db.product.Where(l => l.gid == product_gid).FirstOrDefault().extend4;
                                        db.video_record.Add(vr);
                                        if (db.SaveChanges() == 1)
                                        {
                                            isok = true;
                                        }
                                        else
                                        {
                                            LogManager.WriteLog("对账增加观看记录失败", "product_gid=" + product_gid.ToString() + ",member_gid=" + member_gid.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                isok = true;
                                //扣除库存
                                Guid ordergid = db.lorder.Where(l => l.order_no == order_no).FirstOrDefault().gid;
                                var od = db.order_details.Where(l => l.order_gid == ordergid).ToList();
                                foreach (var dr in od)
                                {
                                    db.product.Where(l => l.gid == dr.product_gid).Update(l => new product { extend4 = l.extend4 - dr.number });
                                }
                            }
                        }
                        else
                        {
                            LogManager.WriteLog("支付成功更新订单失败", LogMsg);
                        }
                    }
                }
                else
                {
                    LogManager.WriteLog("查询订单失败", order_no);
                }
                return isok;
            }
        }

        /// <summary>
        /// 微信下单
        /// </summary>
        /// <param name="order_no">系统订单号</param>
        /// <param name="product">商品gid列表</param>
        /// <param name="member_gid">会员gid</param>
        /// <param name="openid">会员openid</param>
        /// <param name="type">1=真柏 2=视频 3=课程</param>
        /// <param name="remarks">备注</param>
        /// <param name="address">快递地址</param>
        /// <param name="consignee">收货人</param>
        /// <param name="contact_number">联系电话</param>
        /// <returns>返回调用结果</returns>
        /// <para name="result">200 是成功其他失败</para>
        /// <para name="data">对象结果</para>
        /// <remarks>
        /// 2018-08-18 林建生
        /// </remarks>
        public static string wxpay(string order_no, string product, Guid member_gid, string openid, int type,string remarks, string address = "", string consignee = "", string contact_number = "")
        {
            using (EFDB db = new EFDB())
            {
                //订单的gid
                Guid order_gid = Guid.NewGuid();
                //产品金额
                decimal total_price = 0;
                //产品名称
                string body = "多个商品订单";
                //添加订单产品列表
                JArray json = (JArray)JsonConvert.DeserializeObject(product);
                Guid? product_gid = null;
                int number = 0;
                foreach (var j in json)
                {
                    product_gid = Guid.Parse(j["product_gid"].ToString());
                    number = int.Parse(j["number"].ToString());
                    var p = db.product.Where(l => l.gid == product_gid).FirstOrDefault();
                    if (json.Count() == 1)
                    {
                        body = p.name;
                    }
                    //total_price += p.price * number;
                    var od = new order_details();
                    od.gid = Guid.NewGuid();
                    od.add_time = DateTime.Now;
                    od.order_gid = order_gid;
                    od.product_gid = (Guid)product_gid;
                    od.number = number;
                    od.price = p.price;
                    od.pay_price = p.price;
                    db.order_details.Add(od);
                }
                if (db.SaveChanges() == json.Count())
                {
                    //备注
                    string attach = JsonConvert.SerializeObject(new { type = type, total_price, order_gid });
                    //string order_no = RandStr.CreateOrderNO();
                    total_price = db.order_details.Where(l => l.order_gid == order_gid).Sum(l => l.price * l.number);
                    //生成订单
                    var b = new lorder();
                    b.gid = order_gid;
                    b.add_time = DateTime.Now;
                    b.order_no = order_no;
                    b.member_gid = member_gid;
                    b.pay_status = 2;
                    b.pay_type = 2;
                    b.total_price = total_price;
                    b.price = total_price;
                    b.number = type != 1 ? number: json.Count();
                    b.coupon_price = 0;
                    b.express_status = 1;
                    b.address = address;
                    b.consignee = consignee;
                    b.contact_number = contact_number;
                    b.remarks = string.IsNullOrEmpty(remarks) ? attach : remarks;
                    if (type != 1)
                    {
                        b.product_gid = product_gid;
                    }
                    db.lorder.Add(b);
                    if (db.SaveChanges() == 1)
                    {
                        //开始微信统一下单
                        string _Pay_Package = WXPay.Get_RequestHtml(openid, order_no, total_price, body, attach);
                        //微信jspai支付
                        if (_Pay_Package.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(consignee) && !string.IsNullOrEmpty(contact_number) && !string.IsNullOrEmpty(address))
                            {
                                db.member.Where(l => l.gid == member_gid).Update(l => new member { address = address, contact_number = contact_number, real_name = consignee });
                            }
                            return _Pay_Package;
                        }
                        else
                        {
                            db.lorder.Where(l => l.gid == b.gid).Delete();
                            return "微信下单失败";
                        }

                    }
                    else
                    {
                        db.order_details.Where(l => l.order_gid == order_gid).Delete();
                        db.lorder.Where(l => l.gid == order_gid).Delete();
                        return "生成订单失败";
                    }
                }
                else
                {
                    db.order_details.Where(l => l.order_gid == order_gid).Delete();
                    return "生成订单列表失败";
                }
            }
        }

        /// <summary> 
        /// 扣除播放数量
        /// </summary> 
        /// <param name="member_gid">会员gid</param>
        /// <param name="product_gid">产品gid</param>
        /// <param name="type">[1=vip 2=数量 3=次数]</param>
        /// <param name="login_identifier">登陆标识</param>
        /// <param>修改备注</param> 
        /// 2014-5-20 林建生
        public static object videoNumber(Guid member_gid, Guid product_gid, int type, string login_identifier)
        {
            try
            {
                using (EFDB db = new EFDB())
                {
                    var member = db.member.Where(l => l.gid == member_gid).FirstOrDefault();
                    if (member.login_identifier == login_identifier)
                    {
                        if (type == 2)
                        {
                            if (db.video_record.Where(l => l.member_gid == member_gid && l.product_gid == product_gid).Update(l => new video_record { number = l.number - 1, video_time = DateTime.Now }) > 0)
                            {
                                return new AjaxResult("ok");
                            }
                            else
                            {
                                LogManager.WriteLog("扣除观看次数失败", "product_gid=" + product_gid.ToString() + ",member_gid=" + member_gid.ToString());
                            }
                        }
                        else if (type == 3)
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
                                    LogManager.WriteLog("增加观看记录失败", "product_gid=" + product_gid.ToString() + ",member_gid=" + member_gid.ToString());
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
                return new AjaxResult(new { Message = err.Message, Source = err.Source, StackTrace = err.StackTrace });
            }
        }
        #endregion

        #region 文件上传
        /// <summary> 
        /// 多文件上传的操作
        /// </summary> 
        /// <param name="path">上传路径</param>
        /// <param name="files">文件集合</param> 
        public static string UploadFiles(string path, HttpFileCollection files)
        {
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))  //判断当前目录是否存在。
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));  //建立上传文件存放目录。
            }
            foreach (string f in files.AllKeys)
            {
                HttpPostedFile file = files[f];
                string fl = Common.RandStr.CreateOrderNO() + file.FileName;
                file.SaveAs(HttpContext.Current.Server.MapPath(path + fl));
                sb.Append(fl + "$");
            }
            return sb.ToString().TrimEnd('$');
        }

        /// <summary>
        /// 压缩上传
        /// </summary>
        /// <param name="base64str"></param>
        /// <returns></returns>
        public static string jsimg(string path, string base64str)
        {
            string FileName = "";
            try
            {
                if (!string.IsNullOrEmpty(base64str))
                {
                    string imgData = base64str.Split(',')[1];
                    //过滤特殊字符即可   
                    string dummyData = imgData.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                    if (dummyData.Length % 4 > 0)
                    {
                        dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                    }
                    byte[] byteArray = Convert.FromBase64String(dummyData);
                    using (MemoryStream ms = new MemoryStream(byteArray))
                    {
                        Image img = Image.FromStream(ms);
                        path = HttpContext.Current.Server.MapPath(path);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        FileName = Guid.NewGuid() + ".jpg";
                        img.Save(path + FileName);
                    }
                }
            }
            catch { }
            return FileName;
        }

        #endregion

        #region POST文件上传
        /// <summary> 
        /// POST有多文件上传的操作
        /// </summary> 
        /// <param name="path">上传路径</param>
        /// <param name="file">文件</param> 
        public static string UploadFiles(string path, HttpPostedFileBase file)
        {
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))  //判断当前目录是否存在。
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));  //建立上传文件存放目录。
            }
            string filename = Common.RandStr.CreateOrderNO() + Path.GetExtension(file.FileName);
            file.SaveAs(HttpContext.Current.Server.MapPath(path + filename));
            return filename;
        }
        #endregion

        #region POST
        /// <summary>  
        ///   POST请求得到返回数据
        /// </summary>  
        /// <param name="url">调用的Api地址</param>  
        /// <param name="requestJson">表单数据（json格式）</param>  
        /// <returns></returns> 
        public static string Post(string url, string requestJson)
        {
            HttpContent httpContent = new StringContent(requestJson);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return "出错了,StatusCode:" + response.StatusCode.ToString();
            }
        }
        #endregion

        #region 请求时候的IP
        /// <summary>
        /// 请求时候的IP
        /// </summary>
        public static string IP
        {
            get
            {
                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                return ip;
            }
        }
        #endregion

        #region 操作提示
        public static RedirectResult WebRedirect(string title, string url, string msg)
        {
            return new RedirectResult("/ljsheng/msg?msg=" + msg + "&url=" + HttpContext.Current.Server.UrlEncode(url) + "&title=" + title);
        }
        public static RedirectResult Redirect(string title, string url, string msg)
        {
            return new RedirectResult("/home/msg?msg=" + msg + "&url=" + HttpContext.Current.Server.UrlEncode(url) + "&title=" + title);
        }
        public static RedirectResult Redirect(string msg)
        {
            return new RedirectResult("/home/msg?msg=" + msg);
        }
        #endregion

        #region 更新登录信息
        /// <summary>
        /// 用户登录信息CK
        /// </summary>
        /// <param name="gid">用户gid</param>
        /// <returns>返回调用结果</returns>
        /// <para name="result">200 是成功其他失败</para>
        /// <para name="data">对象结果</para>
        /// <remarks>
        /// 2018-08-18 林建生
        /// </remarks>
        public static void UPCKUser(Guid gid)
        {
            using (EFDB db = new EFDB())
            {
                LCookie.DelCookie("linjiansheng");
                LCookie.DelCookie("city");
                //会员登录信息
                var u = db.member.Where(l => l.gid == gid).FirstOrDefault();
                LCookie.AddCookie("linjiansheng", DESRSA.DESEnljsheng(JsonConvert.SerializeObject(new
                {
                    gid = u.gid,
                    account = u.account
                })), 30);
                //设置用户读取数据的城市
                if (string.IsNullOrEmpty(LCookie.GetCookie("city")))
                {
                    LCookie.AddCookie("city", u.city, 30);
                }
            }
        }
        #endregion
    }
}