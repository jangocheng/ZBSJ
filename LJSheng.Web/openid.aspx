<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="openid.aspx.cs" Inherits="LJSheng.Web.openid" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, minimum-scale=1.0, maximum-scale=1.0" />
    <title>正在建立安全连接</title>
    <script>
        function openid()
        {
            var ua = navigator.userAgent.toLowerCase();
            if (ua.match(/MicroMessenger/i) == "micromessenger") {
                self.location = "/ajax/api.ashx?ff=oauth2&tourl=" + getQueryString("tourl");
            }
            else {
                alert("请在微信里打开!");
                self.location = "/home/index";
                //self.location = getQueryString("tourl").replace("$", "?").replace("@", "&");
            }
        }
        //获取URL参数
        function getQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return r[2]; return null;
        }
    </script>
</head>
<body onload="openid();">
    <form id="form1" runat="server">
    <div style="height:100%;text-align:center;margin-top:20%;">
        正在建立安全连接...
        <br /><br />
        <a href="/home/index">如果没有反映点这里返回主页</a>
    </div>
    </form>
</body>
</html>
