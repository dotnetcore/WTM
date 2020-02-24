import lodash from 'lodash'
import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import RootStore from './rootStore'
import views, { Basics } from './views'
Vue.use(VueRouter)
const routes: RouteConfig[] = [
  // {
  //   path: '/login',
  //   name: 'login',
  //   component: Basics.login
  // },
  // {
  //   path: '/',
  //   name: 'home',
  //   component: views.user.component,
  // },
  // {
  //   path: '*',
  //   // redirect: "/"
  //   component: views.user.component
  // },
]
const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
});

router.beforeEach((to, from, next) => {
  // console.log("TCL: from " + RootStore.UserStore.Loading, from);
  // console.log("TCL: to", to);
  // if (RootStore.UserStore.Loading) {
  //   next(false)
  // } else {
  next()
  // }
})
// 登陆成功 注册路由
RootStore.UserStore.UserObservable.subscribe((entitie) => {
  if ((!entitie.Loading) && entitie.OnlineState) {
    const addRoutes = lodash.map(views, (value) => {
      return {
        path: value.path,
        name: value.name,
        component: value.component
      }
    });
    router.addRoutes(addRoutes);
    console.table(addRoutes, ['path', 'name'])
  }
});

export default router
