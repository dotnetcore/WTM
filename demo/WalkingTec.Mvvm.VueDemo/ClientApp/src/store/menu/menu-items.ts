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
                path: "/userManage",
                url: "/user-manage/index",
                component: () => import("@/views/user-manage/index.vue")
            },
            {
                name: "订单管理",
                meta: {},
                path: "/orderManage",
                url: "/order-manage/index",
                component: () => import("@/views/order-manage/index.vue")
            }
        ]
    },
    {
        name: "设置",
        meta: {
            icon: "el-icon-setting"
        },
        path: "",
        children: [
            {
                name: "平台信息",
                meta: {},
                path: "/shopManage",
                url: "/shop-manage/index",
                component: () => import("@/views/shop-manage/index.vue")
            },
            {
                name: "二维码",
                meta: {},
                path: "/qrCode",
                url: "/qr-code/index",
                component: () => import("@/views/qr-code/index.vue")
            }
        ]
    }
];

export default dataManageItems;
