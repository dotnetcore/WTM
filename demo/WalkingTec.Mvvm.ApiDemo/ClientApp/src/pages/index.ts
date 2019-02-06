export default {
    /**WTM**/
    user: {
        name: 'user',
        path: '/user',
        component: () => import('./user').then(x => x.default) 
    }
    /**WTM**/
}