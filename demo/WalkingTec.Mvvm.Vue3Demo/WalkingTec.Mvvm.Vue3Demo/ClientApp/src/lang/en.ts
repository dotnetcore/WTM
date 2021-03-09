export default {
  route: {
    dashboard: "Dashboard",
    actionlog: "Log",
    frameworkgroup: "Group management",
    frameworkrole: "Role management",
    frameworkuser: "User management",
    frameworkmenu: "Menu management",
    dataprivilege: "Data privileges",
    systemManagement: "System"
  },
  navbar: {
    logOut: "Log Out",
    dashboard: "Dashboard",
    github: "Github",
    doc: "Doc",
    api: "Api Doc",
    theme: "Theme",
    size: "Global Size",
    profile: "Profile",
    generation: "Code Gen",
    full: "full screen",
    language: "language",
    set: "Set up",
    password: 'Password',
    oldPassword: 'Old Password',
    newPassword: 'New Password',
    confirmNewPassword: 'Confirm New Password',
    pleaseEnterOldPassword: "Please Enter Old Password",
    pleaseEnterNewPassword: "Please Enter New Password",
    pleaseEnterConfirmNewPassword: "Please Enter Confirm New Password"
  },
  login: {
    title: "Login Form",
    logIn: "Login",
    username: "Username",
    password: "Password",
    pleaseEnterUsername: "Please Enter User Name",
    pleaseEnterPassword: "Please Enter Password",
    any: "any",
    thirdparty: "Or connect with",
    thirdpartyTips:
      "Can not be simulated on local, so please combine you own business simulation! ! !"
  },
  documentation: {
    documentation: "Documentation",
    github: "Github Repository"
  },
  permission: {
    createRole: "New Role",
    editPermission: "Edit",
    roles: "Your roles",
    switchRoles: "Switch roles",
    tips:
      "In some cases, using v-permission will have no effect. For example: Element-UI  el-tab or el-table-column and other scenes that dynamically render dom. You can only do this with v-if.",
    delete: "Delete",
    confirm: "Confirm",
    cancel: "Cancel"
  },
  guide: {
    description:
      "The guide page is useful for some people who entered the project for the first time. You can briefly introduce the features of the project. Demo is based on ",
    button: "Show Guide"
  },
  components: {
    documentation: "Documentation",
    tinymceTips:
      "Rich text is a core feature of the management backend, but at the same time it is a place with lots of pits. In the process of selecting rich texts, I also took a lot of detours. The common rich texts on the market have been basically used, and I finally chose Tinymce. See the more detailed rich text comparison and introduction.",
    stickyTips:
      "when the page is scrolled to the preset position will be sticky on the top.",
    backToTopTips1:
      "When the page is scrolled to the specified position, the Back to Top button appears in the lower right corner",
    backToTopTips2:
      "You can customize the style of the button, show / hide, height of appearance, height of the return. If you need a text prompt, you can use element-ui el-tooltip elements externally",
    imageUploadTips:
      "Since I was using only the vue@1 version, and it is not compatible with mockjs at the moment, I modified it myself, and if you are going to use it, it is better to use official version."
  },
  table: {
    dynamicTips1: "Fixed header, sorted by header order",
    dynamicTips2: "Not fixed header, sorted by click order",
    dragTips1: "The default order",
    dragTips2: "The after dragging order",
    title: "Title",
    importance: "Importance",
    type: "Type",
    remark: "Remark",
    search: "Search",
    add: "Add",
    export: "Export",
    reviewer: "Reviewer",
    id: "ID",
    date: "Date",
    author: "Author",
    readings: "Readings",
    status: "Status",
    actions: "Actions",
    edit: "Edit",
    publish: "Publish",
    draft: "Draft",
    delete: "Delete",
    cancel: "Cancel",
    confirm: "Confirm",
    detail: "Details",
    custom: "Custom column",
    download: "Download",
    startdate: "Start",
    enddate: "End"
  },
  example: {
    warning:
      "Creating and editing pages cannot be cached by keep-alive because keep-alive's 'include' currently does not support caching based on routes, so now it's cached based on component name. If you want to achieve a similar caching effect, you can use a browser caching scheme such as localStorage. Or don't use keep-alive's 'include' to cache all pages directly. See details"
  },
  errorLog: {
    tips: "Please click the bug icon in the upper right corner",
    description:
      "Now the management system are basically the form of the spa, it enhances the user experience, but it also increases the possibility of page problems, a small negligence may lead to the entire page deadlock. Fortunately Vue provides a way to catch handling exceptions, where you can handle errors or report exceptions.",
    documentation: "Document introduction"
  },
  errorMsg: {
    msg: "Error",
    error: "Interface error",
    template: "Template error",
    import: "Import failed"
  },
  excel: {
    export: "Export",
    selectedExport: "Export Selected Items",
    placeholder: "Please enter the file name (default excel-list)"
  },
  zip: {
    export: "Export",
    placeholder: "Please enter the file name (default file)"
  },
  pdf: {
    tips:
      "Here we use window.print() to implement the feature of downloading PDF."
  },
  theme: {
    change: "Change Theme",
    documentation: "Theme documentation",
    tips:
      "Tips: It is different from the theme-pick on the navbar is two different skinning methods, each with different application scenarios. Refer to the documentation for details."
  },
  tagsView: {
    refresh: "Refresh",
    close: "Close",
    closeOthers: "Close Others",
    closeAll: "Close All"
  },
  settings: {
    title: "Page style setting",
    theme: "Theme Color",
    showTagsView: "Open Tags-View",
    showSidebarLogo: "Sidebar Logo",
    fixedHeader: "Fixed Header",
    sidebarTextTheme: "Sidebar Text Theme"
  },
  buttom: {
    add: "Add",
    edit: "Edit",
    delete: "Delete",
    import: "Import",
    export: "Export",
    exportAll: "Export all",
    exportSelect: "Export select",
    reset: "Reset",
    search: "Search",
    determine: "Determine",
    closed: "Close",
    submit: "Submit",
    cancel: "Cancel"
  },
  form: {
    pleaseEnter: "Please enter ",
    upload: "Upload",
      clickUpload: "Click to upload",
      confirmDeletion: "Are you sure to delete?",
      prompt: "Info",
      ImportSucceeded: "Import succeeded",
      successfullyDeleted: "Delete succeeded",
      failedToDelete: "Delete failed",
      ExportSucceeded: "Export succeeded",
      SuccessfullyModified: "Edit succeeded",
    all: "All",
    selected: "Selected",
    yes: "Yes",
      no: "No",
      notnull: " can not be null"
  }
};
