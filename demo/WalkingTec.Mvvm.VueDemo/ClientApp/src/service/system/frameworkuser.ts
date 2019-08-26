import config from "@/config/index";
import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_FrameworkUserBase/";

// 列表
const frameworkuserSearch = {
    url: reqPath + "search",
    method: "post",
    dataType: "array"
};
// 添加
const frameworkuserAdd = {
    url: reqPath + "add",
    method: "post"
};
// 删除
const frameworkuserDelete = {
    url: reqPath + "Delete/{ID}",
    method: "get"
};
// 批量删除
const frameworkuserBatchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 修改
const frameworkuserEdit = {
    url: reqPath + "Edit",
    method: "put"
};
// 详情
const frameworkuser = {
    url: reqPath + "{ID}",
    method: "get"
};
const frameworkuserExportExcel = {
    url: reqPath + "ExportExcel",
    method: "post",
    contentType: contentType.stream
};
const frameworkuserExportExcelByIds = {
    url: reqPath + "ExportExcelByIds",
    method: "post",
    contentType: contentType.stream
};
const frameworkuserGetExcelTemplate = {
    url: reqPath + "GetExcelTemplate",
    method: "get"
};
// 角色
const frameworkuserGetFrameworkRoles = {
    url: reqPath + "GetFrameworkRoles",
    method: "get",
    dataType: "array"
};
// 用户组
const frameworkuserGetFrameworkGroups = {
    url: reqPath + "GetFrameworkGroups",
    method: "get",
    dataType: "array"
};
export default {
    frameworkuserSearch,
    frameworkuserAdd,
    frameworkuserDelete,
    frameworkuserBatchDelete,
    frameworkuserEdit,
    frameworkuser,
    frameworkuserExportExcel,
    frameworkuserExportExcelByIds,
    frameworkuserGetExcelTemplate,
    frameworkuserGetFrameworkRoles,
    frameworkuserGetFrameworkGroups
};
