/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:37
 * @modify date 2018-09-12 18:52:37
 * @desc [description]
*/
import { notification } from "antd";
import GlobalConfig from 'global.config';
import lodash from 'lodash';
import NProgress from 'nprogress';
import { interval, Observable, of, TimeoutError } from "rxjs";
import { ajax, AjaxError, AjaxResponse, AjaxRequest } from "rxjs/ajax";
import { catchError, filter, map, timeout } from "rxjs/operators";
import { getLocalesValue } from "locale";
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
interval(5000).subscribe(obs => {
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
        // this.getHeaders();
    }
    /** 
     * 请求路径前缀
     */
    target = "/"//GlobalConfig.target
    /**
     * 获取 认证 token请求头
     */
    getHeaders() {
        return GlobalConfig.getHeaders()
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
    protected AjaxObservable(Obs: Observable<AjaxResponse>) {
        return new Observable<any>(sub => {
            // 加载进度条
            Request.NProgress();
            Obs.pipe(
                // 超时时间
                timeout(this.timeout),
                // 错误处理
                catchError((err) => of(err)),
                // 过滤请求
                filter((ajax) => {
                    Request.NProgress("done");
                    // 数据 Response 
                    if (ajax instanceof AjaxResponse) {
                        // 无 响应 数据
                        //if (lodash.isNil(ajax.response)) {
                        //    console.warn(`未解析到 response ${ajax.request.url}`, ajax)
                        //    GlobalConfig.development && notification.warn({
                        //        message: "未解析到 response ",
                        //        duration: 5,
                        //        description: `url:${lodash.get(ajax, "request.url")}`
                        //    })
                        //    sub.error({})
                        //    return false
                        //}
                        return true
                    }
                    // 错误
                    if (ajax instanceof AjaxError) {
                        const { response } = ajax;
                        // 返回 业务处理错误
                        if (response && lodash.includes(this.catchStatus, ajax.status)) {
                            if (response.Message && response.Message.length > 0) {
                                lodash.compact(response.Message).map(message => notification.error({
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
                            message: getLocalesValue(`tips.status.${ajax.status}`, `${ajax.request.method}: ${ajax.request.url}`), //ajax.status,
                            duration: 5,
                            // description: `${ajax.request.method}: ${ajax.request.url}`,
                        });
                        return false
                    }
                    if (ajax instanceof TimeoutError) {
                        sub.error({})
                        notification.error({
                            key: ajax.name,
                            message: ajax.message,
                            duration: 5,
                        });
                        return false
                    }
                }),
                // 数据过滤
                map((res: AjaxResponse) => {
                    // 使用传入得 过滤函数
                    if (this.newMap && typeof this.newMap == "function") {
                        return this.newMap(res);
                    }
                    switch (res.status) {
                        case 200:
                            return res.response || true
                        default:
                            notification.warn({
                                message: getLocalesValue(`tips.status.${res.status}`, `请配置 状态 ${res.status} 处理逻辑`),
                                duration: 5,
                                // description: `请配置 状态 ${res.status} 处理逻辑`,
                            });
                            break;
                    }
                })
            ).subscribe(obs => {
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
    static parameterTemplate(url, body, emptyBody = false) {
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
            // if (params.method == "get") {
            //     promise = this.get(params.url, params.body, params.headers).toPromise();
            // } else {
            //     promise = this.post(params.url, params.body, params.headers).toPromise();
            // }
            promise = this.ajax(params).toPromise();
            CacheHttp.set(key, promise);
        }
        const data = await promise || [];
        Cache.set(key, data);
        return data;
    }
    /**
     * ajax
     * @param urlOrRequest 
     */
    ajax(urlOrRequest: string | AjaxRequest) {
        if (lodash.isString(urlOrRequest)) {
            return this.AjaxObservable(ajax(this.get({ url: urlOrRequest, headers: this.getHeaders() })))
        }
        urlOrRequest.headers = { ...this.getHeaders(), ...urlOrRequest.headers };
        switch (lodash.toLower(urlOrRequest.method)) {
            case 'post':
                urlOrRequest = this.post(urlOrRequest)
                break;
            case 'put':
                urlOrRequest = this.put(urlOrRequest)
                break;
            case 'delete':
                urlOrRequest = this.delete(urlOrRequest)
                break;
            default://get
                urlOrRequest = this.get(urlOrRequest)
                break;
        }
        return this.AjaxObservable(ajax(urlOrRequest))
    }
    /**
     * 
     * @param urlOrRequest 
     */
    private get(urlOrRequest: AjaxRequest): AjaxRequest {
        const newParams = Request.parameterTemplate(urlOrRequest.url, urlOrRequest.body, true);
        urlOrRequest.body = Request.formatBody(newParams.body);
        urlOrRequest.url = Request.compatibleUrl(this.target, newParams.url, urlOrRequest.body);
        return urlOrRequest
    }
    /**
     * post
     * @param url 
     * @param body 
     * @param headers 
     */
    private post(urlOrRequest: AjaxRequest): AjaxRequest {
        const newParams = Request.parameterTemplate(urlOrRequest.url, urlOrRequest.body);
        urlOrRequest.body = Request.formatBody(newParams.body, "body", urlOrRequest.headers);
        urlOrRequest.url = Request.compatibleUrl(this.target, newParams.url);
        return urlOrRequest
    }
    /**
     * put
     * @param url 
     * @param body 
     * @param headers 
     */
    private put(urlOrRequest: AjaxRequest): AjaxRequest {
        const newParams = Request.parameterTemplate(urlOrRequest.url, urlOrRequest.body);
        urlOrRequest.body = Request.formatBody(newParams.body, "body", urlOrRequest.headers);
        urlOrRequest.url = Request.compatibleUrl(this.target, newParams.url);
        return urlOrRequest
    }
    /**
     * delete
     * @param url 
     * @param body 
     * @param headers 
     */
    delete(urlOrRequest: AjaxRequest): AjaxRequest {
        const newParams = Request.parameterTemplate(urlOrRequest.url, urlOrRequest.body, true);
        urlOrRequest.body = Request.formatBody(newParams.body);
        urlOrRequest.url = Request.compatibleUrl(this.target, newParams.url, urlOrRequest.body);
        return urlOrRequest
    }
    /** jsonp 回调 计数 */
    private jsonpCounter = 0;
    /**
     * jsonP
     */
    jsonp(url, body?: { [key: string]: any } | string, callbackKey = 'callback') {
        this.getHeaders();
        body = Request.formatBody(body);
        url = Request.compatibleUrl(this.target, url, `${body || '?time=' + new Date().getTime()}&${callbackKey}=`);
        return new Observable(observer => {
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
    static compatibleUrl(address: string, url: string, endStr?: string) {
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
    static formatBody(
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
    static NProgress(type: 'start' | 'done' = 'start') {
        if (type == "start") {
            NProgress.start();
        } else {
            NProgress.done();
        }
    }
}
export default new Request();
