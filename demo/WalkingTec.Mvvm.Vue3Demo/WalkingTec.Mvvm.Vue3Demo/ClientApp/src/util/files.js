/**
 * 创建二进制文件
 * @param response
 */
var createBlob = function (response) {
    var blob = response.data;
    var fileName = response.headers['content-disposition'].match(/filename=(.*?);/)[1];
    var a = document.createElement("a");
    var downUrl = window.URL.createObjectURL(blob);
    a.href = downUrl;
    a.download = fileName;
    a.addEventListener("click", function () {
        setTimeout(function () {
            window.URL.revokeObjectURL(downUrl);
        }, 1000);
    }, false);
    a.click();
};
export { createBlob };
//# sourceMappingURL=files.js.map