import { actionType } from "@/config/enum";
import Chartist from "chartist";

interface IOptions {
  languageKey?: string;
  key?: string;
  label?: string;
}
/**
 * 多语言
 * this 需要指向vue实体
 * @param options
 * @param defaultLabel
 */
const getLanguageByKey = function(options: IOptions, defaultLabel: string = "") {
  if (options.languageKey === undefined) {
      return options.label || defaultLabel;
  } else {
      if (options.key) {
          return this.$t(`${options.languageKey ? options.languageKey + "." : ""}${options.key}`);
      } else {
          return defaultLabel;
      }
  }
};

export default [
  { key: "actionType", value: actionType },
  { key: "Chartist", value: Chartist },
  { key: "getLanguageByKey", value: getLanguageByKey }
];
