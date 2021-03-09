import date from "@/util/date";
// 时间格式
var formatTime = function (value, customFormat, isMsec) {
    if (customFormat === void 0) { customFormat = "yyyy-MM-dd hh:mm:ss"; }
    if (isMsec === void 0) { isMsec = true; }
    // customFormat 要展示的时间格式
    // isMsec----传入的value值是否是毫秒
    value = isMsec ? value : value * 1000;
    return date.toFormat(value, customFormat);
};
export default formatTime;
//# sourceMappingURL=index.js.map