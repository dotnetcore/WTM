/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-04-02 11:49:08
 * @modify date 2021-04-02 11:49:08
 * @desc 用户管理
 */
import { AjaxBasics, globalProperties } from "@/client";
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
@BindAll()
export class FilesController {
    $ajax: AjaxBasics;
    options = {
        fileUpload: {
            url: "/api/_file/upload",
            method: "post"
        },
        fileDelete: {
            url: "/api/_file/DeletedFile/{id}",
            method: "get"
        },
        fileGet: {
            url: "/api/_file/getFile",
            method: "get"
        },
        fileDownload: {
            url: "/api/_file/downloadFile/{id}",
            method: "get"
        }
    }
    async onInit() {
        this.$ajax = globalProperties.$Ajax;
    }
    /**
     * 获取下载地址
     * @returns 
     */
    getUploadUrl() {
        return this.options.fileUpload.url
    }
    /**
     * 删除文件
     * @param body 
     * @returns 
     */
    deleteFiles(body) {
        return this.$ajax.request(lodash.assign(body, this.options.fileDelete))
    }
    /**
    * 获取文件
    * @param id
    */
    getDownloadUrl(id) {
        if (id) {
            return AjaxBasics.onCompatibleUrl(lodash.assign({ body: { id } }, this.options.fileDownload), this.$ajax.options).url;
        }
    }

}