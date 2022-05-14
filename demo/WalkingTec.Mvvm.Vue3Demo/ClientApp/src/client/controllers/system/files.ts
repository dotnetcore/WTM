/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-04-02 11:49:08
 * @modify date 2021-04-02 11:49:08
 * @desc 用户管理
 */
import { AjaxBasics, globalProperties } from "@/client";
import lodash from 'lodash';
import { saveAs } from 'file-saver';
import { BindAll } from 'lodash-decorators';
import { Regulars } from "../../helpers";
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
            method: "get",
            responseType: 'blob'
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
     *下载文件
     * @param body 
     * @returns 
     */
    async downloadFile(id,name){
        const res: any = await this.$ajax.request(lodash.assign({body:{ Id:id }}, this.options.fileDownload)).toPromise()
        const disposition = res.xhr.getResponseHeader('content-disposition');
        Regulars.filename.test(disposition);
        saveAs(res.response, name);
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
