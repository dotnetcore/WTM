import Vue from "vue";
import Router, { RouteConfig } from "vue-router";
/* Layout */
import Layout from "@/layout/index.vue";

Vue.use(Router);

/**
  ConstantRoutes
  a base page that does not have permission requirements
  all roles can be accessed
*/
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
                component: () =>
                    import(/* webpackChunkName: "i18n-demo" */ "@/views/i18n-demo/index.vue"),
                name: "I18n",
                meta: {
                    title: "i18n",
                    icon: "international"
                }
            }
        ]
    }
];

/**
 * asyncRoutes
 * the routes that need to be dynamically loaded based on user roles
 */
export const asyncRoutes: RouteConfig[] = [
    /** when your routing map is too long, you can split it into small modules **/
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
    },
    {
        path: "*",
        redirect: "/404",
        meta: { hidden: true }
    }
];

const createRouter = () =>
    new Router({
        // mode: 'history',  // Disabled due to Github Pages doesn't support this, enable this if you need.
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

// Detail see: https://github.com/vuejs/vue-router/issues/1234#issuecomment-357941465
export function resetRouter() {
    const newRouter = createRouter();
    (router as any).matcher = (newRouter as any).matcher; // reset router
}

export default router;
