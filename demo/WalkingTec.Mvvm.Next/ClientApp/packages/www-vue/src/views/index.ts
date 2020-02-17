import login from "./login/index.vue";
export const Basics = {
    login
};
export default {
    frameworkuserbase: {
        name: '用户管理',
        path: '/frameworkuser',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkUser',
        component: () => import("./frameworkuser/index.vue")
    }
}