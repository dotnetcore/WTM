import { ValidationRule } from 'ant-design-vue/lib/form/Form';
import { NamePath } from 'ant-design-vue/lib/form/interface';
export * from "./controllers";
export * from "./enum";
export * from "./helpers";
export * from "./locales";
declare type ProFieldRequestData<U = any> = (params: U, props: any) => Promise<{
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
    rules?: ValidationRule[];
    request?: ProFieldRequestData<any> | undefined;
    valueType?: WTM_ValueType
}
export enum WTM_ValueType {
    password = "password",
    // money = "money",
    textarea = "textarea",
    // option = "option",
    date = "date",
    dateWeek = "dateWeek",
    dateMonth = "dateMonth",
    dateQuarter = "dateQuarter",
    dateYear = "dateYear",
    dateRange = "dateRange",
    dateTimeRange = "dateTimeRange",
    dateTime = "dateTime",
    time = "time",
    timeRange = "timeRange",
    text = "text",
    select = "select",
    checkbox = "checkbox",
    rate = "rate",
    radio = "radio",
    radioButton = "radioButton",
    // index = "index",
    // indexBorder = "indexBorder",
    progress = "progress",
    // percent = "percent",
    // digit = "digit",
    // second = "second",
    // avatar = "avatar",
    // code = "code",
    switch = "switch",
    // fromNow = "fromNow",
    image = "image",
    // jsonCode = "jsonCode"
}