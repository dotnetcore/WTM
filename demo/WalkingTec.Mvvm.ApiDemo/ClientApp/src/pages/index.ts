export default {
    /**WTM**/
    test: {
        name: 'test',
        path: '/test',
        component: () => import('./test').then(x => x.default) 
    }
    /**WTM**/
}