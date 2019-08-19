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
                path: "/user-manage/index",
                component: () => import("@/views/user-manage/index.vue")
            },
            {
                name: "订单管理",
                meta: {},
                path: "/order-manage/index",
                component: () => import("@/views/order-manage/index.vue")
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
