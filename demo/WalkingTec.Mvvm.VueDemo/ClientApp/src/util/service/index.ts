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
import { contentType } from "@/config/enum";
import { AppModule } from "@/store/modules/app";
import requestBase from './request-base';

const rBase = new requestBase();

export default (option, serverHost?) => {
  let url = serverHost ? serverHost : "" + option.url;
  url = rBase.paramTemplate(url, option.data);
  const axiosReq: AxiosRequestConfig = {
    method: option.method,
    url: url,
    data: {},
    params: {},
    headers: {
      "Content-Type": option.contentType || contentType.json,
      "Accept-Language": AppModule.language
    }
  };
  const data = rBase.requestData(option.data);
  if (["POST", "PUT"].includes(option.method.toUpperCase())) {
    axiosReq.data = data;
  } else {
    axiosReq.params = data;
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
      if (option.contentType === contentType.stream) {
          return res;
      }
      return res.data;
    })
    .catch(error => {
      rBase._handleFail(error);
      throw error;
    });
};
