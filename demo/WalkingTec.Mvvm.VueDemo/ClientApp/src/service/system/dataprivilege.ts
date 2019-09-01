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
    url: reqPath + "add",
    method: "post"
};
// 列表
const search = {
    url: reqPath + "search",
    method: "post",
    dataType: "array"
};

// 删除
const deleted = {
    url: reqPath + "Delete/{ID}",
    method: "get"
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

export default {
    search,
    add,
    deleted,
    batchDelete,
    edit,
    privilegesList
};
