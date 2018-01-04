using LJSheng.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace LJSheng.Web.Controllers
{
    public class LJSController : BaseController
    {
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            string ck = Common.LCookie.GetCookie("ljsheng");
            if (string.IsNullOrEmpty(ck))
            {
                filterContext.HttpContext.Response.Redirect("/dl.aspx");
            }
            else
            {
                using (EFDB db = new EFDB())
                {
                    JObject json = JsonConvert.DeserializeObject(Common.DESRSA.DESDeljsheng(ck)) as JObject;
                    Guid gid = Guid.Parse(json["gid"].ToString());
                    var b = db.ljsheng.Where(l => l.gid == gid).FirstOrDefault();
                    if (b == null || b.login_identifier != json["login_identifier"].ToString() || b.jurisdiction == "锁定")
                    {
                        filterContext.HttpContext.Response.Redirect("/dl.aspx");
                    }
                }
            }
        }
    }
}