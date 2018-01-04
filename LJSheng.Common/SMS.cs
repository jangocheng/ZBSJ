//-----------------------------------------------------------
// 描    述:获取路径下的所有文件
// 修改标识: 林建生 1984-02-04
// 修改内容: LJSheng 项目通用类
//-----------------------------------------------------------
using System;
using System.Net;
using System.Text;
using System.IO;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace LJSheng.Common
{
    public static class SMS
    {
        /// <summary>
        /// 发送发短息
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="msg">短信模版</param>
        /// <param name="json">参数</param>
        public static string postsend(string phone, string msg, string json)
        {
            ITopClient client = new DefaultTopClient("http://gw.api.taobao.com/router/rest", "23334148", "521968d60abc9ca99aa44018248c2788", "json");
            AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
            req.Extend = "123456";
            req.SmsType = "normal";
            req.SmsFreeSignName = "ING少儿英语";
            req.SmsParam = json;
            req.RecNum = phone;
            req.SmsTemplateCode = msg;
            AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
            return rsp.Body;
        }

        static string requestUrl = @"http://admin.sms9.net/houtai/sms.php";
        static string corpId = "13812";
        static string password = "20100508pgs";
        static string channelid = "16613";
        /// <summary>
        /// 发送发短息
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="msg">短信内容</param>
        /// <returns>
        /// 如果发送成功，则返回：success:本次发送短信编号
        /// 如果发送失败，则返回：error:错误描述
        /// 错误描述:传递参数错误=-1 
        /// 用户id或密码错误=-2 
        /// 通道id错误=-3 
        /// 手机号码错误=-4 
        /// 短信内容错误=-5 
        /// 余额不足错误=-6 
        /// 绑定ip错误=-7 
        /// 未带签名=-8 
        /// 签名字数不对=-9 
        /// 通道暂停=-10 
        /// 该时间禁止发送=-11 
        /// 时间戳错误=-12 
        /// 编码异常=-13 
        /// 发送被限制=-14(由于网关限制，同一个手机号不能反复发送过多短信，验证码一分钟只能下发一条一个小时三条) 
        /// 短信内容不正确=-15(语音验证码必须为4-8位的数字)
        /// </returns>
        public static string postsend(string phone, string msg)
        {
            try
            {
                string unixtime = LJSheng.Common.LCommon.TimeToUNIX(DateTime.Now);
                string MD5password = LJSheng.Common.MD5.GetMD5(password + "_" + unixtime + "_topsky").ToLower();
                string url = string.Format("cpid={0}&password={1}&channelid={2}&tele={3}&msg={4}&timestamp={5}", corpId, MD5password, channelid, phone, msg, unixtime);
                string formUrl = requestUrl;
                string formData = url;//提交的参数

                //注意提交的编码 这边是需要改变的 这边默认的是Default：系统当前编码
                byte[] postData = Encoding.GetEncoding("GBK").GetBytes(formData);

                // 设置提交的相关参数 
                HttpWebRequest request = WebRequest.Create(formUrl) as HttpWebRequest;
                Encoding myEncoding = Encoding.GetEncoding("GBK");
                request.Method = "POST";
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR  3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.ContentLength = postData.Length;

                // 提交请求数据 
                System.IO.Stream outputStream = request.GetRequestStream();
                outputStream.Write(postData, 0, postData.Length);
                outputStream.Close();

                HttpWebResponse response;
                Stream responseStream;
                StreamReader reader;
                string srcString;
                response = request.GetResponse() as HttpWebResponse;
                responseStream = response.GetResponseStream();
                reader = new System.IO.StreamReader(responseStream, Encoding.GetEncoding("GBK"));
                srcString = reader.ReadToEnd();
                string result = srcString;   //返回值赋值
                reader.Close();
                return result;
            }
            catch (Exception err)
            {
                return "error:" + err;
            }
        }
    }
}
