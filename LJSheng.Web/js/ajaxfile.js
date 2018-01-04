function upload(file) {

    var file1 = document.getElementById(file);

    postFile(file1.files[0], file);
    //if (window.ActiveXObject) { // 判断是否支持ActiveX控件
    //    var fso = new window.ActiveXObject("Scripting.FileSystemObject");
    //    //var f1 = fso.GetFile(path); //"c://test1.txt");
    //    //var ts = f1.OpenAsTextStream(2, true); //文本流
    //   // var ts = file1.files[0].OpenAsTextStream(2, true); //文本流

    //   postFile(file1.files[0]);
    //    //    postFile(frm);
    //} else {
    //    alert("不支持js上传文件！");
    //}
    //test();
}
//function test() {
//    var pnsys = new ActiveXObject("WScript.shell");
//    pn = pnsys.Environment("PROCESS");
//    alert(pn("WINDIR"));
//}

function postFile(data, file) {
    //1.创建异步对象（小浏览器）
    var req = new XMLHttpRequest();

    //2.设置参数
    req.open("post", "/ajax/api.ashx?ff=afile", true);

    //3.设置 请求 报文体 的 编码格式（设置为 表单默认编码格式）
    req.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    // req.setRequestHeader("")
    //4.设置回调函数
    req.onreadystatechange = function () {
        //请求状态readyState=4准备就绪,服务器返回的状态码status=200接收成功
        if (req.readyState == 4 && req.status == 200) {

            if (req.responseText != "上传出错！") {
                changeName(req.responseText, file);
            }
        }
    };

    //4.发送异步请求
    req.send(data);//post传参在此处
}

function changeName(name, file) {
    var file1 = document.getElementById(file);
    var realname = file1.value;

    var req = new XMLHttpRequest();

    //如果名称遇到中文，请在此处转码,然后放入url中
    req.open("get", "/ajax/api.ashx?ff=afile&name=" + name + "&realname=" + realname, true);

    req.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    req.onreadystatechange = function () {
        //请求状态readyState=4准备就绪,服务器返回的状态码status=200接收成功
        if (req.readyState == 4 && req.status == 200) {
            alert(req.responseText + "<br/>" + realname);
        }
    };

    //4.发送异步请求
    req.send();
}