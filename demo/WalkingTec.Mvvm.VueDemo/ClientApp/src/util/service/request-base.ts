
import axios from "axios";
import { Notification } from "element-ui"; // Message,
import i18n from "@/lang";
export default class requestBase {
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
      _.mapKeys(option.data, (value, key) => {
        datas.append(key, value);
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
    _handleFail(res) {
      let msg: string = i18n.t('errorMsg.error').toString();
      const { response, message } = res;
      console.error(response, message)
      // 导入文件错误信息
      const filterError = (ID?: string) => {
        let notifyMsg: string = i18n.t('errorMsg.template').toString();
        if (ID) {
          notifyMsg = `导入时发生错误, 请查看<a style="text-decoration: underline;" href="/api/_file/downloadFile/${ID}"><i>错误文件</i></a>`;
        }
        Notification({
          title: i18n.t('errorMsg.import').toString(),
          dangerouslyUseHTMLString: true,
          type: "error",
          message: notifyMsg
        });
      };

      // 表单错误-页面需要解析错误
      if (response && _.isObject(response.data)) {
        const { Message, Form } = response.data;
        if (Message) {
          msg = Message.length > 0 ? Message[0] : Message;
        } else if (Form && Form["Entity.Import"]) {
          filterError(Form["Entity.ErrorFileId"]);
          return;
        } else if (Form && Form !== {}) {
          const cxts = Object.keys(Form).map(key => Form[key]);
          msg = cxts.join(',');
        } else {
          msg = response.data;
        }
      } else if (response) {
        const { Message } = response;
        msg = Message || message;
      }
      Notification.error({
        title: i18n.t("errorMsg.msg").toString(),
        message: msg
      });
    }
  }

