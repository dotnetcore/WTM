import Vue from 'vue';
import Router from 'vue-router';
Vue.use(Router);
const router = new Router({
    // mode: 'history',
    routes: [
        {
            name: 'index',
            path: '*',
            meta: { index: 1 },
            component: () => import('../pages/index/index')
        }
    ]
});
export default router;
