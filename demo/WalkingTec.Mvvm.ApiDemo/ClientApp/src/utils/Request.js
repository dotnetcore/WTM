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
        this.address = '/api';
        /**
         * 请求头
         */
        this.headers = {
            credentials: 'include',
            accept: "*/*",
            "Content-Type": "application/json",
        };
        /**
         * 请求超时设置
         */
        this.timeout = 3000;
        /** 文件获取状态 */
        this.downloadLoading = false;
        /** jsonp 回调 计数 */
        this.jsonpCounter = 0;
        /**
         * ajax过滤
         */
        this.responseMap = function (x) {
            // 关闭加载进度条
            setTimeout(function () {
                NProgress.done();
            });
            if (_this.newResponseMap && typeof _this.newResponseMap == "function") {
                return _this.newResponseMap(x);
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
                        duration: 10,
                        message: JSON.stringify(x.response),
                        description: "Url: " + x.request.url + " \n method: " + x.request.method,
                    });
                    return false;
                    break;
                default:
                    notification['error']({
                        message: x.response.message,
                        description: "Url: " + x.request.url + " \n method: " + x.request.method,
                    });
                    return false;
                    break;
            }
            // notification['error']({
            //     key: 'notificationKey',
            //     message: x.message,
            //     description: x.request ? `Url: ${x.request.url} \n method: ${x.request.method}` : '',
            // });
            // console.error(x);
            // throw x;
            return false;
        };
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
     * get
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.get = function (url, body, headers) {
        headers = __assign({}, this.headers, headers);
        if (/\/{\S*}/.test(url)) {
            if (typeof body == "object") {
                var urlStr = lodash.compact(url.match(/\/{\w[^\/{]*}/g).map(function (x) {
                    return body[x.match(/{(\w*)}/)[1]];
                })).join("/");
                url = url.replace(/\/{\S*}/, "/") + urlStr;
                body = {};
            }
        }
        body = this.formatBody(body);
        url = this.compatibleUrl(this.address, url, body);
        return Rx.Observable.ajax.get(url, headers).timeout(this.timeout).catch(function (err) { return Rx.Observable.of(err); }).map(this.responseMap);
    };
    /**
     * post
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.post = function (url, body, headers) {
        headers = __assign({}, this.headers, headers);
        body = this.formatBody(body, "body", headers);
        url = this.compatibleUrl(this.address, url);
        return Rx.Observable.ajax.post(url, body, headers).timeout(this.timeout).catch(function (err) { return Rx.Observable.of(err); }).map(this.responseMap);
    };
    /**
     * put
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.put = function (url, body, headers) {
        headers = __assign({}, this.headers, headers);
        body = this.formatBody(body, "body", headers);
        url = this.compatibleUrl(this.address, url);
        return Rx.Observable.ajax.put(url, body, headers).timeout(this.timeout).catch(function (err) { return Rx.Observable.of(err); }).map(this.responseMap);
    };
    /**
     * delete
     * @param url
     * @param body
     * @param headers
     */
    Request.prototype.delete = function (url, body, headers) {
        headers = __assign({}, this.headers, headers);
        body = this.formatBody(body);
        url = this.compatibleUrl(this.address, url, body);
        return Rx.Observable.ajax.delete(url, headers).timeout(this.timeout).catch(function (err) { return Rx.Observable.of(err); }).map(this.responseMap);
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
            var result, blob, a, downUrl;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (this.downloadLoading) {
                            return [2 /*return*/, message.warn('文件获取中，请勿重复操作~')];
                        }
                        this.downloadLoading = true;
                        NProgress.start();
                        AjaxRequest = __assign({ 
                            // url: url,
                            method: "post", responseType: "blob", timeout: this.timeout, headers: this.headers }, AjaxRequest);
                        if (AjaxRequest.body) {
                            AjaxRequest.body = this.formatBody(AjaxRequest.body, "body", AjaxRequest.headers);
                        }
                        return [4 /*yield*/, Rx.Observable.ajax(AjaxRequest).catch(function (err) { return Rx.Observable.of(err); }).toPromise()];
                    case 1:
                        result = _a.sent();
                        NProgress.done();
                        this.downloadLoading = false;
                        try {
                            if (result.status == 200) {
                                blob = result.response;
                                a = document.createElement('a');
                                downUrl = window.URL.createObjectURL(blob);
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
                                message.success("\u6587\u4EF6\u4E0B\u8F7D\u6210\u529F");
                            }
                            else {
                                notification['error']({
                                    key: 'notificationKey',
                                    message: '文件下载失败',
                                    description: result.message,
                                });
                            }
                        }
                        catch (error) {
                            notification['error']({
                                key: 'notificationKey',
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
     * jsonP
     */
    Request.prototype.jsonp = function (url, body, callbackKey) {
        var _this = this;
        if (callbackKey === void 0) { callbackKey = 'callback'; }
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
        NProgress.start();
        // if (typeof body === 'undefined') {
        //     return '';
        // }
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
    return Request;
}());
export { Request };
export default new Request();
//# sourceMappingURL=Request.js.map