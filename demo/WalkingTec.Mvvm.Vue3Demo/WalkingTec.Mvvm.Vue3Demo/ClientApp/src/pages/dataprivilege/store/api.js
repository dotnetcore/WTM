import config from "@/config/index";
var reqPath = config.headerApi + "/_DataPrivilege/";
// 下啦列表
var privilegesList = {
    url: reqPath + "GetPrivileges",
    method: "get",
    dataType: "array"
};
// 添加
var add = {
    url: reqPath + "Add",
    method: "post"
};
// 详情
var get = {
    url: reqPath + "Get",
    method: "get"
};
// 列表
var search = {
    url: reqPath + "Search",
    method: "post",
    dataType: "array"
};
// 删除
var deleted = {
    url: reqPath + "Delete",
    method: "post"
};
// 修改
var edit = {
    url: reqPath + "Edit",
    method: "put"
};
// 导入
var imported = {
    url: reqPath + "Import",
    method: "post"
};
// 用户组
var getUserGroups = {
    url: reqPath + "GetUserGroups",
    method: "get",
    dataType: "array"
};
// 权限名称
var getPrivileges = {
    url: reqPath + "GetPrivileges",
    method: "get",
    dataType: "array"
};
// 角色
var getPrivilegeByTableName = {
    url: reqPath + "GetPrivilegeByTableName",
    method: "get",
    dataType: "array"
};
export default {
    search: search,
    add: add,
    deleted: deleted,
    edit: edit,
    privilegesList: privilegesList,
    imported: imported,
    getUserGroups: getUserGroups,
    getPrivileges: getPrivileges,
    getPrivilegeByTableName: getPrivilegeByTableName,
    get: get
};
//# sourceMappingURL=api.js.map