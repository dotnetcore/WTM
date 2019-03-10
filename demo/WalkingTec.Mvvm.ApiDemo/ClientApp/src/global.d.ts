// import { ValidationRule } from 'antd/lib/form/Form'
declare module '*.svg'
declare module '*.png'
declare module '*.jpg'
declare module '*.jpeg'
declare module '*.gif'
declare module '*.bmp'
declare module '*.tiff'
declare module '*.json'
declare namespace WTM {
    /**
     * 动作
     */
    interface IActions {
        /** 添加按钮 */
        insert: boolean;
        /** 添加修改 */
        update: boolean;
        /** 删除按钮 */
        delete: boolean;
        /** 导入按钮 */
        import: boolean;
        /** 导出按钮 */
        export: boolean;
        [key: string]: boolean
    }
    interface IUrl {
        url: string;
        method?: 'get' | 'post' | 'put' | 'delete'
        // [key: string]: any;
    }
    interface IUrls {
        search?: IUrl;
        details?: IUrl;
        insert?: IUrl;
        update?: IUrl;
        delete?: IUrl;
        import?: IUrl;
        export?: IUrl;
        exportIds?: IUrl;
        template?: IUrl;
        [key: string]: IUrl;
    }
    interface FormItem {
        [key: string]: {
            label: string;
            rules: ValidationRule[];
            formItem: React.ReactNode;
        }
    }
    interface PageProps {
        /** 页面默认参数 */
        defaultSearchParams: {
            [key: string]: any;
        };
        [key: string]: any;
    }
}
// import { ValidationRule } from 'antd/lib/form/Form' 拷贝
declare type ValidationRule = {
    /** validation error message */
    message?: React.ReactNode;
    /** built-in validation type, available options: https://github.com/yiminghe/async-validator#type */
    type?: string;
    /** indicates whether field is required */
    required?: boolean;
    /** treat required fields that only contain whitespace as errors */
    whitespace?: boolean;
    /** validate the exact length of a field */
    len?: number;
    /** validate the min length of a field */
    min?: number;
    /** validate the max length of a field */
    max?: number;
    /** validate the value from a list of possible values */
    enum?: string | string[];
    /** validate from a regular expression */
    pattern?: RegExp;
    /** transform a value before validation */
    transform?: (value: any) => any;
    /** custom validate function (Note: callback must be called) */
    validator?: (rule: any, value: any, callback: any, source?: any, options?: any) => any;
};
