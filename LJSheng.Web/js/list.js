layui.use(['layer', 'form', 'laydate', 'element'], function () {
    var layer = layui.layer;
});
//获取要请求的控制器
var controller = window.location.pathname.split('/');
controller = controller[controller.length - 1].replace("list", "");
//分页
GetList(1,10);
function GetList(pageindex, pagesize) {
    var json = "{";
    json += "pageindex:" + pageindex;
    json += ",pagesize:" + pagesize;
    try {
        var div = document.getElementById("ss");
        var input = div.getElementsByTagName("input");
        var select = div.getElementsByTagName("select");
        for (var i = 0; i < input.length; i++) {
            json += "," + input[i].name + ":'" + input[i].value + "'";
        }
        for (var i = 0; i < select.length; i++) {
            json += "," + select[i].name + ":'" + select[i].value + "'";
        }
    }
    catch (error) { }
    json += "}";
    //console.log(JSON.stringify(json));
    //重新加载数据
    $.ajax({
        url: "/ljsheng/" + controller,
        data: encodeURI(json),
        type: "post",
        cache: false,
        timeout: 8000,
        dataType: "json",
        ContentType: "application/json; charset=utf-8",
        error: function () { layer.msg("加载失败,请刷新试下!"); },
        beforeSend: function () { },
        success: function (data) {
            var html = template('tpl', data.data);
            document.getElementById('tbody').innerHTML = html;
            layui.use(['laypage'], function () {
                var laypage = layui.laypage;
                laypage.render({
                    elem: 'page'
                    , count: data.data.count
                    , limit: pagesize
                    , limits: [5, 10, 15, 20, 50]
                    , hash: 'pageindex'
                    , curr: location.hash.replace('#!pageindex=', '')
                    , first: '首页'
                    , last: '尾页'
                    , layout: ['count', 'prev', 'page', 'next', 'limit', 'skip']
                    , jump: function (obj, first) {
                        if (!first) {
                            GetList(obj.curr, obj.limit);
                        }
                    }
                });
            });
        },
    });
}
//删除数据
function Delete(gid) {
    layer.confirm('你确定要执行此操作吗？', {
        btn: ['确定', '取消'] //按钮
    }, function () {
        $.ajax({
            url: "/ljsheng/" + controller + "delete",
            data: "gid=" + gid,
            type: "post",
            cache: false,
            timeout: 8000,
            dataType: "json",
            ContentType: "application/json; charset=utf-8",
            error: function () { layer.msg("请求超时"); },
            beforeSend: function () { },
            success: function (data) {
                if (data.result == 200) {
                    $("#" + gid).hide(5);
                    layer.msg(data.data, { icon: 1 });
                }
                else {
                    layer.msg(data.data);
                }
            },
        });
    });
}
//重设密码
function PWD(gid) {
    layer.confirm('你确定要执行此操作吗？', {
        btn: ['确定', '取消'] //按钮
    }, function () {
        $.ajax({
            url: "/ljsheng/" + controller + "pwd",
            data: "gid=" + gid,
            type: "post",
            cache: false,
            timeout: 8000,
            dataType: "json",
            ContentType: "application/json; charset=utf-8",
            error: function () { layer.msg("请求超时"); },
            beforeSend: function () { },
            success: function (data) {
                if (data.result == 200) {
                    layer.msg(data.data, { icon: 1 });
                }
                else {
                    layer.msg(data.data);
                }
            },
        });
    });
}
//弹层新URL
function Show(title, param, url) {
    url = url != "" ? url : controller + 'au';
    layer.open({
        type: 2,
        title: title,
        shadeClose: true,
        maxmin: true, //开启最大化最小化按钮
        shade: 0.8,
        area: ['88%', '88%'],
        content: '/ljsheng/' + url + '?' + param
    });
}
//显示图片
function ShowPicture(gid) {
    layer.ready(function () {
        layer.photos({
            photos: '#' + gid
            , anim: 5
        });
    });
}
