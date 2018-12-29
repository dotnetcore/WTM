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
export class Request {
    /**
     * 
     * @param address 替换默认地址前缀
     * @param newResponseMap 替换默认过滤函数
     */
    constructor(address?, public newResponseMap?) {
        if (typeof address == "string") {
            // if (/^((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+/.test(address)) {
            //     this.address = address;
            // } else {
            //     this.address += address;
            // }
            this.address = address;
        }
    }
    /** 
     * 请求路径前缀
     */
    address = '/api'
    /**
     * 请求头
     */
    headers = {
        credentials: 'include',
        accept: "*/*",
        "Content-Type": "application/json",
    };
    /**
     * 请求超时设置
     */
    timeout = 3000;
    /**
     * get
     * @param url 
     * @param body 
     * @param headers 
     */
    get(url: string, body?: { [key: string]: any } | string, headers?: Object) {
        headers = { ...this.headers, ...headers };
        if (/\/{\S*}/.test(url)) {
            if (typeof body == "object") {
                const urlStr = lodash.compact(url.match(/\/{\w[^\/{]*}/g).map(x => {
                    return body[x.match(/{(\w*)}/)[1]];
                })).join("/");
                url = url.replace(/\/{\S*}/, "/") + urlStr;
                body = {};
            }
        }
        body = this.formatBody(body);
        url = this.compatibleUrl(this.address, url, body as any);
        return Rx.Observable.ajax.get(
            url,
            headers
        ).timeout(this.timeout).catch(err => Rx.Observable.of(err)).map(this.responseMap);
    }
    /**
     * post
     * @param url 
     * @param body 
     * @param headers 
     */
    post(url: string, body?: any, headers?: Object) {
        headers = { ...this.headers, ...headers };
        body = this.formatBody(body, "body", headers);
        url = this.compatibleUrl(this.address, url);

        return Rx.Observable.ajax.post(
            url,
            body,
            headers
        ).timeout(this.timeout).catch(err => Rx.Observable.of(err)).map(this.responseMap);
    }
    /**
     * put
     * @param url 
     * @param body 
     * @param headers 
     */
    put(url: string, body?: any, headers?: Object) {
        headers = { ...this.headers, ...headers };
        body = this.formatBody(body, "body", headers);
        url = this.compatibleUrl(this.address, url);
        return Rx.Observable.ajax.put(
            url,
            body,
            headers
        ).timeout(this.timeout).catch(err => Rx.Observable.of(err)).map(this.responseMap);
    }
    /**
     * delete
     * @param url 
     * @param body 
     * @param headers 
     */
    delete(url: string, body?: { [key: string]: any } | string, headers?: Object) {
        headers = { ...this.headers, ...headers };
        body = this.formatBody(body);
        url = this.compatibleUrl(this.address, url, body as any);
        return Rx.Observable.ajax.delete(
            url,
            headers
        ).timeout(this.timeout).catch(err => Rx.Observable.of(err)).map(this.responseMap);
    }
    /** 文件获取状态 */
    downloadLoading = false
    /**
     * 下载文件
     * @param AjaxRequest 
     * @param fileType 
     * @param fileName 
     */
    async download(AjaxRequest: Rx.AjaxRequest, fileType = '.xls', fileName = moment().format("YYYY_MM_DD_hh_mm_ss")) {
        if (this.downloadLoading) {
            return message.warn('文件获取中，请勿重复操作~')
        }
        this.downloadLoading = true;
        NProgress.start();
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
        const result = await Rx.Observable.ajax(AjaxRequest).catch(err => Rx.Observable.of(err)).toPromise();
        NProgress.done();
        this.downloadLoading = false;
        try {
            if (result.status == 200) {
                const blob = result.response;
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
                a.click();
                window.URL.revokeObjectURL(downUrl);
                message.success(`文件下载成功`)
            } else {
                notification['error']({
                    key: 'notificationKey',
                    message: '文件下载失败',
                    description: result.message,
                });
            }

        } catch (error) {
            notification['error']({
                key: 'notificationKey',
                message: '文件下载失败',
                description: error.message,
            });
        }
    }
    /** jsonp 回调 计数 */
    jsonpCounter = 0;
    /**
     * jsonP
     */
    jsonp(url, body?: { [key: string]: any } | string, callbackKey = 'callback') {
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
        // 加载进度条
        NProgress.start();
        // if (typeof body === 'undefined') {
        //     return '';
        // }
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
    /**
     * ajax过滤
     */
    responseMap = (x) => {
        // 关闭加载进度条
        setTimeout(() => {
            NProgress.done();
        });
        if (this.newResponseMap && typeof this.newResponseMap == "function") {
            return this.newResponseMap(x);
        }
        // if (x.status == 200) {
        //     // 判断是否统一数据格式，是走状态判断，否直接返回 response
        //     if (x.response && x.response.status) {
        //         switch (x.response.status) {
        //             case 200:
        //                 return x.response.data;
        //                 break;
        //             case 204:
        //                 return false;
        //                 break;
        //             default:
        //                 notification['error']({
        //                     message: x.response.message,
        //                     description: `Url: ${x.request.url} \n method: ${x.request.method}`,
        //                 });
        //                 return false
        //                 break;
        //         }
        //     }
        //     return x.response
        // }
        switch (x.status) {
            case 200:
                return x.response;
                break;
            case 204:
                return false;
                break;
            case 400:
                notification['error']({
                    duration:10,
                    message: JSON.stringify(x.response),
                    description: `Url: ${x.request.url} \n method: ${x.request.method}`,
                });
                return false;
                break;
            default:
                notification['error']({
                    message: x.response.message,
                    description: `Url: ${x.request.url} \n method: ${x.request.method}`,
                });
                return false
                break;
        }
        // notification['error']({
        //     key: 'notificationKey',
        //     message: x.message,
        //     description: x.request ? `Url: ${x.request.url} \n method: ${x.request.method}` : '',
        // });
        // console.error(x);
        // throw x;
        return false
    }
    /** 日志 */
    log(url, body, headers) {
    }
}
export default new Request();