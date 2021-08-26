/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:37
 * @modify date 2018-09-12 18:52:37
 * @desc [description]
*/
import lodash from 'lodash';
import { Observable, of, TimeoutError, interval } from "rxjs";
import { ajax, AjaxError, AjaxResponse, AjaxRequest } from "rxjs/ajax";
import { catchError, filter, map, merge, timeout } from "rxjs/operators";
import queryString from 'query-string';
import { Regulars } from "./regulars";
import moment from "moment";
export interface IAjaxBasicsOptions {
    /**
     * 路径
     * @type {string}
     * @memberof IAjaxBasicsOptions
     */
    target?: string;
    /**
     * 超时 时间
     * @type {number}
     * @memberof IAjaxBasicsOptions
     */
    timeout?: number;
    /**
     * 重写 XMLHttpRequest
     * @memberof IAjaxBasicsOptions
     */
    createXHR?: () => XMLHttpRequest;
}
export class AjaxBasics {
    /**
     * 
     * @param target 
     */
    constructor(options?: IAjaxBasicsOptions) {
        this.options = lodash.assign({
            target: "/",
            timeout: 30000,
        }, options);
        // this.onTime()
    }
    // static serviceDate = moment(Date.now());
    // static serviceDateKey = 'Date';
    // /**
    //  * 服务器时间
    //  * @readonly
    //  * @memberof AjaxBasics
    //  */
    // get serviceDate() {
    //     return AjaxBasics.serviceDate
    // }
    // /**
    //  * 计时
    //  */
    // onTime() {
    //     interval(1000).subscribe(x => {
    //         AjaxBasics.serviceDate.add(1, 'seconds')
    //     })
    // }
    options: IAjaxBasicsOptions = {};
    /** get */
    get<T>(url: string, body?: any, headers?: Object, options?: IAjaxBasicsOptions) {
        return this.request<T>({ url, body, headers, method: 'get' }, options).toPromise()
    }
    /** post */
    post<T>(url: string, body?: any, headers?: Object, options?: IAjaxBasicsOptions) {
        return this.request<T>({ url, body, headers, method: 'post' }, options).toPromise()
    }
    /** put */
    put<T>(url: string, body?: any, headers?: Object, options?: IAjaxBasicsOptions) {
        return this.request<T>({ url, body, headers, method: 'put' }, options).toPromise()
    }
    /** patch */
    patch<T>(url: string, body?: any, headers?: Object, options?: IAjaxBasicsOptions) {
        return this.request<T>({ url, body, headers, method: 'patch' }, options).toPromise()
    }
    /** delete */
    delete<T>(url: string, body?: any, headers?: Object, options?: IAjaxBasicsOptions) {
        return this.request<T>({ url, body, headers, method: 'delete' }, options).toPromise()
    }
    /** request */
    request<T>(request: string | AjaxRequest, options?: IAjaxBasicsOptions) {
        return AjaxBasics.onRequest<T>(request, lodash.assign({}, this.options, options))
    }
    /**
     * request
     * @param request 
     */
    static onRequest<T>(request: string | AjaxRequest, options: IAjaxBasicsOptions) {
        if (lodash.isString(request)) {
            request = {
                url: request,
                method: 'get'
            };
        };
        return AjaxBasics.AjaxObservable<T>(request, options)
    }
    /**
     * ajax 启动
     * @param request 
     */
    static AjaxObservableStart(request: AjaxRequest) {
        // return interval(1000).toPromise()
    }
    /**
    * ajax Observable 管道
    * @param Observable 
    */
    static AjaxObservable<T>(request: AjaxRequest, options: IAjaxBasicsOptions) {
        return new Observable<T>((async (sub) => {
            // 加载进度条
            AjaxBasics.onNProgress();
            await AjaxBasics.AjaxObservableStart(request)
            request = AjaxBasics.onCompatibleAjaxRequest(request, options);
            ajax(request).pipe(
                // 超时时间
                timeout(options.timeout),
                // 错误处理
                catchError((err) => of(err)),
                // 过滤请求
                filter((ajax) => {
                    try {
                        // 取服务器时间
                        // try {
                        //     const currentTarget: XMLHttpRequest = lodash.get(ajax, 'originalEvent.currentTarget');
                        //     if (currentTarget) {
                        //         const time = currentTarget.getResponseHeader(AjaxBasics.serviceDateKey)
                        //         time && (AjaxBasics.serviceDate = moment(time));
                        //     }
                        // } catch (error) {
                        //     console.error('获取系统时间错误')
                        // }
                        AjaxBasics.onNProgress("done");
                        return AjaxBasics.onFilter(ajax);
                    } catch (error) {
                        AjaxBasics.onError(error);
                        sub.error(error);
                        return false
                    }
                }),
                // 数据过滤
                map(res => AjaxBasics.onMap(res))
            ).subscribe(res => {
                sub.next(res)
                sub.complete()
            })
        }) as any)
    }
    /**
    * 请求头
    * @type {*}
    * @memberof Request
    */
    static headers: any = {
        'Content-Type': 'application/json'
    };
    /**
     * 请求 map 转换
     * @param res 
     */
    static onMap(res: AjaxResponse) {
        switch (res.status) {
            case 200:
                // 流
                if (lodash.toLower(res.responseType) === "blob") {
                    return res;
                }
                return res.response
        }
        return res
    }
    /**
     * 请求过滤
     *
     * @static
     * @param {(AjaxResponse | AjaxError | TimeoutError)} ajax
     * @returns {boolean}
     * @memberof AjaxBasics
     */
    static onFilter(ajax: AjaxResponse | AjaxError | TimeoutError): boolean {
        // 数据 Response 
        if (ajax instanceof AjaxResponse) {
            // 无 响应 数据
            if (lodash.isNil(ajax.response)) {
                console.error('ajax response undefined', lodash.assign(ajax, { message: 'ajax response undefined' }))
                // throw lodash.merge(ajax, { message: 'ajax response undefined' })
            }
            // else if (!lodash.eq(lodash.get(ajax.response, 'Code', 200), 200)) {
            //     throw lodash.merge(ajax, { message: lodash.get(ajax.response, 'Msg') })
            // }
        } else
            // 错误 超时
            if (ajax instanceof AjaxError || ajax instanceof TimeoutError) {
                throw ajax
            }
        return true
    }
    /**
     * 错误处理
     * @static
     * @param {*} error
     * @memberof AjaxBasics
     */
    static onError(error) {
        console.error("LENG: AjaxBasics -> onError -> error", error)
    }
    /**
     *  加载进度条
     * @param type 
     */
    static onNProgress(type: 'start' | 'done' = 'start') {
        if (type == "start") {
            // NProgress.start();
        } else {
            // NProgress.done();
        }
    }
    /**
     * 处理 AjaxRequest
     * @static
     * @param {(string | AjaxRequest)} request
     * @param {IAjaxBasicsOptions} options
     * @returns
     * @memberof AjaxBasics
     */
    static onCompatibleAjaxRequest(request: AjaxRequest, options: IAjaxBasicsOptions) {
        request = lodash.cloneDeep(request);
        request = AjaxBasics.onCompatibleHeaders(request);
        request = AjaxBasics.onCompatibleBody(request);
        request = AjaxBasics.onCompatibleUrl(request, options);
        request = AjaxBasics.onCompatibleCreateXHR(request, options);
        return request
    }
    /**
     * 处理 Headers
     *
     * @static
     * @param {AjaxRequest} request
     * @returns
     * @memberof AjaxBasics
     */
    static onCompatibleHeaders(request: AjaxRequest) {
        request.headers = lodash.assign({}, AjaxBasics.headers, AjaxBasics.onMergeHeaders({}, request), request.headers);
        return request
    }
    /**
     * 处理 Body
     * @static
     * @param {AjaxRequest} request
     * @returns
     * @memberof AjaxBasics
     */
    static onCompatibleBody(request: AjaxRequest) {
        let { headers, body } = request
        // body = lodash.pickBy(body, lodash.identity)
        switch (lodash.get(headers, 'Content-Type')) {
            case 'application/json;charset=UTF-8':
                body = JSON.stringify(body)
                break;
            case 'application/json':
                break;
            case 'application/x-www-form-urlencoded':
                break;
            case 'form-data':
            case 'multipart/form-data':
                break;
            case null:
            case undefined:
                // delete headers["Content-Type"];
                lodash.unset(headers, 'Content-Type')
                body = lodash.assign(body, AjaxBasics.onMergeBody());
                const formData = new FormData();
                lodash.mapValues(body,
                    (value, key) => {
                        formData.append(key, value);
                    }
                );
                request.body = formData
                return request;
            default:
                break;
        }
        request.body = lodash.assign(body, AjaxBasics.onMergeBody());
        return request;
    }
    /**
     * 合并 body
     * @param body 
     */
    static onMergeBody(body: any = {}) {
        return body
    }
    /**
     * 合并 Headers
     * @param body 
     */
    static onMergeHeaders(Headers: any = {}, request: AjaxRequest) {
        return Headers
    }
    /**
     * 处理 Url
     * @static
     * @param {AjaxRequest} request
     * @param {IAjaxBasicsOptions} options
     * @returns
     * @memberof AjaxBasics
     */
    static onCompatibleUrl(request: AjaxRequest, options: IAjaxBasicsOptions) {
        try {
            const method = lodash.toLower(request.method || 'get')
            if (lodash.isObject(request.body)) {
                const interpolate = /{([\s\S]+?)}/g;
                // 处理 url  参数 /test/{id}
                if (interpolate.test(request.url)) {
                    try {
                        request.url = lodash.template(request.url, { interpolate })(request.body);
                    } catch (error) {
                        lodash.uniq(request.url.match(interpolate)).map(par => {
                            const key = lodash.trimEnd(lodash.trimStart(par, "{"), "}")
                            const reg = new RegExp(`(\\${par})`, "g");
                            request.url = lodash.replace(request.url, reg, lodash.get(request.body, key, ''))
                        })
                    }
                    // 清理 body
                    if (lodash.get(request.body, 'delete', false) === true) {
                        request.body = undefined
                    }
                }
            }
            if (lodash.eq(method, 'get')) {
                request.url = queryString.stringifyUrl({ url: String(request.url), query: lodash.pickBy(request.body, lodash.identity) })
            }
            const isEnd = lodash.endsWith(options.target, "/")
            const isStart = lodash.startsWith(request.url, "/")
            if (isEnd && isStart) {
                request.url = lodash.trimStart(request.url, "/")
            }
            request.url = Regulars.url.test(request.url) ? request.url : options.target + request.url
        } catch (error) {
            console.warn(error)
        }
        return request;
    }
    /**
     * 重写 XHR
     * @static
     * @param {AjaxRequest} request
     * @param {IAjaxBasicsOptions} options
     * @returns
     * @memberof AjaxBasics
     */
    static onCompatibleCreateXHR(request: AjaxRequest, options: IAjaxBasicsOptions) {
        if (lodash.hasIn(options, 'createXHR')) {
            request.createXHR = options.createXHR;
        }
        return request
    }
}
// export default new AjaxBasics();
