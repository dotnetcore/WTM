import config from "@/config/index";
import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_actionlog/";

// 列表
const search = {
  url: reqPath + "Search",
  method: "post",
  dataType: "array"
};
// 批量删除
const batchDelete = {
  url: reqPath + "BatchDelete",
  method: "post"
};
// 详情
const detail = {
  url: reqPath + "{ID}",
  method: "get"
};
const exportExcel = {
  url: reqPath + "ExportExcel",
  method: "post",
  contentType: contentType.stream
};
const exportExcelByIds = {
  url: reqPath + "ExportExcelByIds",
  method: "post",
  contentType: contentType.stream
};
const getExcelTemplate = {
  url: reqPath + "GetExcelTemplate",
  method: "get",
  contentType: contentType.stream
};

// 导入
const imported = {
  url: reqPath + "Import",
  method: "post"
};
export default {
  search,
  batchDelete,
  detail,
  exportExcel,
  exportExcelByIds,
  getExcelTemplate,
  imported
};
