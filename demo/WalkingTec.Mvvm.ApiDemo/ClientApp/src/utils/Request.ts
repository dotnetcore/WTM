/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:37
 * @modify date 2018-09-12 18:52:37
 * @desc [description]
*/
import Rx from "rxjs";
import { message, notification } from "antd";
import NProgress from 'nprogress';
import lodash from 'lodash';
import moment from 'moment';
import GlobalConfig from 'global.config';

interface Preview {
    data: any
    message: string
    status: number
}
/** 缓存 http 请求 */
const CacheHttp = new Map<string, Promise<any>>();
/** 缓存数据 */
const Cache = new Map<string, any>();
// 每30秒清理一次缓存
Rx.Observable.interval(30000).subscribe(obs => {
    CacheHttp.clear();
    Cache.clear();
})
export class Request {
    /**
     * 
     * @param target 替换默认地址前缀
     * @param newMap 替换默认过滤函数
     */
    constructor(target?, public newMap?) {
        if (typeof target === "string") {
            this.target = target;
        }
        this.getHeaders();
    }
    /** 
     * 请求路径前缀
     */
    target = GlobalConfig.target
    /**
     * 获取 认证 token请求头
     */
    getHeaders() {
        return {
            ...GlobalConfig.headers,
            token: GlobalConfig.token.get()
        }
    }

