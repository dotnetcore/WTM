/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-school",
    ID = "school.ID",
    SchoolCode = "school.SchoolCode",
    SchoolName = "school.SchoolName",
    SchoolType = "school.SchoolType",
    Remark = "school.Remark",
    Majors = "school.Majors",
    Photos = "school.Photos",
    Files = "school.Files", 
}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'school',
        [EnumLocaleLabel.SchoolCode]: 'SchoolCode',
        [EnumLocaleLabel.Majors]:'Majors',
        [EnumLocaleLabel.Files]:'Files',
        [EnumLocaleLabel.Photos]:'Photos'
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '学校',
        [EnumLocaleLabel.SchoolCode]: 'SchoolCode',
        [EnumLocaleLabel.Majors]:'Majors',
        [EnumLocaleLabel.Files]:'附件',
        [EnumLocaleLabel.Photos]:'图片'
    }
}