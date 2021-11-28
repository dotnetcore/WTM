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
            url: "/api/_file/DeletedFile/{Id}",
            method: "get"
        },
        getFileName: {
            url: "/api/_file/GetFileName/{Id}",
            method: "get",            
            responseType: "text"
        },
        fileGet: {
            url: "/api/_file/getFile",
            method: "get",
        },
        fileDownload: {
            url: "/api/_file/downloadFile/{Id}",
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
        return this.$ajax.request(lodash.assign({ body }, this.options.fileDelete)).toPromise()
    }

    /**
     * 获取文件名
     * @param body 
     * @returns 
     */
    getFileName(id){
        return this.$ajax.request(lodash.assign({body:{ Id:id }}, this.options.getFileName)).toPromise()
    }
    /**
    * 获取文件
    * @param id
    */
    getDownloadUrl(Id) {
        if (Id) {
            return AjaxBasics.onCompatibleUrl(lodash.assign({ body: { Id } }, this.options.fileDownload), this.$ajax.options).url;
        }
    }

}
