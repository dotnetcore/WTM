import config from "@/config/index";
import { contentType } from "@/config/enum";
var reqPath = config.headerApi + "/_actionlog/";
// 列表
var search = {
    url: reqPath + "Search",
    method: "post",
    dataType: "array"
};
// 批量删除
var batchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 详情
var detail = {
    url: reqPath + "{ID}",
    method: "get"
};
var exportExcel = {
    url: reqPath + "ExportExcel",
    method: "post",
    contentType: contentType.stream
};
var exportExcelByIds = {
    url: reqPath + "ExportExcelByIds",
    method: "post",
    contentType: contentType.stream
};
var getExcelTemplate = {
    url: reqPath + "GetExcelTemplate",
    method: "get",
    contentType: contentType.stream
};
// 导入
var imported = {
    url: reqPath + "Import",
    method: "post"
};
export default {
    search: search,
    batchDelete: batchDelete,
    detail: detail,
    exportExcel: exportExcel,
    exportExcelByIds: exportExcelByIds,
    getExcelTemplate: getExcelTemplate,
    imported: imported
};
//# sourceMappingURL=api.js.map