    /**
     * 请求超时设置
     */
    protected timeout = 10000;
    /**
     * 抛出 状态 
     */
    protected catchStatus = [400]
    /**
     * ajax Observable 管道
     * @param Observable 
     */
    protected AjaxObservable(Observable: Rx.Observable<Rx.AjaxResponse>) {
        return new Rx.Observable(sub => {
            // 加载进度条
            this.NProgress();
            Observable
                // 超时时间
                .timeout(this.timeout)
                // 错误处理
                .catch((err) => Rx.Observable.of(err))
                // 过滤请求
                .filter((ajax) => {
                    this.NProgress("done");
                    // 数据 Response 
                    if (ajax instanceof Rx.AjaxResponse) {
                        // 无 响应 数据
                        if (lodash.isNil(ajax.response)) {
                            console.warn("响应体为 NULL", ajax)
                            GlobalConfig.development && notification.warn({
                                message: "响应体为 NULL ",
                                duration: 5,
                                description: `url:${lodash.get(ajax, "request.url")}`
                            })
                            return false
                        }
                        return true
                    }
                    // 错误
                    if (ajax instanceof Rx.AjaxError) {
                        const { response } = ajax;
                        // 返回 业务处理错误
                        if (response && lodash.includes(this.catchStatus, ajax.status)) {
                            if (response.Message && response.Message.length > 0) {
                                response.Message.map(message => notification.error({
                                    message
                                }))
                            }
                            if (response.errors) {
                                notification.error({
                                    key: ajax.request.url,
                                    message: response.traceId,
                                    duration: 5,
                                    description: response.title,
                                });
                            }
                            sub.error(response)
                            return false
                        }
                        sub.error({})
                        notification.error({
                            key: ajax.request.url,
                            message: ajax.status,
                            duration: 5,
                            description: `${ajax.request.method}: ${ajax.request.url}`,
                        });
                        return false
                    }
                    if (ajax instanceof Rx.TimeoutError) {
                        sub.error({})
                        notification.error({
                            key: ajax.name,
                            message: ajax.message,
                            duration: 5,
                        });
                        return false
                    }
                })
                // 数据过滤
                .map((res: Rx.AjaxResponse) => {
                    // 使用传入得 过滤函数
                    if (this.newMap && typeof this.newMap == "function") {
                        return this.newMap(res);
                    }
                    switch (res.status) {
                        case 200:
                            return res.response
                        default:
                            notification.warn({
                                message: res.status,
                                duration: 5,
                                description: `请配置 状态 ${res.status} 处理逻辑`,
                            });
                            break;
                    }
                }).subscribe(obs => {
                    sub.next(obs)
                    sub.complete()
                })
        })
    }
    /**
     * url 参数 注入
     * @param url url
     * @param body body
     * @param emptyBody 清空 body 
     */
    parameterTemplate(url, body, emptyBody = false) {
        if (lodash.isObject(body) && /{([\s\S]+?)}/g.test(url)) {
            if (typeof body == "object") {
                url = lodash.template(url, { interpolate: /{([\s\S]+?)}/g })(body);
            }
            // 清空body
            emptyBody && (body = {});
        }
        return {
            url,
            body
        }
    }
    /**
     *  请求数据 缓存数据
     * @param params 
     */
    async cache(params: {
        url: string,
        body?: { [key: string]: any } | string,
        headers?: Object
        method?: "get" | "post"
    }) {
        params = { url: "", body: {}, method: "get", ...params };
        const key = JSON.stringify({ url: params.url, body: params.body })
        if (Cache.has(key)) {
            return Cache.get(key);
        }
        let promise: Promise<any>// = Http.get(this.address + parmas.address, parmas.params).toPromise();
        if (CacheHttp.has(key)) {
            promise = CacheHttp.get(key);
        } else {
            if (params.method == "get") {
                promise = this.get(params.url, params.body, params.headers).toPromise();
            } else {
                promise = this.post(params.url, params.body, params.headers).toPromise();
            }
            CacheHttp.set(key, promise);
        }
        const data = await promise || [];
        Cache.set(key, data);
        return data;
    }
    /**
     * get
     * @param url 
     * @param body 
     * @param headers 
     */
    get(url: string, body?: { [key: string]: any } | string, headers?: Object): Rx.Observable<any> {
        headers = { ...this.getHeaders(), ...headers };
        const newParams = this.parameterTemplate(url, body, true);
        body = this.formatBody(newParams.body);
        url = this.compatibleUrl(this.target, newParams.url, body as any);
        return this.AjaxObservable(Rx.Observable.ajax.get(url, headers))
    }
    /**
     * post
     * @param url 
     * @param body 
     * @param headers 
     */
    post(url: string, body?: any, headers?: Object): Rx.Observable<any> {
        headers = { ...this.getHeaders(), ...headers };
        const newParams = this.parameterTemplate(url, body);
        body = this.formatBody(newParams.body, "body", headers);
        url = this.compatibleUrl(this.target, newParams.url);
        return this.AjaxObservable(Rx.Observable.ajax.post(url, body, headers))
    }
    /**
     * put
     * @param url 
     * @param body 
     * @param headers 
     */
    put(url: string, body?: any, headers?: Object): Rx.Observable<any> {
        headers = { ...this.getHeaders(), ...headers };
        const newParams = this.parameterTemplate(url, body);
        body = this.formatBody(newParams.body, "body", headers);
        url = this.compatibleUrl(this.target, newParams.url);
        return this.AjaxObservable(Rx.Observable.ajax.put(url, body, headers))
    }
    /**
     * delete
     * @param url 
     * @param body 
     * @param headers 
     */
    delete(url: string, body?: { [key: string]: any } | string, headers?: Object): Rx.Observable<any> {
        headers = { ...this.getHeaders(), ...headers };
        const newParams = this.parameterTemplate(url, body, true);
        body = this.formatBody(newParams.body);
        url = this.compatibleUrl(this.target, newParams.url, body as any);
        return this.AjaxObservable(Rx.Observable.ajax.delete(url, headers))
    }
    /** jsonp 回调 计数 */
    private jsonpCounter = 0;
    /**
     * jsonP
     */
    jsonp(url, body?: { [key: string]: any } | string, callbackKey = 'callback') {
        this.getHeaders();
        body = this.formatBody(body);
        url = this.compatibleUrl(this.target, url, `${body || '?time=' + new Date().getTime()}&${callbackKey}=`);
        return new Rx.Observable(observer => {
            this.jsonpCounter++;
            const key = '_jsonp_callback_' + this.jsonpCounter;
            const script = document.createElement('script');
            script.src = url + key;
            script.onerror = (err) => observer.error(err);
            document.body.appendChild(script);
            window[key] = (response) => {
                // clean up
                script.parentNode.removeChild(script);
                delete window[key];
                // push response downstream
                observer.next(response);
                observer.complete();
            };
        })
    };
    /**
     * url 兼容处理 
     * @param address 前缀
     * @param url url
     * @param endStr 结尾，参数等
     */
    compatibleUrl(address: string, url: string, endStr?: string) {
        endStr = endStr || ''
        if (/^((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+/.test(url)) {
            return `${url}${endStr}`;
        }
        else {
            // address  / 结尾  url / 开头
            const isAddressWith = lodash.endsWith(address, "/")
            const isUrlWith = lodash.startsWith(url, "/")
            // debugger
            if (isAddressWith) {
                if (isUrlWith) {
                    url = lodash.trimStart(url, "/")
                }
            } else {
                if (isUrlWith) {

                } else {
                    url = "/" + url;
                }
            }
        }
        return `${address}${url}${endStr}`
    }
    /**
     * 格式化 参数
     * @param body  参数 
     * @param type  参数传递类型
     * @param headers 请求头 type = body 使用
     */
    formatBody(
        body?: { [key: string]: any } | any[] | string,
        type: "url" | "body" = "url",
        headers?: Object
    ): any {

        if (type === "url") {
            let param = "";
            if (typeof body != 'string') {
                let parlist = [];
                lodash.forEach(body, (value, key) => {
                    if (!lodash.isNil(value) && value != "") {
                        parlist.push(`${key}=${value}`);
                    }
                });
                if (parlist.length) {
                    param = "?" + parlist.join("&");
                }
            } else {
                param = body;
            }
            return param;
        } else {
            // 处理 Content-Type body 类型 
            switch (headers["Content-Type"]) {
                case 'application/json;charset=UTF-8':
                    body = JSON.stringify(body)
                    break;
                case 'application/json':
                    if (lodash.isArray(body)) {
                        body = [...body]
                    }
                    if (lodash.isPlainObject(body)) {
                        body = { ...body as any }
                    }
                    break;
                case 'application/x-www-form-urlencoded':

                    break;
                case 'form-data':
                case 'multipart/form-data':

                    break;
                case null:
                    delete headers["Content-Type"];
                    break;
                default:
                    break;
            }
            return body;
        }
    }
    /** 日志 */
    protected log(url, body, headers) {

    }
    /**
     *  加载进度条
     * @param type 
     */
    protected NProgress(type: 'start' | 'done' = 'start') {
        if (type == "start") {
            NProgress.start();
        } else {
            NProgress.done();
        }
    }
}
export default new Request();
