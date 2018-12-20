export default {
    editer: () => import('./editer').then(x => x.default),
    /**WTM**/
    test: () => import('./test').then(x => x.default),
    test2: () => import('./test2').then(x => x.default)
    /**WTM**/
}