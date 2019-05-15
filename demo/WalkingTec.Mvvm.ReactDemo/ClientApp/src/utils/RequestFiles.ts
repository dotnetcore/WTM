/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:37
 * @modify date 2018-09-12 18:52:37
 * @desc [description]
*/
import { message, notification } from "antd";
import moment from 'moment';
import { ajax, AjaxRequest } from "rxjs/ajax";
import { Request } from './Request';
/** 文件服务器 */
const files = {
    fileUpload: {
        src: "/api/_file/upload",
        method: "post"
    },
    fileDelete: {
        src: "/api/_file/deleteFile/{id}",
        method: "get"
    },
    fileGet: {
        src: "/api/_file/getFile",
        method: "get"
    },
    fileDownload: {
        src: "/api/_file/downloadFile",
        method: "get"
    }
};
export class RequestFiles extends Request {
    /**
     * 
     * @param target 替换默认地址前缀
     * @param newMap 替换默认过滤函数
     */
    constructor(target?, public newMap?) {
        super(target)
    }
    /**
     * 文件服务器地址
     */
    FileTarget = Request.compatibleUrl(this.target, files.fileUpload.src);
    /**
     * 删除文件
     * @param id 
     */
    async onFileDelete(id) {
        const res = await this.ajax({ ...files.fileDelete, body: { id } });
        if (res) {
        }
        return res
    }
    /**
     * 获取文件
     * @param id 
     */
    onFileUrl(id, target = this.target) {
        if (id) {
            const src = files.fileGet.src;
            return `${Request.compatibleUrl(target, src)}/${id}`
        }
    }
    /**
     * 获取 下周 文件
     * @param id 
     */
    onFileDownload(id, target = this.target) {
        if (id) {
            const src = files.fileDownload.src;
            return `${Request.compatibleUrl(target, src)}/${id}`
        }
    }
    /** 文件获取状态 */
    private downloadLoading = false
    /**
     * 下载文件
     * @param AjaxRequest 
     * @param fileType 
     * @param fileName 
     */
    async download(AjaxRequest: AjaxRequest, fileType = '.xls', fileName = moment().format("YYYY_MM_DD_hh_mm_ss")) {
        this.getHeaders();
        if (this.downloadLoading) {
            return message.warn('文件获取中，请勿重复操作~')
        }
        this.downloadLoading = true;
        Request.NProgress()
        AjaxRequest.url = Request.compatibleUrl(this.target, AjaxRequest.url);
        AjaxRequest = {
            // url: url,
            method: "post",
            responseType: "blob",
            timeout: this.timeout,
            headers: this.getHeaders(),
            ...AjaxRequest
        }
        /**
         * get 方式 直接打开窗口
         */
        // if (lodash.isEqual(lodash.toLower('get'), AjaxRequest.method)) {
        //     window.open(AjaxRequest.url)
        // } else {
        // post
        if (AjaxRequest.body) {
            AjaxRequest.body = Request.formatBody(AjaxRequest.body, "body", AjaxRequest.headers);
        }
        try {
            const result = await ajax(AjaxRequest).toPromise();
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
        // }
        Request.NProgress("done")
        this.downloadLoading = false;
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
}
export default new RequestFiles();
