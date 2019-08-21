import config from "@/config/index";

const privilegesList = {
    url: config.headerApi + "/_DataPrivilege/GetPrivileges",
    method: "get"
};
// 添加
const dataprivilegeAdd = {
    url: config.headerApi + "/_DataPrivilege/add",
    method: "post"
};
// 列表
const dataprivilegeSearchList = {
    url: config.headerApi + "/_DataPrivilege/search",
    method: "post"
};

// 删除
const dataprivilegeDelete = ({ id }) => {
    return {
        url: `${config.headerApi}/_DataPrivilege/Delete/${id}`,
        method: "get"
    };
};
// 批量删除
const dataprivilegeBatchDelete = {
    url: config.headerApi + "/_DataPrivilege/BatchDelete",
    method: "post"
};
// 修改
const dataprivilegeEdit = {
    url: config.headerApi + "/_DataPrivilege/Edit",
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
