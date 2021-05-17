
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
    valueType?: WTM_ValueType
}
// https://procomponents.ant.design/components/field-set
export enum WTM_ValueType {
    password = "password",
    textarea = "textarea",
    date = "date",
    dateWeek = "dateWeek",
    dateMonth = "dateMonth",
    dateRange = "dateRange",
    text = "text",
    select = "select",
    slider = "slider",
    checkbox = "checkbox",
    rate = "rate",
    radio = "radio",
    // radioButton = "radioButton",
    // progress = "progress",
    switch = "switch",
    image = "image",
    transfer = "transfer",
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