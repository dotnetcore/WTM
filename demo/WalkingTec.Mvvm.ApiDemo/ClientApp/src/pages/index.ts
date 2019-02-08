export default {
    /**WTM**/
    user: {
        name: '用户列表',
        path: '/user',
        component: () => import('./user').then(x => x.default) 
    }
    /**WTM**/
}