import lodash from 'lodash'
import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import RootStore from './rootStore'
import views, { Basics } from './views'
import exception from "./components/other/exception.vue"
import Globalconfig from './global.config';
Vue.use(VueRouter);
const tabsPage = Globalconfig.settings.tabsPage;
// // 命名 组件 tabpages 使用
// let components = {
//   "home": Basics.home,
//   '404': exception,
//   'external':Basics.external,
// };
const pageRoutes: RouteConfig[] = lodash.map(views, (value) => {
  const pageKey = lodash.snakeCase(value.path);
  // lodash.set(components, pageKey, value.component);
  let page = {
    path: value.path,
    name: value.name,
    // meta: lodash.merge({ pageKey }, value),
    // props: value,// controller: value.controller,
    component: value.component,
    // components
  };
  if (!tabsPage) {
    lodash.unset(page, 'components')
  }
  return page
});

const routes: RouteConfig[] = [
  {
    path: '/',
    name: "Home",
    // meta: { pageKey: 'home' },
    component:Basics.home,
    // components
  },
  {
    path: '/external/:url',
    name: "external",
    // meta: { pageKey: 'external' },
    component:Basics.external,
    // components
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
});

// router.beforeEach((to, from, next) => {
//   // console.log("TCL: from " + RootStore.UserStore.Loading, from);
//   // console.log("TCL: to", to);
//   // if (RootStore.UserStore.Loading) {
//   //   next(false)
//   // } else {
//   next()
//   // }
// })
// 登陆成功 注册路由
RootStore.UserStore.UserObservable.subscribe((entitie) => {
  if ((!entitie.Loading) && entitie.OnlineState) {
    router.addRoutes([
      ...pageRoutes,
      {
        path: '*',
        // redirect: "/"
        component: exception,
        // components
      }
    ]);
    console.table(pageRoutes, ['path', 'name'])
  }
});

export default router
