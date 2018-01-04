using LJSheng.Common;
using LJSheng.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace LJSheng.Web
{
    public partial class dbbak : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ck = LCookie.GetCookie("ljsheng");
                if (string.IsNullOrEmpty(ck))
                {
                    Response.Redirect("/dl.aspx");
                }
                else
                {
                    JObject json = JsonConvert.DeserializeObject(Common.DESRSA.DESDeljsheng(ck)) as JObject;
                    Guid gid = Guid.Parse(json["gid"].ToString());
                    using (EFDB db = new EFDB())
                    {
                        var b = db.ljsheng.Where(l => l.gid == gid).FirstOrDefault();
                        if (b == null || b.login_identifier != json["login_identifier"].ToString() || json["jurisdiction"].ToString() == "锁定")
                        {
                            Response.Redirect("/dl.aspx");
                        }
                    }
                    Bind();
                }
            }
        }
        //数据绑定
        private void Bind()
        {
            string directory = System.Web.HttpContext.Current.Server.MapPath("/uploadfiles/dbbak/");
            List<FileInfo> files = new List<FileInfo>();
            ///获取文件列表信息  
            foreach (var file in Directory.GetFiles(directory))
            {
                files.Add(new FileInfo(file));
            }
            ///查询文件列表信息  
            var filevalues = from file in files
                             where file.Extension == ".bak"
                             orderby file.CreationTime descending
                             select file;
            LVljsheng.DataSource = filevalues;
            LVljsheng.DataBind();
        }

        //listviet操作
        protected void LVljsheng_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "del":
                    string file = System.Web.HttpContext.Current.Server.MapPath("/uploadfiles/dbbak/" + ((Label)e.Item.FindControl("name")).Text);
                    switch (e.CommandName)
                    {
                        case "del":
                            if (File.Exists(file))
                                File.Delete(file);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            Bind();
        }

        protected void bf_Click(object sender, EventArgs e)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("/uploadfiles/dbbak/");
            string name = "dbbackup_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            DbHelperSQL.ExecuteSql("BACKUP DATABASE [" + Request.QueryString["db"] + "] TO  DISK = N'" + path + name + ".bak' WITH  RETAINDAYS = 7, NOFORMAT, NOINIT,  NAME = N'" + name + "', SKIP, REWIND, NOUNLOAD,  STATS = 10");
            JS.AlertAndRedirect("备份成功", "dbbak.aspx", this);
        }
    }
}