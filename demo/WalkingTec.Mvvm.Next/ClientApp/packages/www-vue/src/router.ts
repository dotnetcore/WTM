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
  {
    path: '/',
    name: 'home',
    component: views.user.component,
  },
  ...lodash.map(views, (value) => {
    return {
      path: value.path,
      name: value.name,
      component: value.component
    }
  }),
  {
    path: '*',
    // redirect: "/"
    component: views.user.component
  },
]
console.table(routes, ['path', 'name'])
const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})
router.beforeEach((to, from, next) => {
  console.log("TCL: from " + RootStore.UserStore.Loading, from);
  console.log("TCL: to", to);
  // if (RootStore.UserStore.Loading) {
  //   next(false)
  // } else {
  next()
  // }
})
// router.onReady(() => {
//   console.warn('onReady');
//   // 监控用户登录状态通知
//   // const LoginSubscription = RootStore.UserStore.UserObservable.subscribe((entitie) => {
//   //   console.warn("TCL: entitie", entitie)
//   //   // LoginSubscription.unsubscribe();
//   //   const isLogin = lodash.eq(router.currentRoute.path, '/login');
//   //   if ((!entitie.Loading && !entitie.OnlineState)) {
//   //     !isLogin && router.replace({ path: "/login" })
//   //   } else {
//   //     isLogin && router.replace({ path: "/" });
//   //   }
//   // });
// })

export default router
