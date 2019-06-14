/**
 * server封装，所有请求都走这里
 * option = {
 *      url:请求地址
 *      method: 'post' // 请求方式，post\get\put\delete
 *      data: {}, // 参数
 * }
 * service(option)
            .then(data => {
                console.log('then', data);
            })
            .catch(data => {
                console.log('catch', data);
            });
 */

import axios from "axios";
import config from "@/config/index";
import { Notification } from "element-ui"; // Message,

const service = (option, serverHost?) => {
    const originalData = option.data || {};
    const url = serverHost ? serverHost : config.serverHost + option.url;
    const req = {
        method: option.method,
        url: url,
        data: {},
        params: {},
        headers: {}
    };
    const data = {};
    for (const key in originalData) {
        if (
            originalData[key] !== null &&
            originalData[key] !== undefined &&
            originalData[key] !== ""
        ) {
            data[key] = originalData[key];
        }
    }
    if (option.method === "post") {
        // 针对参数类型是对象（包含数组）
        req.data = data;
        req.headers = {
            // "Content-Type": "application/x-www-form-urlencoded"
            "Content-Type": "application/json"
        };
        // req.transformRequest = [
        //     function(data) {
        //         let ret = "";
        //         for (const it in data) {
        //             ret +=
        //                 encodeURIComponent(it) +
        //                 "=" +
        //                 encodeURIComponent(data[it]) +
        //                 "&";
        //         }
        //         if (ret !== "") {
        //             ret = ret.substr(0, ret.length - 1);
        //         }
        //         return ret;
        //     }
        // ];
    } else {
        req.params = data;
    }
    return axios({ ...req })
        .then(res => {
            console.log("res::", res);
            return res.data;
        })
        .catch(res => {
            console.log("error", res);
            Notification.error({
                title: "错误",
                message: res
            });
            throw res;
        });
};
export default service;
