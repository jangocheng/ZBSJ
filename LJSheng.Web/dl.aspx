<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dl.aspx.cs" Inherits="LJSheng.Web.Views.ljsheng.dl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" /> 
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>后台登录</title>
    <link href="/css/ljsheng/global.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        html {
            height: 100%;
            overflow: hidden;
        }

        body {
            background: #303f52 url(/images/ljsheng/log.jpg) center -50px;
            height: 100%;
        }
    </style>
    <script type="text/javascript">
        //创建当前日期＋随机数
        function getURLDateTimePar() {
            var today = new Date();
            var intYear = "" + today.getYear() + "";
            var intMonth = "" + today.getMonth() + 1 + "";
            var intDay = "" + today.getDate() + "";
            var intHours = "" + today.getHours() + "";
            var intMinutes = "" + today.getMinutes() + "";
            var intSeconds = "" + today.getSeconds() + "";

            var RandomNumber = getRandomNumber();
            RandomNumber = "" + RandomNumber + "";
            return intYear + intMonth + intDay + intHours + intMinutes + intSeconds + RandomNumber;
        }
        //生成随机数
        function getRandomNumber() {
            return parseInt(Math.random() * (10000 - 1 + 1) + 1);
        }
        function checkData() {
            if (document.getElementById("txtusername").value == "") {
                alert("请输入用户名");
                return false;
            }
            if (document.getElementById("txtpassword").value == "") {
                alert("请输入密码");
                return false;
            }
            if (document.getElementById("txtcode").value == "") {
                alert("请输入验证码");
                return false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="login">
        <ul class="log">
            <li>
                <input type="text" id="txtusername" runat="server" /></li>
            <li>
                <input type="password" id="txtpassword" runat="server" /></li>
            <li class="yzm">
                <input type="text" id="txtcode" runat="server" /><img alt="点击更换" width="60" height="25" src="/plugins/VerifyCode.aspx" onclick="this.src=this.src + '?' + getURLDateTimePar();" id="imgVerify" title="看不清？请点击图片更换"  style="cursor: pointer;" /></li>
            <li class="logbtn">
                <asp:Button ID="Login" runat="server" Text="" OnClientClick="return checkData();" OnClick="Login_Click"/></li>
        </ul>
    </div>
    </form>
</body>
</html>
