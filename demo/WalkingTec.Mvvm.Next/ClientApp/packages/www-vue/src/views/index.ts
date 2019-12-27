import login from "./login/index.vue";
export const Basics = {
    login
};
export default {
    // login,
    // home,
    user: {
        name: 'user',
        path: '/user',
        controller: 'WalkingTec.Mvvm.Admin.Api,ActionLog',
        component: () => import("./user/index.vue")
    },
    test: {
        name: 'test',
        path: '/test',
        controller: 'WalkingTec.Mvvm.Admin.Api,ActionLog',
        component: () => import("./test/index.vue")
    },
    test2: {
        name: 'actionlog',
        path: '/actionlog',
        controller: 'WalkingTec.Mvvm.Admin.Api,ActionLog',
        component: () => import("./test/index.vue")
    },
}