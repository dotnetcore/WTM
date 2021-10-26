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
    LogType_0 = "actionlog.LogType.0",
    LogType_1 = "actionlog.LogType.1",
    LogType_2 = "actionlog.LogType.2",
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
        [EnumLocaleLabel.LogType_0]: 'Normal',
        [EnumLocaleLabel.LogType_1]: 'Exception',
        [EnumLocaleLabel.LogType_2]: 'Debug',
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
        [EnumLocaleLabel.LogType_0]: '普通',
        [EnumLocaleLabel.LogType_1]: '异常',
        [EnumLocaleLabel.LogType_2]: '调试',
    }
}