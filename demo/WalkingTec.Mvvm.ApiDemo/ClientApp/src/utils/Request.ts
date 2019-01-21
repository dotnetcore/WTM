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
     * @param address 替换默认地址前缀
     * @param newMap 替换默认过滤函数
     */
    constructor(address?, public newMap?) {
        if (typeof address === "string") {
            this.address = address;
        }
        this.getHeaders();
    }
    /** 
     * 请求路径前缀
     */
    address = '/api/'
    /**
     * 请求头
     */
    private headers = {
        credentials: 'include',
        accept: "*/*",
        "Content-Type": "application/json",
        "token": null
    };
    /**
     * 获取 认证 token请求头
     */
    getHeaders() {
        this.headers.token = window.localStorage.getItem('__token') || null;
        return this.headers
    }

    /**
     * 请求超时设置
     */
    private timeout = 10000;
    /**
     * 抛出 状态 
     */
    private catchStatus = [400]
    /**
     * ajax Observable 管道
     * @param Observable 
     */
    private AjaxObservable(Observable: Rx.Observable<Rx.AjaxResponse>) {
        // 加载进度条
        this.NProgress();
        return new Rx.Observable(sub => {
            Observable
                // 超时时间
                .timeout(this.timeout)
                // 错误处理
                .catch((err: Rx.AjaxError) => Rx.Observable.of(err))
                // 过滤请求
                .filter((ajax) => {
                    this.NProgress("done");
                    if (ajax instanceof Rx.AjaxResponse) {
                        return true
                    }
                    if (ajax instanceof Rx.AjaxError) {
                        // 返回 业务处理错误
                        if (lodash.includes(this.catchStatus, ajax.status)) {
                            sub.error(ajax.response)
                            if (ajax.response) {
                                return false
                            }
                        }
                        notification['error']({
                            key: ajax.request.url,
                            message: ajax.status,
                            duration: 5,
                            description: `${ajax.request.method}: ${ajax.request.url}`,
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
                                description: `请配置 处理逻辑`,
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
        if (/{([\s\S]+?)}/g.test(url)) {
            if (typeof body == "object") {
                url = lodash.template(url, {
                    interpolate: /{([\s\S]+?)}/g
                })(body);
            }
            // 清空body
            if (emptyBody) {
                body = {};
            }
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
        this.getHeaders();
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
        this.getHeaders();
        headers = { ...this.headers, ...headers };
        const newParams = this.parameterTemplate(url, body, true);
        body = this.formatBody(newParams.body);
        url = this.compatibleUrl(this.address, newParams.url, body as any);
        return this.AjaxObservable(Rx.Observable.ajax.get(url, headers))
    }
    /**
     * post
     * @param url 
     * @param body 
     * @param headers 
     */
    post(url: string, body?: any, headers?: Object): Rx.Observable<any> {
        this.getHeaders();
        headers = { ...this.headers, ...headers };
        const newParams = this.parameterTemplate(url, body);
        body = this.formatBody(newParams.body, "body", headers);
        url = this.compatibleUrl(this.address, newParams.url);
        return this.AjaxObservable(Rx.Observable.ajax.post(url, body, headers))
    }
    /**
     * put
     * @param url 
     * @param body 
     * @param headers 
     */
    put(url: string, body?: any, headers?: Object): Rx.Observable<any> {
        this.getHeaders();
        headers = { ...this.headers, ...headers };
        const newParams = this.parameterTemplate(url, body);
        body = this.formatBody(newParams.body, "body", headers);
        url = this.compatibleUrl(this.address, newParams.url);
        return this.AjaxObservable(Rx.Observable.ajax.put(url, body, headers))
    }
    /**
     * delete
     * @param url 
     * @param body 
     * @param headers 
     */
    delete(url: string, body?: { [key: string]: any } | string, headers?: Object): Rx.Observable<any> {
        this.getHeaders();
        headers = { ...this.headers, ...headers };
        const newParams = this.parameterTemplate(url, body, true);
        body = this.formatBody(newParams.body);
        url = this.compatibleUrl(this.address, newParams.url, body as any);
        return this.AjaxObservable(Rx.Observable.ajax.delete(url, headers))
    }
    /** 文件获取状态 */
    private downloadLoading = false
    /**
     * 下载文件
     * @param AjaxRequest 
     * @param fileType 
     * @param fileName 
     */
    async download(AjaxRequest: Rx.AjaxRequest, fileType = '.xls', fileName = moment().format("YYYY_MM_DD_hh_mm_ss")) {
        this.getHeaders();
        if (this.downloadLoading) {
            return message.warn('文件获取中，请勿重复操作~')
        }
        this.downloadLoading = true;
        this.NProgress()
        AjaxRequest.url = this.compatibleUrl(this.address, AjaxRequest.url);
        AjaxRequest = {
            // url: url,
            method: "post",
            responseType: "blob",
            timeout: this.timeout,
            headers: this.headers,
            ...AjaxRequest
        }
        if (AjaxRequest.body) {
            AjaxRequest.body = this.formatBody(AjaxRequest.body, "body", AjaxRequest.headers);
        }
        try {
            const result = await Rx.Observable.ajax(AjaxRequest).toPromise();
            this.onCreateBlob(result.response, fileType, fileName).click();
            notification.success({
                key: "download",
                message: `文件下载成功`,
                description: ''
            })
        } catch (error) {
            notification.error({
                key: "download",
                message: '文件下载失败',
                description: error.message,
            });
        }
        finally {
            this.NProgress("done")
            this.downloadLoading = false;
        }
    }
    /**
     * 创建二进制文件
     * @param response 
     */
    onCreateBlob(response, fileType = '.xls', fileName = moment().format("YYYY_MM_DD_hh_mm_ss")) {
        const blob = response;
        const a = document.createElement('a');
        const downUrl = window.URL.createObjectURL(blob);
        a.href = downUrl;
        switch (blob.type) {
            case 'application/vnd.ms-excel':
                a.download = fileName + '.xls';
                break;
            default:
                a.download = fileName + fileType;
                break;
        }
        a.addEventListener("click", () => {
            setTimeout(() => {
                window.URL.revokeObjectURL(downUrl);
            }, 1000);
        }, false);
        return a;
    }
    /**
     * 重写 Upload 默认请求  https://ant.design/components/upload-cn/#onChange
     * @param option 
     * @param responseType 
     */
    customRequest(option, responseType: XMLHttpRequestResponseType = "") {
        function getError(option, xhr) {
            const msg = `cannot post ${option.action} ${xhr.status}'`;
            const err = new Error(msg) as any;
            err.status = xhr.status;
            err.method = 'post';
            err.url = option.action;
            return err;
        }
        function getBody(xhr) {
            if (xhr.responseType == "blob") {
                return xhr.response
            }
            const text = xhr.responseText || xhr.response;
            if (!text) {
                return text;
            }

            try {
                return JSON.parse(text);
            } catch (e) {
                return text;
            }
        }
        const xhr = new XMLHttpRequest();
        xhr.responseType = responseType;
        if (option.onProgress && xhr.upload) {
            xhr.upload.onprogress = function progress(e: any) {
                if (e.total > 0) {
                    e.percent = e.loaded / e.total * 100;
                }
                option.onProgress(e);
            };
        }
        const formData = new FormData();
        if (option.data) {
            Object.keys(option.data).map(key => {
                formData.append(key, option.data[key]);
            });
        }
        formData.append(option.filename, option.file);
        xhr.onerror = function error(e) {
            option.onError(e);
        };
        xhr.onload = function onload() {
            if (xhr.status < 200 || xhr.status >= 300) {
                return option.onError(getError(option, xhr), getBody(xhr));
            }
            option.onSuccess(getBody(xhr), xhr);
        };
        xhr.open('post', option.action, true);
        if (option.withCredentials && 'withCredentials' in xhr) {
            xhr.withCredentials = true;
        }
        const headers = option.headers || {};
        if (headers['X-Requested-With'] !== null) {
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        }
        for (const h in headers) {
            if (headers.hasOwnProperty(h) && headers[h] !== null) {
                xhr.setRequestHeader(h, headers[h]);
            }
        }
        xhr.send(formData);
        return {
            abort() {
                xhr.abort();
            },
        };
    }
    /** jsonp 回调 计数 */
    private jsonpCounter = 0;
    /**
     * jsonP
     */
    jsonp(url, body?: { [key: string]: any } | string, callbackKey = 'callback') {
        this.getHeaders();
        body = this.formatBody(body);
        url = this.compatibleUrl(this.address, url, `${body || '?time=' + new Date().getTime()}&${callbackKey}=`);
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
    private log(url, body, headers) {

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
