/**
 * 接口
 */

const indextest = {
    url: "/index/test",
    method: "get"
};
// 查询
const searchList = {
    url: "/_frameworkuserbase/search",
    action: "get",
    method: "post"
};

// 查询详情
const frameworkuserbase = ({ id }) => {
    return {
        url: `/_frameworkuserbase/${id}`,
        method: "get"
    };
};
// 角色列表
const frameworkRolesList = {
    url: "/_FrameworkUserBase/GetFrameworkRoles",
    method: "get"
};
// 组列表
const frameworkGroupsList = {
    url: "/_FrameworkUserBase/GetFrameworkGroups",
    method: "get"
};

export default {
    indextest,
    searchList,
    frameworkuserbase,
    frameworkRolesList,
    frameworkGroupsList
};
