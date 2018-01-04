using LJSheng.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace LJSheng.Web.Controllers
{
    public class UController : BaseController
    {
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            string ck = Common.LCookie.GetCookie("linjiansheng");
            if (string.IsNullOrEmpty(ck))
            {
                //如果验证失败，则返回登陆页
                filterContext.HttpContext.Response.Redirect("/home/denglu?lx=1");
            }
            else
            {
                try
                {
                    JObject json = JsonConvert.DeserializeObject(Common.DESRSA.DESDeljsheng(ck)) as JObject;
                    Guid gid = Guid.Parse(json["gid"].ToString());
                    using (EFDB db = new EFDB())
                    {
                        var b = db.member.Where(l => l.gid == gid).FirstOrDefault();
                        if (b != null && b.login_identifier == json["login_identifier"].ToString())
                        {
                            if (string.IsNullOrEmpty(json["grade"].ToString()))
                            {
                                filterContext.HttpContext.Response.Redirect("/home/zcpay?lx=1");
                            }
                        }
                        else
                        {
                            Common.LCookie.DelALLCookie();
                            filterContext.HttpContext.Response.Redirect("/home/denglu?lx=1");
                        }
                    }
                }
                catch
                {
                    Common.LCookie.DelALLCookie();
                    filterContext.HttpContext.Response.Redirect("/home/denglu?lx=1");
                }
            }
        }
    }
}