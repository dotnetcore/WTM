import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import views, { Basics } from './views'
import lodash from 'lodash'
Vue.use(VueRouter)
const routes: RouteConfig[] = [
  {
    path: '/',
    name: 'home',
    component: Basics.home
  },
  ...lodash.map(views, (value, key) => {
    return {
      path: `/${key}`,
      name: key,
      component: value
    }
  }),
  {
    path:'*',
    redirect: "/"
    // component: Basics.home
  },
]
console.log("TCL: routes", routes)

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})
// router.beforeEach((to, from, next) => {
//   console.log("TCL: from", from)
//   console.log("TCL: to", to)
//   next()
// })
export default router
