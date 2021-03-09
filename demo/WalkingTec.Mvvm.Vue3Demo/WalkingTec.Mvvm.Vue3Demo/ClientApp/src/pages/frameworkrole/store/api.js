import config from "@/config/index";
import { contentType } from "@/config/enum";
var reqPath = config.headerApi + "/_FrameworkRole/";
// 列表
var search = {
    url: reqPath + "Search",
    method: "post",
    dataType: "array"
};
// 添加
var add = {
    url: reqPath + "Add",
    method: "post"
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
// 角色
var getFrameworkRoles = {
    url: reqPath + "GetFrameworkRoles",
    method: "get",
    dataType: "array"
};
// 用户组
var getFrameworkGroups = {
    url: reqPath + "GetFrameworkGroups",
    method: "get",
    dataType: "array"
};
var editPrivilege = {
    url: reqPath + "EditPrivilege",
    method: "put"
};
// 权限列表
var getPageActions = {
    url: reqPath + "GetPageActions/{ID}",
    method: "get"
};
// 导入
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
    getFrameworkRoles: getFrameworkRoles,
    getFrameworkGroups: getFrameworkGroups,
    editPrivilege: editPrivilege,
    getPageActions: getPageActions,
    imported: imported
};
//# sourceMappingURL=api.js.map