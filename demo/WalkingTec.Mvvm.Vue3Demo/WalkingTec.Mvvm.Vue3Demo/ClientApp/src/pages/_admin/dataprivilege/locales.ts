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
    DpType = "dataprivilege.DpType",
    DpType_0 = "dataprivilege.DpType.0",
    DpType_1 = "dataprivilege.DpType.1",
    SelectedItemsID = "dataprivilege.SelectedItemsID",
    IsAll = "dataprivilege.IsAll",
    IsAll_true = "dataprivilege.IsAll.true",
    IsAll_false = "dataprivilege.IsAll.false",
    UserCode = "dataprivilege.UserCode",
    GroupCode = "dataprivilege.GroupCode",
}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'Data Privilege',
        [EnumLocaleLabel.Name]: 'Name',
        [EnumLocaleLabel.TableName]: 'TableName',
        [EnumLocaleLabel.RelateIDs]: 'Privileges',
        [EnumLocaleLabel.DpType]: 'DpType',
        [EnumLocaleLabel.DpType_0]: 'User group permissions',
        [EnumLocaleLabel.DpType_1]: 'User permissions',
        [EnumLocaleLabel.SelectedItemsID]: 'Allow access to',
        [EnumLocaleLabel.IsAll]: 'All permissions',
        [EnumLocaleLabel.IsAll_true]: 'true',
        [EnumLocaleLabel.IsAll_false]: 'false',
        [EnumLocaleLabel.UserCode]: 'UserItCode',
        [EnumLocaleLabel.GroupCode]: 'User Group',
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '数据权限',
        [EnumLocaleLabel.Name]: '授权对象',
        [EnumLocaleLabel.TableName]: '权限名称',
        [EnumLocaleLabel.RelateIDs]: '权限',
        [EnumLocaleLabel.DpType]: '权限类型',
        [EnumLocaleLabel.DpType_0]: '用户组权限',
        [EnumLocaleLabel.DpType_1]: '用户权限',
        [EnumLocaleLabel.SelectedItemsID]: '允许访问',
        [EnumLocaleLabel.IsAll]: '全部权限',
        [EnumLocaleLabel.IsAll_true]: '是',
        [EnumLocaleLabel.IsAll_false]: '否',
        [EnumLocaleLabel.UserCode]: '用户Id',
        [EnumLocaleLabel.GroupCode]: '用户组',

    }
}