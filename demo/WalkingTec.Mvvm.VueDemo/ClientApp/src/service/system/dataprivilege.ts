import config from "@/config/index";

const privilegesList = {
    url: config.headerApi + "/_DataPrivilege/GetPrivileges",
    method: "get"
};
// 添加
const dataprivilegeAdd = {
    url: config.headerApi + "/_dataprivilege/add",
    method: "post"
};
// 列表
const dataprivilegeSearchList = {
    url: config.headerApi + "/_dataprivilege/search",
    method: "post"
};
export default {
    privilegesList,
    dataprivilegeSearchList,
    dataprivilegeAdd
};
