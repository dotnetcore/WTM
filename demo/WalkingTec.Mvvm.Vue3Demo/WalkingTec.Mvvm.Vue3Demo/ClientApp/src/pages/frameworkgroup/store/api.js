import config from "@/config/index";
import { contentType } from "@/config/enum";
var reqPath = config.headerApi + "/_FrameworkGroup/";
// 列表
var search = {
    url: reqPath + "Search",
    method: "post",
    dataType: "array"
};
// 详情
var detail = {
    url: reqPath + "{ID}",
    method: "get"
};
// 添加
var add = {
    url: reqPath + "Add",
    method: "post"
};
var edit = {
    url: reqPath + "Edit",
    method: "put"
};
// 批量删除
var batchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 导出
var exportExcel = {
    url: reqPath + "ExportExcel",
    method: "post",
    contentType: contentType.stream
};
// 多选导出
var exportExcelByIds = {
    url: reqPath + "ExportExcelByIds",
    method: "post",
    contentType: contentType.stream
};
// 获取模版
var getExcelTemplate = {
    url: reqPath + "GetExcelTemplate",
    method: "get",
    contentType: contentType.stream
};
var imported = {
    url: reqPath + "Import",
    method: "post"
};
export default {
    search: search,
    add: add,
    batchDelete: batchDelete,
    edit: edit,
    detail: detail,
    exportExcel: exportExcel,
    exportExcelByIds: exportExcelByIds,
    getExcelTemplate: getExcelTemplate,
    imported: imported
};
//# sourceMappingURL=api.js.map