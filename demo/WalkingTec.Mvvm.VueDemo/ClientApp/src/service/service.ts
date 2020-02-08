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

import axios, { AxiosRequestConfig } from "axios";
import { Notification } from "element-ui"; // Message,
import { contentType } from "@/config/enum";

class requestBase {
  constructor() {}
  /**
   * 请求参数类型
   * @param originalData
   */
  requestData(originalData = {}) {
    const data = originalData;
    if (!_.isArray(originalData)) {
      for (const key in originalData || {}) {
        if (
          originalData[key] !== null &&
          originalData[key] !== undefined &&
          originalData[key] !== ""
        ) {
          data[key] = originalData[key];
        }
      }
    }
    return data;
  }
  /**
   * Transform 请求
   */
  transformReq() {
    return [
      function(data) {
        let ret = "";
        for (const it in data) {
          ret +=
            encodeURIComponent(it) + "=" + encodeURIComponent(data[it]) + "&";
        }
        if (ret !== "") {
          ret = ret.substr(0, ret.length - 1);
        }
        return ret;
      }
    ];
  }
  /**
   * formdata请求
   * @param url
   * @param option
   * @param configs
   */
  serviceFormData(url, option, configs) {
    const datas = new FormData();
    Object.keys(option.data).forEach(key => {
      datas.append(key, option.data[key]);
    });
    return axios
      .post(url, datas, { headers: configs, responseType: "arraybuffer" })
      .then(response => response.data)
      .catch(errors => console.error(errors));
  }
  /**
   * 替换模块{}
   * @param url
   * @param param
   */
  paramTemplate(url, param) {
    if (
      _.isObject(param) &&
      typeof param === "object" &&
      /{([\s\S]+?)}/g.test(url)
    ) {
      url = _.template(url, { interpolate: /{([\s\S]+?)}/g })(param);
    }
    return url;
  }
  /**
   * 错误信息
   *
   *  if (res.response && res.response.status === 400) {
        msg = res.response.data.Message[0];
      }
   */
  requestError(res) {
    let msg = "接口错误";
    const { response, data, message } = res;
    // 导入文件错误信息
    const filterError = (ID?: string) => {
      let notifyMsg: string = "模版错误！";
      if (ID) {
        notifyMsg = `导入时发生错误, 请查看<a style="text-decoration: underline;" href="/api/_file/downloadFile/${ID}"><i>错误文件</i></a>`;
      }
      Notification({
        title: "导入失败",
        dangerouslyUseHTMLString: true,
        type: "error",
        message: notifyMsg
      });
    };
    // 错误类型判断
    if (response) {
      const { Message, Form } = response.data;
      if (Message) {
        msg = Message[0];
      } else if (Form["Entity.Import"]) {
        filterError(Form["Entity.ErrorFileId"]);
        return;
      }
    } else if (data) {
      msg = data.message;
    } else if (message) {
      msg = message;
    }
    Notification.error({
      title: "错误",
      message: msg
    });
  }
}

const rBase = new requestBase();

const _request = (option, serverHost?) => {
  let url = serverHost ? serverHost : "" + option.url;
  url = rBase.paramTemplate(url, option.data);
  const axiosReq: AxiosRequestConfig = {
    method: option.method,
    url: url,
    data: {},
    params: {},
    headers: {
      "Content-Type": option.contentType || contentType.json
    }
  };
  const data = rBase.requestData(option.data);
  if (["POST", "PUT"].includes(option.method.toUpperCase())) {
    axiosReq.data = data;
  } else {
    axiosReq.params = data;
  }
  // 返回图片
  if (option.isBuffer) {
    axiosReq["responseType"] = "arraybuffer";
  }
  if (option.contentType === contentType.stream) {
    axiosReq["responseType"] = "blob";
    axiosReq.data = option.data;
  } else if (option.contentType === contentType.multipart) {
    return rBase.serviceFormData(url, option, axiosReq.headers);
  } else if (option.contentType === contentType.form) {
    axiosReq["transformRequest"] = rBase.transformReq();
  }
  return axios({ ...axiosReq })
    .then(res => {
      const response = res.data;
      if (option.isBuffer) {
        return response;
      }
      return response;
    })
    .catch(res => {
      rBase.requestError(res);
      throw res;
    });
};
export default _request;
