import config from "@/config/index";
// 上传
var upload = {
    url: config.headerApi + "/_file/upload",
    method: "post"
};
// 上传地址
var actionApi = "/api/_file/upload";
// 下载地址
var fileApi = "/api/_file/downloadFile/";
// 删除file
var deletedFile = {
    url: config.headerApi + "/_file/DeletedFile/{id}",
    method: "get",
    blockError: true
};
;
export { upload, actionApi, fileApi };
export default { upload: upload, deletedFile: deletedFile };
//# sourceMappingURL=upload.js.map