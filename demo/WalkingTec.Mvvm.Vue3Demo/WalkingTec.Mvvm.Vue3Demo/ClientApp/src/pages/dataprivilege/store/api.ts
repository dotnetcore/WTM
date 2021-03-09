import config from "@/config/index";
const reqPath = config.headerApi + "/_DataPrivilege/";
// 下啦列表
const privilegesList = {
  url: reqPath + "GetPrivileges",
  method: "get",
  dataType: "array"
};
// 添加
const add = {
  url: reqPath + "Add",
  method: "post"
};

// 详情
const get = {
  url: reqPath + "Get",
  method: "get"
};

// 列表
const search = {
  url: reqPath + "Search",
  method: "post",
  dataType: "array"
};
// 删除
const deleted = {
  url: reqPath + "Delete",
  method: "post"
};

// 修改
const edit = {
  url: reqPath + "Edit",
  method: "put"
};

// 导入
const imported = {
  url: reqPath + "Import",
  method: "post"
};

// 用户组
const getUserGroups = {
  url: reqPath + "GetUserGroups",
  method: "get",
  dataType: "array"
};
// 权限名称
const getPrivileges = {
  url: reqPath + "GetPrivileges",
  method: "get",
  dataType: "array"
};

// 角色
const getPrivilegeByTableName = {
  url: reqPath + "GetPrivilegeByTableName",
  method: "get",
  dataType: "array"
};

export default {
  search,
  add,
  deleted,
  edit,
  privilegesList,
  imported,
  getUserGroups,
  getPrivileges,
  getPrivilegeByTableName,
  get
};
