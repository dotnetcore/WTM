import Vue from "vue";
import Router from "vue-router";
import Layout from "@/components/layout/index.vue";
import { AppModule } from "@/store/modules/app";
Vue.use(Router);
export var constantRoutes = [
    {
        path: "/",
        component: Layout,
        redirect: "/dashboard",
        children: [
            {
                path: "dashboard",
                component: function () {
                    var languagePage = AppModule.language === 'en' ? '.en' : '';
                    return import(/* webpackChunkName: "dashboard" */ "@/pages/dashboard/index" + languagePage + ".vue").catch(function (err) {
                        return import(/* webpackChunkName: "dashboard" */ "@/pages/dashboard/index.vue");
                    });
                },
                name: "Dashboard",
                meta: {
                    title: "dashboard",
                    icon: "el-icon-odometer",
                    affix: true
                }
            }
        ]
    },
    {
        path: "/404",
        component: function () { return import("@/pages/error-page/404.vue"); },
        meta: { hidden: true }
    },
    {
        path: "/demo",
        component: function () { return import("@/pages/demo/index.vue"); },
        meta: { hidden: true }
    }
];
var createRouter = function () {
    return new Router({
        // mode: 'history',
        scrollBehavior: function (to, from, savedPosition) {
            if (savedPosition) {
                return savedPosition;
            }
            else {
                return { x: 0, y: 0 };
            }
        },
        base: process.env.BASE_URL,
        routes: constantRoutes
    });
};
var router = createRouter();
export function resetRouter() {
    var newRouter = createRouter();
    router.matcher = newRouter.matcher; // reset router
}
export default router;
//# sourceMappingURL=index.js.map