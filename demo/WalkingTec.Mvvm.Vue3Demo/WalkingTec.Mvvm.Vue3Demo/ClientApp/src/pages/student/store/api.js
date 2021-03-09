import config from "@/config/index";
import { contentType } from "@/config/enum";
var reqPath = config.headerApi + "/student/";
// 列表
var search = {
    url: reqPath + "search",
    method: "post",
    dataType: "array"
};
// 添加
var add = {
    url: reqPath + "Add",
    method: "post"
};
// 删除
var deleted = {
    url: reqPath + "Delete/{ID}",
    method: "get"
};
// 批量删除
var batchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 修改
var edit = {
    url: reqPath + "Edit",
    method: "put"
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
var getMajor = {
    url: reqPath + "getMajors",
    method: "get",
    dataType: "array"
};
export default {
    getMajor: getMajor,
    search: search,
    add: add,
    deleted: deleted,
    batchDelete: batchDelete,
    edit: edit,
    detail: detail,
    exportExcel: exportExcel,
    exportExcelByIds: exportExcelByIds,
    getExcelTemplate: getExcelTemplate,
    imported: imported
};
//# sourceMappingURL=api.js.map