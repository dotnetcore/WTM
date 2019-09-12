import React from "react";
export default {
    actionlog: {
        name: '日志管理',
        path: '/_actionlog',
        controller: '_ActionLog',
        component: React.lazy(() => import('./actionlog'))
    },
    frameworkgroup: {
        name: '用户组管理',
        path: '/_frameworkgroup',
        controller: '_FrameworkGroup',
        component: React.lazy(() => import('./frameworkgroup'))
    },
    frameworkrole: {
        name: '角色管理',
        path: '/_frameworkrole',
        controller: '_FrameworkRole',
        component: React.lazy(() => import('./frameworkrole'))
    },
    frameworkuserbase: {
        name: '用户管理',
        path: '/_frameworkuser',
        controller: '_FrameworkUser',
        component: React.lazy(() => import('./frameworkuser'))
    },
    frameworkmenu: {
        name: '菜单管理',
        path: '/_frameworkmenu',
        controller: 'FrameworkMenu',
        component: React.lazy(() => import('./frameworkmenu'))
    },
    dataprivilege: {
        name: '数据权限',
        path: '/_dataprivilege',
        controller: '_DataPrivilege',
        component: React.lazy(() => import('./dataprivilege'))
    }
    /**WTM**/
}
