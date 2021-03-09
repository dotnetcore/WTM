// 大写首字母
var firstUpperCase = function (str) {
    // 转换字符串首字母为大写，剩下为小写。
    //_.capitalize([string=''])
    return str.replace(/( |^)[a-z]/g, function (L) { return L.toUpperCase(); });
};
// params 返回 string
var urlStringify = function (params) {
    var list = Object.keys(params)
        .map(function (item) {
        return item + "=" + params[item];
    })
        .join("&");
    return list;
};
// params 返回 string
var strStringify = function (params, seg, segjoin) {
    var list = Object.keys(params)
        .map(function (item) {
        return item + (seg || "=") + params[item];
    })
        .join(segjoin || "&");
    return list;
};
// 图片格式data
var getBase64Str = function (obj) {
    var u8arr = new Uint8Array(obj).reduce(function (data, byte) { return data + String.fromCharCode(byte); }, "");
    return "data:image/png;base64," + btoa(u8arr);
};
// 图片转换Blob
var getDownLoadUrl = function (base64str) {
    var parts = base64str.split(";base64,");
    var contentType = parts[0].split(":")[1];
    var raw = window.atob(parts[1]);
    var rawLength = raw.length;
    var uInt8Array = new Uint8Array(rawLength);
    for (var i = 0; i < rawLength; ++i) {
        uInt8Array[i] = raw.charCodeAt(i);
    }
    var blob = new Blob([uInt8Array], { type: contentType });
    return URL.createObjectURL(blob);
};
var listToString = function (list, key) {
    if (list === void 0) { list = []; }
    return list.map(function (item) {
        return item[key];
    });
};
var exportXlsx = function (data, name) {
    var b = new Blob([data], {
        type: "application/vnd.ms-excel;charset=utf-8"
    });
    var url = URL.createObjectURL(b);
    var link = document.createElement("a");
    link.download = name + ".xls";
    link.href = url;
    link.click();
    window.URL.revokeObjectURL(url); //释放掉blob对象
};
/**
 * 替换模版{key}
 */
var paramTemplate = function (str, param) {
    if (_.isObject(param) &&
        typeof param === "object" &&
        /{([\s\S]+?)}/g.test(str)) {
        str = _.template(str, { interpolate: /{([\s\S]+?)}/g })(param);
    }
    return str;
};
export { firstUpperCase, urlStringify, strStringify, getBase64Str, getDownLoadUrl, listToString, exportXlsx, paramTemplate };
//# sourceMappingURL=string.js.map