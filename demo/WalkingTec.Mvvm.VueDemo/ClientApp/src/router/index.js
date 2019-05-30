import Vue from "vue";
import Router from "vue-router";

import index from "../pages/index/index";
import login from "../pages/login/index";

Vue.use(Router);
const router = new Router({
    // mode: 'history',
    routes: [
        {
            name: "index",
            path: "*",
            component: () => index
        },
        {
            name: "login",
            path: "/login",
            component: () => login
        }
    ]
});
export default router;
