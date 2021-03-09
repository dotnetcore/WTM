export var isValidUsername = function (str) {
    return ["admin", "editor"].indexOf(str.trim()) >= 0;
};
export var isExternal = function (path) {
    return /^((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+/.test(path);
};
export var isArray = function (arg) {
    if (typeof Array.isArray === "undefined") {
        return Object.prototype.toString.call(arg) === "[object Array]";
    }
    return Array.isArray(arg);
};
export var isValidURL = function (url) {
    var reg = /^(https?|ftp):\/\/([a-zA-Z0-9.-]+(:[a-zA-Z0-9.&%$-]+)*@)*((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])){3}|([a-zA-Z0-9-]+\.)*[a-zA-Z0-9-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(:[0-9]+)*(\/($|[a-zA-Z0-9.,?'\\+&%$#=~_-]+))*$/;
    return reg.test(url);
};
//# sourceMappingURL=validate.js.map