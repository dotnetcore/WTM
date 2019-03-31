export default {
    tmp: {
        name: '测试',
        path: '/tmp',
        component: () => import('./tmp').then(x => x.default)
    }

, actionlog: {
        name: '日志管理',
        path: '/actionlog',
        controller:'ActionLog',
        component: () => import('./actionlog').then(x => x.default) 
    }

, school: {
        name: '学校管理',
        path: '/school',
        controller: 'School',
        component: () => import('./school').then(x => x.default) 
    }

, frameworkgroup: {
        name: '用户组管理',
        path: '/frameworkgroup',
        controller: 'FrameworkGroup',
        component: () => import('./frameworkgroup').then(x => x.default) 
    }

, frameworkrole: {
        name: '角色管理',
        path: '/frameworkrole',
        component: () => import('./frameworkrole').then(x => x.default) 
    }

, frameworkuserbase: {
        name: '用户管理',
        path: '/frameworkuserbase',
        controller: 'frameworkuser',
       component: () => import('./frameworkuserbase').then(x => x.default) 
    }

, frameworkmenu: {
        name: '菜单管理',
        path: '/frameworkmenu',
        controller: 'FrameworkMenu',
       component: () => import('./frameworkmenu').then(x => x.default) 
    }
/**WTM**/
 
 
 
 
 
 
 
}