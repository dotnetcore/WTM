import Bowser from 'bowser';
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import { configure } from "mobx";
configure({ enforceActions: "observed" });
@BindAll()
export class WtmConfig {
    constructor() {
        // this.onInspectVersion();
        console.group('WtmGlobal')
        console.log(this)
        console.groupEnd()
    }
    /**
     * 显示详情的 key
     * @memberof WtmGlobal
     */
    readonly detailsVisible = "details"
    /**
     * 模态框类型
     * @type {("modal" | "drawer")}
     * @memberof WtmConfig
     */
    readonly modalType: "modal" | "drawer" = "modal"
    /**
     * api 地址
     * @memberof XTGlobal
     */
    readonly target = ''// lodash.get(window, '__xt__env.target', process.env.target);
    /**
     * 环境设备信息
     * @memberof XTGlobal
     */
    readonly userAgent = Bowser.parse(window.navigator.userAgent)
    /**
     *   localStorage  前缀 
     * @memberof XTGlobal
     */
    readonly localStorageStartsWith = "__wtm_";
    /**
     * 版本信息
     * @memberof XTGlobal
     */
    readonly version = process.env.version;
    /**
     * 构建时间戳
     * @memberof XTGlobal
     */
    readonly timestamp = process.env.timestamp;
    /**
     * Node env
     * @memberof XTGlobal
     */
    readonly NODE_ENV = process.env.NODE_ENV;
    /**
     * 环境
     * @memberof XTGlobal
     */
    get production() {
        return this.NODE_ENV === 'production'
    }
    /**
     * 鉴权开关 production 开启
     * @readonly
     * @memberof WtmConfig
     */
    get authority() {
        if (this.production) {
            return true
        }
        return false
    }
    /**
     * 检查版本信息 
     */
    onInspectVersion() {
        if (window && window.localStorage) {
            const version = window.localStorage.getItem('version');
            // 清理 版本 不统一缓存
            if (!lodash.eq(version, this.version)) {
                lodash.mapKeys(window.localStorage, (value, key: any) => {
                    if (lodash.startsWith(key, this.localStorageStartsWith)) {
                        window.localStorage.removeItem(key)
                    }
                })
            }
            window.localStorage.setItem('version', this.version)
        }
    }
}
export default new WtmConfig();
