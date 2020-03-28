import config from "@/config/index";
import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_FrameworkGroup/";
// 列表
const search = {
  url: reqPath + "Search",
  method: "post",
  dataType: "array"
};
// 详情
const detail = {
  url: reqPath + "{ID}",
  method: "get"
};
// 添加
const add = {
  url: reqPath + "Add",
  method: "post"
};
const edit = {
  url: reqPath + "Edit",
  method: "put"
};
// 批量删除
const batchDelete = {
  url: reqPath + "BatchDelete",
  method: "post"
};
// 导出
const exportExcel = {
  url: reqPath + "ExportExcel",
  method: "post",
  contentType: contentType.stream
};
// 多选导出
const exportExcelByIds = {
  url: reqPath + "ExportExcelByIds",
  method: "post",
  contentType: contentType.stream
};
// 获取模版
const getExcelTemplate = {
  url: reqPath + "GetExcelTemplate",
  method: "get",
  contentType: contentType.stream
};

const imported = {
  url: reqPath + "Import",
  method: "post"
};

export default {
  search,
  add,
  batchDelete,
  edit,
  detail,
  exportExcel,
  exportExcelByIds,
  getExcelTemplate,
  imported
};
