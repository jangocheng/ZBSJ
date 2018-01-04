//  哪个请求
var url;
var data;
var id;
// 参数，最大高度
var MAX_HEIGHT = 250;
// 渲染
function render(src) {
    // 创建一个 Image 对象
    var image = new Image();
    // 绑定 load 事件处理器，加载完成后执行
    image.onload = function () {
        // 获取 canvas DOM 对象
        var canvas = document.createElement("canvas");
        // 如果高度超标
        if (image.height > MAX_HEIGHT) {
            // 宽度等比例缩放 *=
            image.width *= MAX_HEIGHT / image.height;
            image.height = MAX_HEIGHT;
        }
        // 获取 canvas的 2d 环境对象,
        // 可以理解Context是管理员，canvas是房子
        var ctx = canvas.getContext("2d");
        // canvas清屏
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        // 重置canvas宽高
        canvas.width = image.width;
        canvas.height = image.height;
        // 将图像绘制到canvas上
        ctx.drawImage(image, 0, 0, image.width, image.height);
        // !!! 注意，image 没有加入到 dom之中
        //document.getElementById('img').src = canvas.toDataURL("image/png");
        var blob = dataURLtoBlob(canvas.toDataURL("image/png"));
        var fd = new FormData();
        fd.append("image", blob, "image.png");
        imgCompressUpload(canvas.toDataURL("image/png"));
    };
    // 设置src属性，浏览器会自动加载。
    // 记住必须先绑定事件，才能设置src属性，否则会出同步问题。
    image.src = src;
};
// 加载 图像文件(url路径)
function loadImage(src) {
    // 过滤掉 非 image 类型的文件
    if (!src.type.match(/image.*/)) {
        if (window.console) {
            console.log("选择的文件类型不是图片: ", src.type);
        } else {
            window.confirm("只能选择图片文件");
        }
        return;
    }
    // 创建 FileReader 对象 并调用 render 函数来完成渲染.
    var reader = new FileReader();
    // 绑定load事件自动回调函数
    reader.onload = function (e) {
        // 调用前面的 render 函数
        render(e.target.result);
    };
    // 读取文件内容
    reader.readAsDataURL(src);
};
function loadImageFile(file) {
    if (document.getElementById(file).files.length === 0) { imgCompressUpload(""); }
    var oFile = document.getElementById(file).files[0];
    loadImage(oFile)
}
function dataURLtoBlob(dataurl) {
    var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new Blob([u8arr], { type: mime });
}
function imgCompressUpload(formData) {
    $.ajax({
        url: url,
        data: { base64Data: formData, data },
        type: "post",
        dataType: "json",
        ContentType: "application/json; charset=utf-8",
        beforeSend: function () { $("#" + id + "bt").text("请求中..."); },
        error: function () { alert("AJAX Error"); },
        success: function (data) {
            $("#" + id + "bt").text(data.data.msg);
            if (data.result == 200) {
                $("#" + id + "img").attr("src", data.data.file);
            }
        }
    })
}