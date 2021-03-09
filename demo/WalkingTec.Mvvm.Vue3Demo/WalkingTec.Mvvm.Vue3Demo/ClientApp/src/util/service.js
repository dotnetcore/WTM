/**
 * server封装，所有请求都走这里
 * option = {
 *      url:请求地址
 *      method: 'post' // 请求方式，post\get\put\delete
 *      data: {}, // 参数
 * }
 * _request(option)
            .then(data => {
                console.log('then', data);
            })
            .catch(data => {
                console.log('catch', data);
            });
 */
import { __assign } from "tslib";
import axios from "axios";
import { Notification } from "element-ui"; // Message,
import { contentType } from "@/config/enum";
import { AppModule } from "@/store/modules/app";
import i18n from "@/lang";
var requestBase = /** @class */ (function () {
    function requestBase() {
    }
    /**
     * 请求参数类型
     * @param params
     */
    requestBase.prototype.requestData = function (params) {
        if (params === void 0) { params = {}; }
        var data = {};
        if (!_.isArray(params)) {
            for (var key in params || {}) {
                var item = params[key];
                if (item !== undefined &&
                    item !== "") {
                    data[key] = _.isObject(item) ? this.requestData(item) : item;
                }
                else {
                    data[key] = null;
                }
            }
        }
        else {
            data = params;
        }
        return data;
    };
    /**
     * Transform 请求
     */
    requestBase.prototype.transformReq = function () {
        return [
            function (data) {
                var ret = "";
                for (var it in data) {
                    ret +=
                        encodeURIComponent(it) + "=" + encodeURIComponent(data[it]) + "&";
                }
                if (ret !== "") {
                    ret = ret.substr(0, ret.length - 1);
                }
                return ret;
            }
        ];
    };
    /**
     * formdata请求
     * @param url
     * @param option
     * @param configs
     */
    requestBase.prototype.serviceFormData = function (url, option, configs) {
        var datas = new FormData();
        _.mapKeys(option.data, function (value, key) {
            datas.append(key, value);
        });
        return axios
            .post(url, datas, { headers: configs, responseType: "arraybuffer" })
            .then(function (response) { return response.data; })
            .catch(function (errors) { return console.error(errors); });
    };
    /**
     * 替换模块{}
     * @param url
     * @param param
     */
    requestBase.prototype.paramTemplate = function (url, param) {
        if (_.isObject(param) &&
            typeof param === "object" &&
            /{([\s\S]+?)}/g.test(url)) {
            url = _.template(url, { interpolate: /{([\s\S]+?)}/g })(param);
        }
        return url;
    };
    /**
     * 错误信息
     *
     *  if (res.response && res.response.status === 400) {
          msg = res.response.data.Message[0];
        }
     */
    requestBase.prototype.requestError = function (res) {
        var msg = i18n.t('errorMsg.error').toString();
        var response = res.response, message = res.message;
        console.log('response, message', response, message);
        // 导入文件错误信息
        var filterError = function (ID) {
            var notifyMsg = i18n.t('errorMsg.template').toString();
            if (ID) {
                notifyMsg = "\u5BFC\u5165\u65F6\u53D1\u751F\u9519\u8BEF, \u8BF7\u67E5\u770B<a style=\"text-decoration: underline;\" href=\"/api/_file/downloadFile/" + ID + "\"><i>\u9519\u8BEF\u6587\u4EF6</i></a>";
            }
            Notification({
                title: i18n.t('errorMsg.import').toString(),
                dangerouslyUseHTMLString: true,
                type: "error",
                message: notifyMsg
            });
        };
        // 错误类型判断
        if (response) {
            var _a = response.data || response, Message = _a.Message, Form_1 = _a.Form;
            if (Message && Message.length > 0) {
                msg = Message[0];
            }
            else if (Form_1 && Form_1["Entity.Import"]) {
                filterError(Form_1["Entity.ErrorFileId"]);
                return;
            }
            else if (Form_1 && Form_1 !== {}) {
                var cxts = Object.keys(Form_1).map(function (key) { return Form_1[key]; });
                msg = cxts.join(',');
            }
            else {
                msg = response.data;
            }
        }
        else if (message) {
            msg = message;
        }
        Notification.error({
            title: i18n.t("errorMsg.msg").toString(),
            message: msg
        });
    };
    return requestBase;
}());
var rBase = new requestBase();
var _request = function (option, serverHost) {
    var url = serverHost ? serverHost : "" + option.url;
    url = rBase.paramTemplate(url, option.data);
    var axiosReq = {
        method: option.method,
        url: url,
        data: {},
        params: {},
        headers: {
            "Content-Type": option.contentType || contentType.json,
            "Accept-Language": AppModule.language
        }
    };
    var data = rBase.requestData(option.data);
    if (["POST", "PUT"].includes(option.method.toUpperCase())) {
        axiosReq.data = data;
    }
    else {
        axiosReq.params = data;
    }
    if (option.contentType === contentType.stream) {
        axiosReq["responseType"] = "blob";
        axiosReq.data = option.data;
    }
    else if (option.contentType === contentType.multipart) {
        return rBase.serviceFormData(url, option, axiosReq.headers);
    }
    else if (option.contentType === contentType.form) {
        axiosReq["transformRequest"] = rBase.transformReq();
    }
    return axios(__assign({}, axiosReq))
        .then(function (res) {
        if (option.contentType === contentType.stream) {
            return res;
        }
        return res.data;
    })
        .catch(function (error) {
        if (!option.blockError) {
            rBase.requestError(error);
        }
        throw error;
    });
};
export default _request;
//# sourceMappingURL=service.js.map