/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-city",
    ID = "city.ID",
    Name = "city.Name",
    Level = "city.Level",
    ParentId = "city.ParentId",
}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'city',
        [EnumLocaleLabel.Name]: 'Name',
        [EnumLocaleLabel.Level]: 'Level',
        [EnumLocaleLabel.ParentId]: 'ParentId',
      
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'city',
        [EnumLocaleLabel.Name]: 'Name',
        [EnumLocaleLabel.Level]: 'Level',
        [EnumLocaleLabel.ParentId]: 'ParentId',
    }
}