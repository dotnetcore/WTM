import config from "@/config/index";
import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_FrameworkMenu/";
// 列表
const frameworkmenuSearchList = {
    url: reqPath + "Search",
    method: "post",
    dataType: "array"
};
// 详情
const frameworkmenu = {
    url: reqPath + "{ID}",
    method: "get"
};
// 添加
const frameworkmenuAdd = {
    url: reqPath + "add",
    method: "post"
};
const frameworkmenuEdit = {
    url: reqPath + "Edit",
    method: "put"
};
// 批量删除
const frameworkmenuBatchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 删除 -------
const frameworkmenuDelete = {
    url: reqPath + "Delete/{ID}",
    method: "get"
};
// 导出
const frameworkmenuExportExcel = {
    url: reqPath + "ExportExcel",
    method: "post",
    contentType: contentType.stream
};
// 多选导出
const frameworkmenuExportExcelByIds = {
    url: reqPath + "ExportExcelByIds",
    method: "post",
    contentType: contentType.stream
};
// 获取模版
const frameworkmenuGetExcelTemplate = {
    url: reqPath + "GetExcelTemplate",
    method: "get"
};

const frameworkmenuSyncModel = {
    url: reqPath + "SyncModel",
    method: "get"
};
const frameworkmenuUnsetPages = {
    url: reqPath + "UnsetPages",
    method: "get"
};
const frameworkmenuRefreshMenu = {
    url: reqPath + "RefreshMenu",
    method: "get"
};
const frameworkmenuGetActionsByModel = {
    url: reqPath + "GetActionsByModel",
    method: "get"
};
const frameworkmenuGetFolders = {
    url: reqPath + "GetFolders",
    method: "get"
};

export default {
    frameworkmenuSearchList,
    frameworkmenuAdd,
    frameworkmenuDelete,
    frameworkmenuBatchDelete,
    frameworkmenuEdit,
    frameworkmenu,
    frameworkmenuExportExcel,
    frameworkmenuExportExcelByIds,
    frameworkmenuGetExcelTemplate,
    frameworkmenuSyncModel,
    frameworkmenuUnsetPages,
    frameworkmenuRefreshMenu,
    frameworkmenuGetActionsByModel,
    frameworkmenuGetFolders
};
