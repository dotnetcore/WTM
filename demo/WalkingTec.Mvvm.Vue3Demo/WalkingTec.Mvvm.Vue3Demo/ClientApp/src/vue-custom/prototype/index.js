import { actionType } from "@/config/enum";
import Chartist from "chartist";
/**
 * 多语言
 * this 需要指向vue实体
 * @param options
 * @param defaultLabel
 */
var getLanguageByKey = function (options, defaultLabel) {
    if (defaultLabel === void 0) { defaultLabel = ""; }
    if (options.languageKey === undefined) {
        return options.label || defaultLabel;
    }
    else {
        if (options.key) {
            return this.$t("" + (options.languageKey ? options.languageKey + "." : "") + options.key);
        }
        else {
            return defaultLabel;
        }
    }
};
export default [
    { key: "actionType", value: actionType },
    { key: "Chartist", value: Chartist },
    { key: "getLanguageByKey", value: getLanguageByKey }
];
//# sourceMappingURL=index.js.map