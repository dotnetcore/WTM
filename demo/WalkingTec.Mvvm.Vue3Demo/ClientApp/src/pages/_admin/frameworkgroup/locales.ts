/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-frameworkgroup",
    ID = "frameworkgroup.ID",
    GroupCode = "frameworkgroup.GroupCode",
    GroupName = "frameworkgroup.GroupName",
    ParentId = "frameworkgroup.ParentId",
    Manager = "frameworkgroup.Manager",
    GroupRemark = "frameworkgroup.GroupRemark",
}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'Group Management',
        [EnumLocaleLabel.GroupCode]: 'GroupCode',
        [EnumLocaleLabel.ParentId]: 'ParentId',
        [EnumLocaleLabel.Manager]: 'Manager',
        [EnumLocaleLabel.GroupName]: 'GroupName',
        [EnumLocaleLabel.GroupRemark]: 'GroupRemark',
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '用户组管理',
        [EnumLocaleLabel.GroupCode]: '用户组编码',
        [EnumLocaleLabel.Manager]: '部门主管',
        [EnumLocaleLabel.ParentId]: '父级',
        [EnumLocaleLabel.GroupName]: '用户组名称',
        [EnumLocaleLabel.GroupRemark]: '备注',

    }
}