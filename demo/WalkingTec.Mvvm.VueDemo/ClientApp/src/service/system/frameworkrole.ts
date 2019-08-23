import config from "@/config/index";
import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_FrameworkRole/";
// 列表
const frameworkroleSearchList = {
    url: reqPath + "Search",
    method: "post"
};
// 添加
const frameworkroleAdd = {
    url: reqPath + "add",
    method: "post"
};
// 删除
const frameworkroleDelete = ({ ID }) => {
    return {
        url: reqPath + "Delete/" + ID,
        method: "get"
    };
};
// 批量删除
const frameworkroleBatchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 修改
const frameworkroleEdit = {
    url: reqPath + "Edit",
    method: "put"
};
// 详情
const frameworkrole = ({ ID }) => {
    return {
        url: reqPath + ID,
        method: "get"
    };
};
const frameworkroleExportExcel = {
    url: reqPath + "ExportExcel",
    method: "post",
    contentType: contentType.stream
};
const frameworkroleExportExcelByIds = {
    url: reqPath + "ExportExcelByIds",
    method: "post",
    contentType: contentType.stream
};
const frameworkroleGetExcelTemplate = {
    url: reqPath + "GetExcelTemplate",
    method: "get"
};
// 角色
const frameworkroleGetFrameworkRoles = {
    url: reqPath + "GetFrameworkRoles",
    method: "get",
    dataType: "array"
};
// 用户组
const frameworkroleGetFrameworkGroups = {
    url: reqPath + "GetFrameworkGroups",
    method: "get",
    dataType: "array"
};

const frameworkroleEditPrivilege = {
    url: reqPath + "EditPrivilege",
    method: "put"
};

const frameworkroleGetPageActions = ({ ID }) => {
    return {
        url: reqPath + "GetPageActions/" + ID,
        method: "get"
    };
};

export default {
    frameworkroleSearchList,
    frameworkroleAdd,
    frameworkroleDelete,
    frameworkroleBatchDelete,
    frameworkroleEdit,
    frameworkrole,
    frameworkroleExportExcel,
    frameworkroleExportExcelByIds,
    frameworkroleGetExcelTemplate,
    frameworkroleGetFrameworkRoles,
    frameworkroleGetFrameworkGroups,
    frameworkroleEditPrivilege,
    frameworkroleGetPageActions
};
