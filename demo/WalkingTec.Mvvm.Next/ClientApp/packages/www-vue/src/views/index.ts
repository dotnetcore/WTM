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
    frameworkuserbase: {
        name: '用户管理',
        path: '/frameworkuser',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkUser',
        component: () => import("./frameworkuser/index.vue")
    }
}