import Vue from "vue";
import Router, { RouteConfig } from "vue-router";
import Layout from "@/layout/index.vue";
Vue.use(Router);

export const constantRoutes: RouteConfig[] = [
    {
        path: "/",
        component: Layout,
        redirect: "/dashboard",
        children: [
            {
                path: "dashboard",
                component: () =>
                    import(/* webpackChunkName: "dashboard" */ "@/views/dashboard/index.vue"),
                name: "Dashboard",
                meta: {
                    title: "dashboard",
                    icon: "dashboard",
                    affix: true
                }
            }
        ]
    },
    {
        path: "/i18n",
        component: Layout,
        children: [
            {
                path: "index",
                component: () => import("@/views/i18n-demo/index.vue"),
                name: "I18n",
                meta: {
                    title: "i18n",
                    icon: "international"
                }
            }
        ]
    },
    {
        path: "external-link",
        component: Layout,
        children: [
            {
                path: "https://github.com/Armour/vue-typescript-admin-template",
                meta: {
                    title: "externalLink",
                    icon: "link"
                }
            }
        ]
    }
];

export const asyncRoutes: RouteConfig[] = [
    {
        path: "*",
        redirect: "/404",
        meta: { hidden: true }
    }
];

const createRouter = () =>
    new Router({
        // mode: 'history',
        scrollBehavior: (to, from, savedPosition) => {
            if (savedPosition) {
                return savedPosition;
            } else {
                return { x: 0, y: 0 };
            }
        },
        base: process.env.BASE_URL,
        routes: constantRoutes
    });

const router = createRouter();

export function resetRouter() {
    const newRouter = createRouter();
    (router as any).matcher = (newRouter as any).matcher; // reset router
}

export default router;
