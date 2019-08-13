/**
 *  菜单数据
 */
const dataManageItems = [
    {
        name: "日志管理",
        meta: {
            icon: "el-icon-info"
        },
        path: "/test/index",
        component: () => import("@/pages/index/test/index.vue")
    },
    {
        name: "用户管理",
        meta: {
            icon: "el-icon-user-solid"
        },
        path: "/user/index",
        component: () => import("@/pages/index/user/index.vue")
    },
    {
        name: "角色管理",
        meta: {
            icon: "el-icon-s-tools"
        },
        path: "/test/index",
        component: () => import("@/pages/index/test/index.vue")
    },
    {
        name: "用户组管理",
        meta: {
            icon: "el-icon-s-custom"
        },
        path: "/test/index",
        component: () => import("@/pages/index/test/index.vue")
    },
    {
        name: "菜单管理",
        meta: {
            icon: "el-icon-s-fold"
        },
        path: "/test/index",
        component: () => import("@/pages/index/test/index.vue")
    },
    {
        name: "数据权限",
        meta: {
            icon: "el-icon-s-check"
        },
        path: "/test/index",
        component: () => import("@/pages/index/test/index.vue")
    }
];

export default dataManageItems;
