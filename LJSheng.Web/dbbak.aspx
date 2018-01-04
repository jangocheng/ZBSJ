<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dbbak.aspx.cs" Inherits="LJSheng.Web.dbbak" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>数据库备份</title>
    <link href="~/plugins/layui/css/layui.css" rel="stylesheet" />
</head>
<body style="margin-left:10px;margin-top:10px;">
    <form id="form1" runat="server">
        <div class="layui-form-item">
            <div class="layui-inline">
                <div class="layui-input-inline">
                    <asp:Button runat="server" ID="bf" CssClass="layui-btn" Text="立即备份" OnClick="bf_Click" />
                </div>
            </div>
        </div>
        <div class="layui-form">
            <asp:ListView ID="LVljsheng" runat="server" OnItemCommand="LVljsheng_ItemCommand">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="name" runat="server" Text='<%# Eval("name") %>' />
                        </td>
                        <td>
                            <%#Eval("CreationTime")%>
                        </td>
                        <td>
                            <div class="layui-btn-group">
                                <asp:Button ID="del" runat="server" Text="删除" CssClass="layui-btn" CommandName="del" OnClientClick="return confirm('确定删除吗？');" />
                                <a class="layui-btn" target="_blank" href="/uploadfiles/dbbak/<%#Eval("name")%>">下载备份</a>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div>当前没有符合条件的数据</div>
                </EmptyDataTemplate>
                <LayoutTemplate>
                    <table class="layui-table">
                        <thead>
                            <tr runat="server" id="itemPlaceholderContainer">
                                <th>备份文件名</th>
                                <th>备份时间</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceholder" runat="server">
                            </tr>
                        </tbody>
                    </table>
                </LayoutTemplate>
            </asp:ListView>
        </div>
    </form>
</body>
</html>
