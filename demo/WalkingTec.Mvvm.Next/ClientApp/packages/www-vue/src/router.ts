import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import views, { Basics } from './views'
import layout from './components/layout/index.vue'
import lodash from 'lodash'
Vue.use(VueRouter)
const routes: RouteConfig[] = [
  {
    path: '/login',
    name: 'login',
    component: Basics.login
  },
  {
    path: '/',
    component: layout,
    children: [
      {
        path: '/',
        name: 'home',
        component: Basics.home,
      },
      ...lodash.map(views, (value, key) => {
        return {
          path: `/${key}`,
          name: key,
          component: value
        }
      }),
      {
        path: '*',
        // redirect: "/"
        component: views.test
      },
    ],
  },


]
console.table(routes[1].children, ['path', 'name'])

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
