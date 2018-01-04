using System;

namespace LJSheng.Web
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Common.LCookie.DelALLCookie();
            if (Request.QueryString["lx"] == "0")
            {
                Response.Redirect("/dl.aspx");
            }
            else
            {
                Response.Redirect("/home/denglu?lx=" + Request.QueryString["lx"]);
            }
        }
    }
}