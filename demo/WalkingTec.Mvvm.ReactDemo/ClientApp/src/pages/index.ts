import React from "react";
export default {
    actionlog: {
        name: 'MenuKey.Log',
        path: '/actionlog',
        controller: 'WalkingTec.Mvvm.Admin.Api,ActionLog',
        component: React.lazy(() => import('./actionlog'))
    },
    frameworkgroup: {
        name: 'MenuKey.GroupManagement',
        path: '/frameworkgroup',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkGroup',
        component: React.lazy(() => import('./frameworkgroup'))
    },
    frameworkrole: {
        name: 'MenuKey.RoleManagement',
        path: '/frameworkrole',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkRole',
        component: React.lazy(() => import('./frameworkrole'))
    },
    frameworkuserbase: {
        name: 'MenuKey.UserManagement',
        path: '/frameworkuser',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkUser',
        component: React.lazy(() => import('./frameworkuser'))
    },
    frameworkmenu: {
        name: 'MenuKey.MenuMangement',
        path: '/frameworkmenu',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkMenu',
        component: React.lazy(() => import('./frameworkmenu'))
    },
    dataprivilege: {
        name: 'MenuKey.DataPrivilege',
        path: '/dataprivilege',
        controller: 'WalkingTec.Mvvm.Admin.Api,DataPrivilege',
        component: React.lazy(() => import('./dataprivilege'))
    }
    /**WTM**/
}
