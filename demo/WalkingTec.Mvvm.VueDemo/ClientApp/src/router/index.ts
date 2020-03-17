import Vue from "vue";
import Router, { RouteConfig } from "vue-router";
import Layout from "@/components/layout/index.vue";
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
          import(
            /* webpackChunkName: "dashboard" */ "@/pages/dashboard/index.vue"
          ),
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
    path: "/i18n",
    component: Layout,
    children: [
      {
        path: "index",
        component: () => import("@/pages/i18n-demo/index.vue"),
        name: "I18n",
        meta: {
          title: "i18n",
          icon: "el-icon-orange"
        }
      }
    ]
  },
  {
    path: "/demo",
    component: Layout,
    children: [
      {
        path: "index",
        component: () => import("@/pages/demo/index.vue"),
        name: "demo",
        meta: {
          title: "demo",
          icon: "el-icon-orange"
        }
      }
    ]
  },
  {
    path: "/external",
    component: Layout,
    children: [
      {
        path: "",
        component: () => import("@/pages/external/index.vue"),
        name: "外链",
        meta: {
          title: "externalLink",
          icon: "el-icon-link"
        },
        // props: { default: true, url: "" }
        props: router => {
          return { ...router.query };
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
