import Cookies from "js-cookie";
import config from "@/config/index";
// App
var sidebarStatusKey = "sidebar_status";
export var getSidebarStatus = function () { return Cookies.get(sidebarStatusKey); };
export var setSidebarStatus = function (sidebarStatus) {
    return Cookies.set(sidebarStatusKey, sidebarStatus, { expires: config.cookiesExpires });
};
var languageKey = "language";
export var getLanguage = function () { return Cookies.get(languageKey); };
export var setLanguage = function (language) {
    return Cookies.set(languageKey, language, { expires: config.cookiesExpires });
};
var sizeKey = "size";
export var getSize = function () { return Cookies.get(sizeKey); };
export var setSize = function (size) { return Cookies.set(sizeKey, size, { expires: config.cookiesExpires }); };
// User
var tokenKey = config.tokenKey;
export var getToken = function () { return Cookies.get(tokenKey); };
export var setToken = function (token) { return Cookies.set(tokenKey, token, { expires: config.cookiesExpires }); };
export var removeToken = function () { return Cookies.remove(tokenKey); };
var settingsKey = "settings";
export var getSettings = function () { return Cookies.getJSON(settingsKey); };
export var setSettings = function (value) { return Cookies.set(settingsKey, value, { expires: config.cookiesExpires }); };
/**
 * set cookie
 * @param key
 * @param value
 */
export var setCookie = function (key, value) {
    Cookies.set(key, value, { expires: config.cookiesExpires });
};
/**
 * get cookie
 * @param name
 */
export var getCookie = function (name) {
    return Cookies.get(name);
};
//# sourceMappingURL=cookie.js.map