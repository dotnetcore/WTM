import config from "@/config/index";
import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_FrameworkGroup/";
// 列表
const frameworkgroupSearchList = {
    url: reqPath + "Search",
    method: "post"
};
// 详情
const frameworkgroup = ({ ID }) => {
    return {
        url: reqPath + ID,
        method: "get"
    };
};
// 添加
const frameworkgroupAdd = {
    url: reqPath + "add",
    method: "post"
};
const frameworkgroupEdit = {
    url: reqPath + "Edit",
    method: "put"
};
// 批量删除
const frameworkgroupBatchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 删除 -------
const frameworkgroupDelete = ({ ID }) => {
    return {
        url: reqPath + "Delete/" + ID,
        method: "get"
    };
};
// 导出
const frameworkgroupExportExcel = {
    url: reqPath + "ExportExcel",
    method: "post",
    contentType: contentType.stream
};
// 多选导出
const frameworkgroupExportExcelByIds = {
    url: reqPath + "ExportExcelByIds",
    method: "post",
    contentType: contentType.stream
};
// 获取模版
const frameworkgroupGetExcelTemplate = {
    url: reqPath + "GetExcelTemplate",
    method: "get"
};

const frameworkgroupImport = {
    url: reqPath + "Import",
    method: "post"
};

export default {
    frameworkgroupSearchList,
    frameworkgroupAdd,
    frameworkgroupDelete,
    frameworkgroupBatchDelete,
    frameworkgroupEdit,
    frameworkgroup,
    frameworkgroupExportExcel,
    frameworkgroupExportExcelByIds,
    frameworkgroupGetExcelTemplate,
    frameworkgroupImport
};
