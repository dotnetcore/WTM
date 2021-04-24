import { $i18n, WTM_EntitiesField, WTM_ValueType, FieldRequest } from '@/client';

/**
 * 字段描述 枚举
 */
export enum EnumLocaleLabel {
    /** 当前页面名称 */
    PageName = "PageName.test",

    Sex_Male = "frameworkuser.Sex_Male",
    Sex_Female = "frameworkuser.Sex_Female",

}
export default {
    en: {
        [EnumLocaleLabel.PageName]: 'Test',
        [EnumLocaleLabel.Sex_Male]: 'Male',
        [EnumLocaleLabel.Sex_Female]: 'Female',
        [WTM_ValueType.checkbox]:'checkbox',
        [WTM_ValueType.date]:'date',
        [WTM_ValueType.dateMonth]:'dateMonth',
        [WTM_ValueType.dateRange]:'dateRange',
        [WTM_ValueType.dateWeek]:'dateWeek',
        [WTM_ValueType.image]:'image',
        [WTM_ValueType.password]:'password',
        [WTM_ValueType.radio]:'radio',
        [WTM_ValueType.rate]:'rate',
        [WTM_ValueType.select]:'select',
        [WTM_ValueType.slider]:'slider',
        [WTM_ValueType.switch]:'switch',
        [WTM_ValueType.text]:'text',
        [WTM_ValueType.textarea]:'textarea',
        [WTM_ValueType.transfer]:'transfer',
    },
    zh: {
        [EnumLocaleLabel.PageName]: '测试',
        [EnumLocaleLabel.Sex_Male]: '男',
        [EnumLocaleLabel.Sex_Female]: '女',
        [WTM_ValueType.checkbox]:'多选框',
        [WTM_ValueType.date]:'时间',
        [WTM_ValueType.dateMonth]:'月份',
        [WTM_ValueType.dateRange]:'范围',
        [WTM_ValueType.dateWeek]:'周期',
        [WTM_ValueType.image]:'图片',
        [WTM_ValueType.password]:'密码',
        [WTM_ValueType.radio]:'单选',
        [WTM_ValueType.rate]:'评分',
        [WTM_ValueType.select]:'选择框',
        [WTM_ValueType.slider]:'滑动输入',
        [WTM_ValueType.switch]:'开关',
        [WTM_ValueType.text]:'文本',
        [WTM_ValueType.textarea]:'文本域',
        [WTM_ValueType.transfer]:'穿梭框',
    }
}