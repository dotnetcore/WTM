import config from "@/config/index";
const reqPath = config.headerApi + "/_DataPrivilege/";
// 下啦列表
const privilegesList = {
    url: reqPath + "GetPrivileges",
    method: "get",
    dataType: "array"
};
// 添加
const dataprivilegeAdd = {
    url: reqPath + "add",
    method: "post"
};
// 列表
const dataprivilegeSearchList = {
    url: reqPath + "search",
    method: "post",
    dataType: "array"
};

// 删除
const dataprivilegeDelete = {
    url: reqPath + "Delete/{ID}",
    method: "get"
};
// 批量删除
const dataprivilegeBatchDelete = {
    url: reqPath + "BatchDelete",
    method: "post"
};
// 修改
const dataprivilegeEdit = {
    url: reqPath + "Edit",
    method: "put"
};

export default {
    privilegesList,
    dataprivilegeSearchList,
    dataprivilegeAdd,
    dataprivilegeDelete,
    dataprivilegeBatchDelete,
    dataprivilegeEdit
};
