/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-dataprivilege",
    ID = "dataprivilege.ID",
    Name = "dataprivilege.Name",
    TableName = "dataprivilege.TableName",
    RelateIDs = "dataprivilege.RelateIDs",
}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'DataPrivilege',
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '数据权限',

    }
}