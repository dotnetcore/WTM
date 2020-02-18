import login from "./login/index.vue";
export const Basics = {
    login
};
export default {
    actionlog: {
        name: '日志管理',
        path: '/actionlog',
        controller: 'WalkingTec.Mvvm.Admin.Api,ActionLog',
        component: () => import("./actionlog/index.vue")
    },
    dataprivilege: {
        name: '数据权限',
        path: '/dataprivilege',
        controller: 'WalkingTec.Mvvm.Admin.Api,DataPrivilege',
        component: () => import("./dataprivilege/index.vue")
    },
    frameworkgroup: {
        name: '用户组管理',
        path: '/frameworkgroup',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkGroup',
        component: () => import("./frameworkgroup/index.vue"),
    },
    frameworkmenu: {
        name: '菜单管理',
        path: '/frameworkmenu',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkMenu',
        component: () => import("./frameworkmenu/index.vue"),
    },
    frameworkrole: {
        name: '角色管理',
        path: '/frameworkrole',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkRole',
        component: () => import("./frameworkrole/index.vue"),
    },
    frameworkuserbase: {
        name: '用户管理',
        path: '/frameworkuser',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkUser',
        component: () => import("./frameworkuser/index.vue")
    }
}