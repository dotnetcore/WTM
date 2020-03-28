import config from "@/config/index";
import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_FrameworkUserBase/";

// 列表
const search = {
  url: reqPath + "search",
  method: "post",
  dataType: "array"
};
// 添加
const add = {
  url: reqPath + "Add",
  method: "post"
};
// 批量删除
const batchDelete = {
  url: reqPath + "BatchDelete",
  method: "post"
};
// 修改
const edit = {
  url: reqPath + "Edit",
  method: "put"
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
// 角色
const getFrameworkRoles = {
  url: reqPath + "GetFrameworkRoles",
  method: "get",
  dataType: "array"
};
// 用户组
const getFrameworkGroups = {
  url: reqPath + "GetFrameworkGroups",
  method: "get",
  dataType: "array"
};

// 导入
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
  getFrameworkRoles,
  getFrameworkGroups,
  imported
};
