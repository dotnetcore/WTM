var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spread = (this && this.__spread) || function () {
    for (var ar = [], i = 0; i < arguments.length; i++) ar = ar.concat(__read(arguments[i]));
    return ar;
};
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
/** 缓存 http 请求 */
var CacheHttp = new Map();
/** 缓存数据 */
var Cache = new Map();
// 每30秒清理一次缓存
Rx.Observable.interval(30000).subscribe(function (obs) {
    CacheHttp.clear();
    Cache.clear();
});
var Request = /** @class */ (function () {
    /**
     *
     * @param address 替换默认地址前缀
     * @param newResponseMap 替换默认过滤函数
     */
    function Request(address, newResponseMap) {
        var _this = this;
        this.newResponseMap = newResponseMap;
        /**
         * 请求路径前缀
         */
        this.address = '/masterdata/';
        /**
         * 请求头
         */
        this.headers = {
            credentials: 'include',
            accept: "*/*",
            "Content-Type": "application/json",
            "token": null
        };
        /**
         * 请求超时设置
         */
        this.timeout = 10000;
        /** 文件获取状态 */
        this.downloadLoading = false;
        /** jsonp 回调 计数 */
        this.jsonpCounter = 0;
        this.notificationKey = "notificationKey";
        /**
         * ajax过滤
         */
        this.responseMap = function (res) {
            // 关闭加载进度条
            setTimeout(function () {
                _this.NProgress("done");
            });
            try {
                // 使用传入得 过滤函数
                if (_this.newResponseMap && typeof _this.newResponseMap == "function") {
                    return _this.newResponseMap(res);
                }
                if (res.status == 200) {
                    // 判断是否统一数据格式，是走状态判断，否直接返回 response
                    if (res.response && res.response.status) {
                        switch (res.response.status) {
                            case 200:
                                return res.response.data;
                                break;
                            case 204:
                                return false;
                                break;
                            default:
                                throw {
                                    url: res.request.url,
                                    request: res,
                                    message: res.response.message,
                                    response: res.response
                                };
                                return false;
                                break;
                        }
                    }
                    return res.response;
                }
                throw {
                    url: res.request.url,
                    request: res,
                    message: res.message,
                    response: false
                };
            }
            catch (error) {
                // console.error(error);
                if (lodash.includes(error.url, "/test/")) {
                    error.message = "请检查接口配置是否正确";
                }
                notification['error']({
                    key: 'ajaxError',
                    message: error.message,
                    duration: 10,
                    description: "Url: " + error.url,
                });
                return false;
            }
        };
        /**
         * 过滤 map 返回的 假值
         */
        this.filter = function (data) {
            return data;
        };
        if (typeof address == "string") {
            // if (/^((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+/.test(address)) {
            //     this.address = address;
            // } else {
            //     this.address += address;
            // }
            this.address = address;
        }
        this.getHeaders();
    }
    /**
     * 获取 认证 token请求头
     */
    Request.prototype.getHeaders = function () {
        this.headers.token = window.localStorage.getItem('__token') || null;
        return this.headers;
    };
    /**
     * ajax Observable 管道
     * @param Observable
     */
    Request.prototype.AjaxObservable = function (Observable) {
        return Observable
            // 超时时间
            .timeout(this.timeout)
            // 错误处理
            .catch(function (err) { return Rx.Observable.of(err); })
            // 数据过滤
            .map(this.responseMap)
            // 数据筛选
            .filter(this.filter);
    };
    /**
     * url 参数 注入
     * @param url url
     * @param body body
     * @param emptyBody 清空 body
     */
    Request.prototype.parameterTemplate = function (url, body, emptyBody) {
        if (emptyBody === void 0) { emptyBody = false; }
        if (/{([\s\S]+?)}/g.test(url)) {
            if (typeof body == "object") {
                url = lodash.template(url, {
                    interpolate: /{([\s\S]+?)}/g
                })(body);
            }
        }
        // 清空body
        if (emptyBody) {
            body = {};
        }
        return {
            url: url,
            body: body
        };
    };
    /**
     *  请求数据 缓存数据
     * @param params
     */
    Request.prototype.cache = function (params) {
        return __awaiter(this, void 0, void 0, function () {
            var key, promise, data;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        params = __assign({ url: "", body: {}, method: "get" }, params);
                        this.getHeaders();
                        key = JSON.stringify({ url: params.url, body: params.body });
                        if (Cache.has(key)) {
                            return [2 /*return*/, Cache.get(key)];
                        }
                        if (CacheHttp.has(key)) {
                            promise = CacheHttp.get(key);
                        }
                        else {
                            if (params.method == "get") {
                                promise = this.get(params.url, params.body, params.headers).toPromise();
                            }
                            else {
                                promise = this.post(params.url, params.body, params.headers).toPromise();
                            }
                            CacheHttp.set(key, promise);
                        }
                        return [4 /*yield*/, promise];
                    case 1:
                        data = (_a.sent()) || [];
                        Cache.set(key, data);
                        return [2 /*return*/, data];
                }
            });
        });
    };
    /**
     * get
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.get = function (url, body, headers) {
        this.getHeaders();
        headers = __assign({}, this.headers, headers);
        var newParams = this.parameterTemplate(url, body, true);
        body = this.formatBody(newParams.body);
        url = this.compatibleUrl(this.address, newParams.url, body);
        return this.AjaxObservable(Rx.Observable.ajax.get(url, headers));
    };
    /**
     * post
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.post = function (url, body, headers) {
        this.getHeaders();
        headers = __assign({}, this.headers, headers);
        var newParams = this.parameterTemplate(url, body);
        body = this.formatBody(newParams.body, "body", headers);
        url = this.compatibleUrl(this.address, newParams.url);
        return this.AjaxObservable(Rx.Observable.ajax.post(url, body, headers));
    };
    /**
     * put
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.put = function (url, body, headers) {
        this.getHeaders();
        headers = __assign({}, this.headers, headers);
        var newParams = this.parameterTemplate(url, body);
        body = this.formatBody(newParams.body, "body", headers);
        url = this.compatibleUrl(this.address, newParams.url);
        return this.AjaxObservable(Rx.Observable.ajax.put(url, body, headers));
    };
    /**
     * delete
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.delete = function (url, body, headers) {
        this.getHeaders();
        headers = __assign({}, this.headers, headers);
        var newParams = this.parameterTemplate(url, body, true);
        body = this.formatBody(newParams.body);
        url = this.compatibleUrl(this.address, newParams.url, body);
        return this.AjaxObservable(Rx.Observable.ajax.delete(url, headers));
    };
    /**
     * 下载文件
     * @param AjaxRequest
     * @param fileType
     * @param fileName
     */
    Request.prototype.download = function (AjaxRequest, fileType, fileName) {
        if (fileType === void 0) { fileType = '.xls'; }
        if (fileName === void 0) { fileName = moment().format("YYYY_MM_DD_hh_mm_ss"); }
        return __awaiter(this, void 0, void 0, function () {
            var result;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.getHeaders();
                        if (this.downloadLoading) {
                            return [2 /*return*/, message.warn('文件获取中，请勿重复操作~')];
                        }
                        this.downloadLoading = true;
                        this.NProgress();
                        AjaxRequest.url = this.compatibleUrl(this.address, AjaxRequest.url);
                        AjaxRequest = __assign({ 
                            // url: url,
                            method: "post", responseType: "blob", timeout: this.timeout, headers: this.headers }, AjaxRequest);
                        if (AjaxRequest.body) {
                            AjaxRequest.body = this.formatBody(AjaxRequest.body, "body", AjaxRequest.headers);
                        }
                        return [4 /*yield*/, Rx.Observable.ajax(AjaxRequest).catch(function (err) { return Rx.Observable.of(err); }).toPromise()];
                    case 1:
                        result = _a.sent();
                        this.NProgress("done");
                        this.downloadLoading = false;
                        try {
                            if (result.status == 200) {
                                this.onCreateBlob(result.response, fileType, fileName).click();
                                notification.success({
                                    message: "\u6587\u4EF6\u4E0B\u8F7D\u6210\u529F",
                                    description: ''
                                });
                            }
                            else {
                                notification['error']({
                                    key: this.notificationKey,
                                    message: '文件下载失败',
                                    description: result.message,
                                });
                            }
                        }
                        catch (error) {
                            notification['error']({
                                key: this.notificationKey,
                                message: '文件下载失败',
                                description: error.message,
                            });
                        }
                        return [2 /*return*/];
                }
            });
        });
    };
    /**
     * 创建二进制文件
     * @param response
     */
    Request.prototype.onCreateBlob = function (response, fileType, fileName) {
        if (fileType === void 0) { fileType = '.xls'; }
        if (fileName === void 0) { fileName = moment().format("YYYY_MM_DD_hh_mm_ss"); }
        var blob = response;
        var a = document.createElement('a');
        var downUrl = window.URL.createObjectURL(blob);
        a.href = downUrl;
        switch (blob.type) {
            case 'application/vnd.ms-excel':
                a.download = fileName + '.xls';
                break;
            default:
                a.download = fileName + fileType;
                break;
        }
        a.addEventListener("click", function () {
            setTimeout(function () {
                window.URL.revokeObjectURL(downUrl);
            });
        }, false);
        return a;
    };
    /**
     * 重写 Upload 默认请求  https://ant.design/components/upload-cn/#onChange
     * @param option
     * @param responseType
     */
    Request.prototype.customRequest = function (option, responseType) {
        if (responseType === void 0) { responseType = ""; }
        function getError(option, xhr) {
            var msg = "cannot post " + option.action + " " + xhr.status + "'";
            var err = new Error(msg);
            err.status = xhr.status;
            err.method = 'post';
            err.url = option.action;
            return err;
        }
        function getBody(xhr) {
            if (xhr.responseType == "blob") {
                return xhr.response;
            }
            var text = xhr.responseText || xhr.response;
            if (!text) {
                return text;
            }
            try {
                return JSON.parse(text);
            }
            catch (e) {
                return text;
            }
        }
        var xhr = new XMLHttpRequest();
        xhr.responseType = responseType;
        if (option.onProgress && xhr.upload) {
            xhr.upload.onprogress = function progress(e) {
                if (e.total > 0) {
                    e.percent = e.loaded / e.total * 100;
                }
                option.onProgress(e);
            };
        }
        var formData = new FormData();
        if (option.data) {
            Object.keys(option.data).map(function (key) {
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
        var headers = option.headers || {};
        if (headers['X-Requested-With'] !== null) {
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        }
        for (var h in headers) {
            if (headers.hasOwnProperty(h) && headers[h] !== null) {
                xhr.setRequestHeader(h, headers[h]);
            }
        }
        xhr.send(formData);
        return {
            abort: function () {
                xhr.abort();
            },
        };
    };
    /**
     * jsonP
     */
    Request.prototype.jsonp = function (url, body, callbackKey) {
        var _this = this;
        if (callbackKey === void 0) { callbackKey = 'callback'; }
        this.getHeaders();
        body = this.formatBody(body);
        url = this.compatibleUrl(this.address, url, (body || '?time=' + new Date().getTime()) + "&" + callbackKey + "=");
        return new Rx.Observable(function (observer) {
            _this.jsonpCounter++;
            var key = '_jsonp_callback_' + _this.jsonpCounter;
            var script = document.createElement('script');
            script.src = url + key;
            script.onerror = function (err) { return observer.error(err); };
            document.body.appendChild(script);
            window[key] = function (response) {
                // clean up
                script.parentNode.removeChild(script);
                delete window[key];
                // push response downstream
                observer.next(response);
                observer.complete();
            };
        });
    };
    ;
    /**
     * url 兼容处理
     * @param address 前缀
     * @param url url
     * @param endStr 结尾，参数等
     */
    Request.prototype.compatibleUrl = function (address, url, endStr) {
        endStr = endStr || '';
        if (/^((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+/.test(url)) {
            return "" + url + endStr;
        }
        else {
            // address  / 结尾  url / 开头
            var isAddressWith = lodash.endsWith(address, "/");
            var isUrlWith = lodash.startsWith(url, "/");
            // debugger
            if (isAddressWith) {
                if (isUrlWith) {
                    url = lodash.trimStart(url, "/");
                }
            }
            else {
                if (isUrlWith) {
                }
                else {
                    url = "/" + url;
                }
            }
        }
        return "" + address + url + endStr;
    };
    /**
     * 格式化 参数
     * @param body  参数
     * @param type  参数传递类型
     * @param headers 请求头 type = body 使用
     */
    Request.prototype.formatBody = function (body, type, headers) {
        if (type === void 0) { type = "url"; }
        // 加载进度条
        this.NProgress();
        if (type === "url") {
            var param = "";
            if (typeof body != 'string') {
                var parlist_1 = [];
                lodash.forEach(body, function (value, key) {
                    if (!lodash.isNil(value) && value != "") {
                        parlist_1.push(key + "=" + value);
                    }
                });
                if (parlist_1.length) {
                    param = "?" + parlist_1.join("&");
                }
            }
            else {
                param = body;
            }
            return param;
        }
        else {
            // 处理 Content-Type body 类型 
            switch (headers["Content-Type"]) {
                case 'application/json;charset=UTF-8':
                    body = JSON.stringify(body);
                    break;
                case 'application/json':
                    if (lodash.isArray(body)) {
                        body = __spread(body);
                    }
                    if (lodash.isPlainObject(body)) {
                        body = __assign({}, body);
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
    };
    /** 日志 */
    Request.prototype.log = function (url, body, headers) {
    };
    /**
     *  加载进度条
     * @param type
     */
    Request.prototype.NProgress = function (type) {
        if (type === void 0) { type = 'start'; }
        if (type == "start") {
            NProgress.start();
        }
        else {
            NProgress.done();
        }
    };
    return Request;
}());
export { Request };
export default new Request();
//# sourceMappingURL=Request.js.map