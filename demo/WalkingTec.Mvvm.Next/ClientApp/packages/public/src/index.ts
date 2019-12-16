import { configure } from 'mobx';
/**
 * https://cn.mobx.js.org/refguide/api.html
 * 启用严格模式
 */
configure({ enforceActions: "observed" });
export * from "./entities/user";
export * from "./entities/time";
export * from "./entities/page";
export * from "./utils/helps";
export * from "./utils/regulars";

