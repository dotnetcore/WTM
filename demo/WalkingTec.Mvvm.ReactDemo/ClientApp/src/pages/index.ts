import React from "react";
export default {
    actionlog: {
        name: '日志管理',
        path: '/actionlog',
        controller: 'ActionLog',
        component: React.lazy(() => import('./actionlog'))
    },
    frameworkgroup: {
        name: '用户组管理',
        path: '/frameworkgroup',
        controller: 'FrameworkGroup',
        component: React.lazy(() => import('./frameworkgroup'))
    },
    frameworkrole: {
        name: '角色管理',
        path: '/frameworkrole',
        controller: 'FrameworkRole',
        component: React.lazy(() => import('./frameworkrole'))
    },
    frameworkuserbase: {
        name: '用户管理',
        path: '/frameworkuser',
        controller: 'FrameworkUser',
        component: React.lazy(() => import('./frameworkuser'))
    },
    frameworkmenu: {
        name: '菜单管理',
        path: '/frameworkmenu',
        controller: 'FrameworkMenu',
        component: React.lazy(() => import('./frameworkmenu'))
    },
    dataprivilege: {
        name: '数据权限',
        path: '/dataprivilege',
        controller: 'DataPrivilege',
        component: React.lazy(() => import('./dataprivilege'))
    }
    /**WTM**/
}
