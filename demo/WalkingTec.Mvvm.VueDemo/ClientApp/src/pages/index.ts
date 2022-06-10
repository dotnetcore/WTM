/**
 * 页面集合
 */
export default {
  actionlog: {
    name: "actionlog",
    path: "/actionlog",
    controller: "WalkingTec.Mvvm.Admin.Api,ActionLog",
    icon: "el-icon-receiving"
  },
   frameworkuserbase: {
    name: "frameworkuser",
    path: "/frameworkuser",
    controller: "WalkingTec.Mvvm.Admin.Api,FrameworkUser",
    icon: "el-icon-user"
  },
  frameworkrole: {
    name: "frameworkrole",
    path: "/frameworkrole",
    controller: "WalkingTec.Mvvm.Admin.Api,FrameworkRole",
    icon: "el-icon-s-custom"
  },
 frameworkgroup: {
    name: "frameworkgroup",
    path: "/frameworkgroup",
    controller: "WalkingTec.Mvvm.Admin.Api,FrameworkGroup",
    icon: "el-icon-office-building"
  },
  frameworkmenu: {
    name: "frameworkmenu",
    path: "/frameworkmenu",
    controller: "WalkingTec.Mvvm.Admin.Api,FrameworkMenu",
    icon: "el-icon-menu"
  },
  dataprivilege: {
    name: "dataprivilege",
    path: "/dataprivilege",
    controller: "WalkingTec.Mvvm.Admin.Api,DataPrivilege",
    icon: "el-icon-odometer"
  }

, student: {
    name: '学生管理',
    path: '/student',
    controller: 'WalkingTec.Mvvm.VueDemo.Controllers,Student'
    }

, school: {
    name: '学校管理',
    path: '/school',
    controller: 'WalkingTec.Mvvm.VueDemo.Controllers,School'
    }

, major: {
    name: '专业管理',
    path: '/major',
    controller: 'WalkingTec.Mvvm.VueDemo.Controllers,Major'
    }

, frameworktenant: {
    name: '租户管理',
    path: '/frameworktenant',
    controller: 'WalkingTec.Mvvm.Admin.Api,FrameworkTenant'
    }
/**WTM**/
 
 
 
 

};
