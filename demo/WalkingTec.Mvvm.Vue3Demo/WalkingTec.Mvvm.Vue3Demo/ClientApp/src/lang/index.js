import { __assign } from "tslib";
import Vue from "vue";
import VueI18n from "vue-i18n";
import { getLanguage } from "@/util/cookie";
// element-ui built-in lang
import elementEnLocale from "element-ui/lib/locale/lang/en";
import elementZhLocale from "element-ui/lib/locale/lang/zh-CN";
import elementEsLocale from "element-ui/lib/locale/lang/es";
import elementJaLocale from "element-ui/lib/locale/lang/ja";
// User defined lang
import enLocale from "./en";
import zhLocale from "./zh";
import esLocale from "./es";
import jaLocale from "./ja";
Vue.use(VueI18n);
var messages = {
    en: __assign(__assign({}, enLocale), elementEnLocale),
    zh: __assign(__assign({}, zhLocale), elementZhLocale),
    es: __assign(__assign({}, esLocale), elementEsLocale),
    ja: __assign(__assign({}, jaLocale), elementJaLocale)
};
var missing = function (locale, key, vm) {
    return key.substr(key.lastIndexOf(".") + 1);
};
export var getLocale = function () {
    var cookieLanguage = getLanguage();
    if (cookieLanguage) {
        return cookieLanguage;
    }
    var language = navigator.language.toLowerCase();
    var locales = Object.keys(messages);
    for (var _i = 0, locales_1 = locales; _i < locales_1.length; _i++) {
        var locale = locales_1[_i];
        if (language.indexOf(locale) > -1) {
            return locale;
        }
    }
    return "zh";
};
var i18n = new VueI18n({
    locale: getLocale(),
    messages: messages,
    missing: missing
});
export default i18n;
//# sourceMappingURL=index.js.map