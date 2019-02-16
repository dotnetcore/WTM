export default {
    user: {
        name: '用户列表',
        path: '/frameworkuser',
        component: () => import('./frameworkuser').then(x => x.default) 
    }
    
, frameworkrole: {
        name: '角色管理',
        path: '/frameworkrole',
        component: () => import('./frameworkrole').then(x => x.default) 
    }

, frameworkgroup: {
        name: '用户组管理',
        path: '/frameworkgroup',
        component: () => import('./frameworkgroup').then(x => x.default) 
    }
/**WTM**/
 
 
}