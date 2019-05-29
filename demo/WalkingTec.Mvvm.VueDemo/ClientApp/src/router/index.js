import Vue from "vue";
import Router from "vue-router";

import index from "../pages/index/index";

Vue.use(Router);
const router = new Router({
    // mode: 'history',
    routes: [
        {
            name: "index",
            path: "*",
            meta: { index: 1 },
            component: () => index
        }
    ]
});
export default router;
