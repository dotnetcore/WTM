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

, frameworkuserbase: {
        name: '用户管理',
        path: '/frameworkuserbase',
        component: () => import('./frameworkuserbase').then(x => x.default) 
    }

, frameworkgroup: {
        name: '用户组管理',
        path: '/frameworkgroup',
        component: () => import('./frameworkgroup').then(x => x.default) 
    }

, frameworkrole: {
        name: '角色管理',
        path: '/frameworkrole',
        component: () => import('./frameworkrole').then(x => x.default) 
    }
/**WTM**/
 
 
 
 
 
}