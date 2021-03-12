import { ValidationRule } from 'ant-design-vue/lib/form/Form';
import { NamePath } from 'ant-design-vue/lib/form/interface';
export * from "./controllers";
export * from "./enum";
export * from "./helpers";
export * from "./locales";
declare type ProFieldRequestData<U = any> = (params: U, props: any) => Promise<{
    label?: any;
    value?: any;
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
    valueType?: "password" | "money" | "textarea" | "option" | "date" | "dateWeek" | "dateMonth" | "dateQuarter" | "dateYear" | "dateRange" | "dateTimeRange" | "dateTime" | "time" | "timeRange" | "text" | "select" | "checkbox" | "rate" | "radio" | "radioButton" | "index" | "indexBorder" | "progress" | "percent" | "digit" | "second" | "avatar" | "code" | "switch" | "fromNow" | "image" | "jsonCode"
}
