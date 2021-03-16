import { BindAll } from 'lodash-decorators';
import lodash from 'lodash';
import Bowser from 'bowser';
@BindAll()
export class WtmConfig {
    constructor() {
        this.onInspectVersion();
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
     * api 地址
     * @memberof XTGlobal
     */
    readonly target = lodash.get(window, '__xt__env.target', process.env.target);
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
    readonly version = lodash.get(window, '__xt__env.version', process.env.version);
    /**
     * 构建时间戳
     * @memberof XTGlobal
     */
    readonly timestamp = lodash.get(window, '__xt__env.timestamp', process.env.timestamp);
    /**
     * Node env
     * @memberof XTGlobal
     */
    readonly NODE_ENV: typeof process.env.NODE_ENV = lodash.get(window, '__xt__env.NODE_ENV', process.env.NODE_ENV);
    /**
     * 环境
     * @memberof XTGlobal
     */
    readonly DEPLOY_ENV: typeof process.env.DEPLOY_ENV = lodash.get(window, '__xt__env.DEPLOY_ENV', process.env.DEPLOY_ENV);
    /**
     * Android
     * @readonly
     * @memberof XTGlobal
     */
    get isAndroid() {
        return lodash.eq(this.userAgent.os.name, Bowser.OS_MAP.Android)
    }
    /**
     * iOS
     * @readonly
     * @memberof XTGlobal
     */
    get isiOS() {
        return lodash.eq(this.userAgent.os.name, Bowser.OS_MAP.iOS)
    }
    /**
     * 本地 dev
     * @memberof XTGlobal
     */
    get dev() {
        return this.NODE_ENV === 'development'
    }
    /**
     *生产环境
     * @memberof XTGlobal
     */
    get production() {
        return this.DEPLOY_ENV === 'pro'
    }
    /**
     * 微信浏览器
     * @readonly
     * @memberof XTGlobal
     */
    get WechatBowser() {
        return lodash.eq(this.userAgent.browser.name, Bowser.BROWSER_MAP.wechat)
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
