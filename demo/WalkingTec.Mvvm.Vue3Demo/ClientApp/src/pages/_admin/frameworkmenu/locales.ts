/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-frameworkmenu",
    ID = "frameworkmenu.ID",
    Name = "frameworkmenu.PageName",
    SelectedModule = "frameworkmenu.SelectedModule",
    SelectedActionIDs = "frameworkmenu.SelectedActionIDs",
    FolderOnly = "frameworkmenu.FolderOnly",
    ShowOnMenu = "frameworkmenu.ShowOnMenu",
    IsPublic = "frameworkmenu.IsPublic",
    IsInside = "frameworkmenu.IsInside",
    IsInside_0 = "frameworkmenu.IsInside.0",
    IsInside_1 = "frameworkmenu.IsInside.1",
    Url = "frameworkmenu.Url",
    ParentId = "frameworkmenu.ParentId",
    DisplayOrder = "frameworkmenu.DisplayOrder",
    Icon = "frameworkmenu.Icon",

}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'Menu Mangement',
        [EnumLocaleLabel.Name]: 'PageName',
        [EnumLocaleLabel.SelectedModule]: 'Module',
        [EnumLocaleLabel.SelectedActionIDs]: 'Actions',
        [EnumLocaleLabel.FolderOnly]: 'Folder',
        [EnumLocaleLabel.ShowOnMenu]: 'ShowOnMenu',
        [EnumLocaleLabel.IsPublic]: 'IsPublic',
        [EnumLocaleLabel.IsInside]: 'UrlType',
        [EnumLocaleLabel.IsInside_0]: 'Inside Url',
        [EnumLocaleLabel.IsInside_1]: 'Ourside Url',
        [EnumLocaleLabel.Url]: 'Url',
        [EnumLocaleLabel.ParentId]: 'ParentFolder',
        [EnumLocaleLabel.DisplayOrder]: 'DisplayOrder',
        [EnumLocaleLabel.Icon]: 'Icon',

    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '菜单管理',
        [EnumLocaleLabel.Name]: '页面名称',
        [EnumLocaleLabel.SelectedModule]: '模块名称',
        [EnumLocaleLabel.SelectedActionIDs]: '动作名称',
        [EnumLocaleLabel.FolderOnly]: '目录',
        [EnumLocaleLabel.ShowOnMenu]: '菜单显示',
        [EnumLocaleLabel.IsPublic]: '公开',
        [EnumLocaleLabel.IsInside]: '地址类型',
        [EnumLocaleLabel.IsInside_0]: '内部地址',
        [EnumLocaleLabel.IsInside_1]: '外部地址',
        [EnumLocaleLabel.Url]: 'Url',
        [EnumLocaleLabel.ParentId]: '父目录',
        [EnumLocaleLabel.DisplayOrder]: '顺序',
        [EnumLocaleLabel.Icon]: '图标',
    }
}