/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-frameworkrole",
    ID = "frameworkrole.ID",
    RoleCode = "frameworkrole.RoleCode",
    RoleName = "frameworkrole.RoleName",
    RoleRemark = "frameworkrole.RoleRemark",
    Page = "frameworkrole.Page",
    Action = "frameworkrole.Action",

}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'Role Manage',
        [EnumLocaleLabel.RoleCode]: 'RoleCode',
        [EnumLocaleLabel.RoleName]: 'RoleName',
        [EnumLocaleLabel.RoleRemark]: 'RoleRemark',
        [EnumLocaleLabel.Page]: 'Page',
        [EnumLocaleLabel.Action]: 'Action',
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '角色管理',
        [EnumLocaleLabel.RoleCode]: '角色编码',
        [EnumLocaleLabel.RoleName]: '角色名称',
        [EnumLocaleLabel.RoleRemark]: '备注',
        [EnumLocaleLabel.Page]: '页面',
        [EnumLocaleLabel.Action]: '动作',

    }
}