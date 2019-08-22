/**
 *  菜单数据
 */

const dataManageItems = [
    {
        name: "管理",
        meta: {
            icon: "el-icon-coin"
        },
        path: "",
        children: [
            {
                name: "用户管理",
                meta: {},
                path: "/test"
                // component: () => import("@/views/test.vue")
            }
        ]
    },
    {
        name: "系统管理",
        meta: {
            icon: "el-icon-setting"
        },
        path: "system", // System
        children: []
    }
];

export default dataManageItems;
