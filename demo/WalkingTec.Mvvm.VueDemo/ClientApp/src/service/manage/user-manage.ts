// 查询
const tableData = params => ({
    url: "/nsp-apis/uds/in/user/v2/users",
    method: "get",
    data: params
});

export default {
    tableData
};
