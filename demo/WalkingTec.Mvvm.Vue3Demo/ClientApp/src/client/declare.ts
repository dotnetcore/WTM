
import { ValidationRule } from 'ant-design-vue/lib/form/Form';
import { NamePath } from 'ant-design-vue/lib/form/interface';
/**
 * 类型定义
 */
export declare type ProFieldRequestData = (props?: any) => Promise<{
    label: any;
    value: any;
    [key: string]: any;
}[]>;
export declare type WTM_EntitiesField = {
    /** 表单 Name  */
    name: NamePath;
    /** 表单 label  */
    label?: string;
    /** 描述 */
    placeholder?: string;
    /** 默认值 */
    initialValue?: any;
    /** 校验规则 */
    rules?: ValidationRule[];
    /** 数据源 */
    request?: ProFieldRequestData | undefined;
    /** 联动字段 */
    linkage?: Array<string>;
    /** 数据类型 */
    valueType?: WTM_ValueType;
    /**
     * 传递给 Field 的 props
     */
    fieldProps?: any;
}
// https://procomponents.ant.design/components/field-set
export enum WTM_ValueType {
    /** 密码框 */
    password = "password",
    /** 文本域 */
    textarea = "textarea",
    /** 时间 */
    date = "date",
    dateWeek = "dateWeek",
    dateMonth = "dateMonth",
    dateRange = "dateRange",
    /** 文本 */
    text = "text",
    /** 选择框 */
    select = "select",
    /** 滑动输入 */
    slider = "slider",
    /** 多选框 */
    checkbox = "checkbox",
    /** 评分 */
    rate = "rate",
    /** 单选 */
    radio = "radio",
    /** 开关 */
    switch = "switch",
    /** 图片上传 */
    image = "image",
    /** 穿梭框 */
    transfer = "transfer",
    /** 文件 */
    upload = "upload",
    /** 可编辑表格 */
    grid = "grid",
    /** 富文本 */
    editor = "editor",
    icons = "icons",
    // radioButton = "radioButton",
    // progress = "progress",
    // money = "money",
    // option = "option",
    // dateQuarter = "dateQuarter",
    // dateYear = "dateYear",
    // dateTimeRange = "dateTimeRange",
    // dateTime = "dateTime",
    // time = "time",
    // timeRange = "timeRange",
    // index = "index",
    // indexBorder = "indexBorder",
    // percent = "percent",
    // digit = "digit",
    // second = "second",
    // avatar = "avatar",
    // code = "code",
    // fromNow = "fromNow",
    // jsonCode = "jsonCode"
}