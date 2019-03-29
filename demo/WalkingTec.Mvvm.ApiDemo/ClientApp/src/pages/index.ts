export default {
    tmp: {
        name: '测试',
        path: '/tmp',
        component: () => import('./tmp').then(x => x.default)
    }

, actionlog: {
        name: '日志管理',
        path: '/actionlog',
        component: () => import('./actionlog').then(x => x.default) 
    }
/**WTM**/
 
 
}