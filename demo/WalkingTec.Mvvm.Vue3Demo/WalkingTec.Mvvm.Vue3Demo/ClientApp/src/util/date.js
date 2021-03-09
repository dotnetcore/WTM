export default {
    /**
     * 日期格式化
     * @param phone
     */
    toFormat: function (dateStr, fmt) {
        if (fmt === void 0) { fmt = "yyyy-MM-dd hh:mm:ss"; }
        if (!dateStr || isNaN(dateStr)) {
            return "";
        }
        var date = new Date(dateStr);
        //author: meizz
        var o = {
            "M+": date.getMonth() + 1,
            "d+": date.getDate(),
            "h+": date.getHours(),
            "m+": date.getMinutes(),
            "s+": date.getSeconds(),
            "q+": Math.floor((date.getMonth() + 3) / 3),
            S: date.getMilliseconds() //毫秒
        };
        if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(fmt)) {
                fmt = fmt.replace(RegExp.$1, RegExp.$1.length == 1
                    ? o[k]
                    : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return fmt;
    }
};
//# sourceMappingURL=date.js.map