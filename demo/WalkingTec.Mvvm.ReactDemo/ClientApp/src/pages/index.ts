import React from "react";
export default {
    actionlog: {
        name: '日志管理',
        path: '/actionlog',
        controller: 'WalkingTec.Mvvm.Admin.Api,ActionLog',
        component: React.lazy(() => import('./actionlog'))
    },
    frameworkgroup: {
        name: '用户组管理',
        path: '/frameworkgroup',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkGroup',
        component: React.lazy(() => import('./frameworkgroup'))
    },
    frameworkrole: {
        name: '角色管理',
        path: '/frameworkrole',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkRole',
        component: React.lazy(() => import('./frameworkrole'))
    },
    frameworkuserbase: {
        name: '用户管理',
        path: '/frameworkuser',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkUser',
        component: React.lazy(() => import('./frameworkuser'))
    },
    frameworkmenu: {
        name: '菜单管理',
        path: '/frameworkmenu',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkMenu',
        component: React.lazy(() => import('./frameworkmenu'))
    },
    dataprivilege: {
        name: '数据权限',
        path: '/dataprivilege',
        controller: 'WalkingTec.Mvvm.Admin.Api,DataPrivilege',
        component: React.lazy(() => import('./dataprivilege'))
    }
    /**WTM**/
}
