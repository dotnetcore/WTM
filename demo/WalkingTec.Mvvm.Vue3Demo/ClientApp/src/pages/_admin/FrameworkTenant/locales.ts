/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-FrameworkTenant",
    ID = "FrameworkTenant.ID",
    TCode = "FrameworkTenant.TCode",
    TName = "FrameworkTenant.TName",
    TDb = "FrameworkTenant.TDb",
    TDbType = 'FrameworkTenant.TDbType',
    DbContext = 'FrameworkTenant.DbContext',
    TDomain = 'FrameworkTenant.TDomain',
    EnableSub = 'FrameworkTenant.EnableSub',
    Enabled = 'FrameworkTenant.Enabled',
    AdminRoleCode = 'FrameworkTenant.AdminRoleCode'
}
export default {
    en: { 
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'Group Management',
        [EnumLocaleLabel.TCode]: 'TCode',
        [EnumLocaleLabel.TName]: 'TName',
        [EnumLocaleLabel.TDb]: 'TDb',
        [EnumLocaleLabel.DbContext]: 'DbContext',
        [EnumLocaleLabel.TDbType]: 'TDbType',
        [EnumLocaleLabel.Enabled]: 'Enabled',
        [EnumLocaleLabel.TDomain]: 'TDomain',
        [EnumLocaleLabel.EnableSub]: 'EnableSub',
        [EnumLocaleLabel.AdminRoleCode]: 'AdminRoleCode',
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '租户管理',
        [EnumLocaleLabel.TDb]: '租户数据库',
        [EnumLocaleLabel.TName]: '租户名称',
        [EnumLocaleLabel.TCode]: '租户编号',
        [EnumLocaleLabel.TDbType]: '数据库类型',
        [EnumLocaleLabel.DbContext]: '数据库架构',
        [EnumLocaleLabel.TDomain]: '租户域名',
        [EnumLocaleLabel.AdminRoleCode]: '角色',
        [EnumLocaleLabel.EnableSub]: '允许子租户',
        [EnumLocaleLabel.Enabled]: '启用',
    }
}