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
    , frameworktenant: {
        name: 'MenuKey.FrameworkTenant',
        path: '/frameworktenant',
        controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkTenant',
        component: React.lazy(() => import('./frameworktenant'))
    }
    , frameworkworkflow: {
        name: 'MenuKey.Workflow',
        path: '/frameworkworkflow',
        controller: '',
        component: React.lazy(() => import('./frameworkworkflow'))
    }
, school: {
        name: '学校管理',
        path: '/school',
        controller: 'WalkingTec.Mvvm.ReactDemo.Controllers,School',
        component: React.lazy(() => import('./school'))
    }

, major: {
        name: '专业管理',
        path: '/major',
        controller: 'WalkingTec.Mvvm.ReactDemo.Controllers,Major',
        component: React.lazy(() => import('./major'))
    }

, student: {
        name: '学生管理',
        path: '/student',
        controller: 'WalkingTec.Mvvm.ReactDemo.Controllers,Student',
        component: React.lazy(() => import('./student'))
    }

, city: {
        name: '城市管理',
        path: '/city',
        controller: 'WalkingTec.Mvvm.ReactDemo.Controllers,City',
        component: React.lazy(() => import('./city'))
    }

/**WTM**/
 
 
 
 
 
}
