/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.admin-actionlog",
    ID = "actionlog.ID",
    ModuleName = "actionlog.ModuleName",
    ActionName = "actionlog.ActionName",
    ITCode = "actionlog.ITCode",
    ActionUrl = "actionlog.ActionUrl",
    ActionTime = "actionlog.ActionTime",
    Duration = "actionlog.Duration",
    Remark = "actionlog.Remark",
    IP = "actionlog.IP",
    LogType = "actionlog.LogType",
}
export default {
    en: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: 'Log',
        [EnumLocaleLabel.ModuleName]: 'ModuleName',
        [EnumLocaleLabel.ActionName]: 'ActionName',
        [EnumLocaleLabel.ITCode]: 'ITCode',
        [EnumLocaleLabel.ActionUrl]: 'ActionUrl',
        [EnumLocaleLabel.ActionTime]: 'ActionTime',
        [EnumLocaleLabel.Duration]: 'Duration',
        [EnumLocaleLabel.Remark]: 'Remark',
        [EnumLocaleLabel.IP]: 'IP',
        [EnumLocaleLabel.LogType]: 'LogType',
       
    },
    zh: {
        [EnumLocaleLabel.ID]: 'ID',
        [EnumLocaleLabel.PageName]: '日志',
        [EnumLocaleLabel.ModuleName]: '模块',
        [EnumLocaleLabel.ActionName]: '动作',
        [EnumLocaleLabel.ITCode]: '账户',
        [EnumLocaleLabel.ActionUrl]: 'Url',
        [EnumLocaleLabel.ActionTime]: '操作时间',
        [EnumLocaleLabel.Duration]: '时长',
        [EnumLocaleLabel.Remark]: '备注',
        [EnumLocaleLabel.IP]: 'IP',
        [EnumLocaleLabel.LogType]: '类型',
    }
}