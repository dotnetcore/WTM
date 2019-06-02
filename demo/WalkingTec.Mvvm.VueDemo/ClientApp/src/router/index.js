import Vue from "vue";
import Router from "vue-router";

// import index from "../pages/index/index";
import test from "../pages/index/test/index";

Vue.use(Router);
const router = new Router({
    mode: "hash", // 'history',
    routes: [
        {
            name: "test",
            path: "*",
            component: test
        },
        {
            name: "index",
            path: "/index",
            component: () => import("../pages/index/index")
        }
    ]
});
export default router;
