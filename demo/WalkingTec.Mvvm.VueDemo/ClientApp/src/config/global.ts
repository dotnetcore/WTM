import cache from "@/util/cache";
import config from "@/config/index";

type ResolverFn = () => void;
type AnyOrResolverFn = any | ResolverFn;

const fn = () => {};
const globalParams = cache.getCookieJson(config.globalKey) || {};
/**
 * 属性
 */
const globalConfig = {
    dialog: globalParams["dialog"] || "弹框",
    tabs: globalParams["tabs"] || false
};
/**
 * 执行
 */
const globalFn = {
    dialog: fn,
    tabs: fn
};

function setGlobal(key: string, valOrFn: AnyOrResolverFn) {
    if (typeof valOrFn === "function") {
        globalFn[key] = valOrFn;
    } else {
        globalFn[key](valOrFn);
        cache.setCookieJson(config.globalKey, globalConfig);
    }
}
export { globalConfig, setGlobal };
