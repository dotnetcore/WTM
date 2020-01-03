// import { ValidationRule } from 'antd/lib/form/Form'
declare module '*.svg'
declare module '*.png'
declare module '*.jpg'
declare module '*.jpeg'
declare module '*.gif'
declare module '*.bmp'
declare module '*.tiff'
declare module '*.json'
declare module "*.module.less" {
    const classes: { [key: string]: string };
    export default classes;
}
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
            label: React.ReactNode;
            rules: ValidationRule[];
            formItem: React.ReactNode;
            /** 传递给 formItem 组件的 附加 props table 中有效  */
            formItemProps?: {
                /** 隐藏域 */
                hidden?: boolean;
                /** 模型的 value 优先级大于  defaultValues 中的值*/
                value?: any;
                /** 禁用 控件输入更改 */
                disabled?: boolean;
                /** 禁用 组件 显示 span */
                display?: boolean;
                /** 默认值 列表 */
                defaultValues?: Object;
            }
            /** 传递给 columns 组件的 附加 props table 中有效  */
            columnsProps?: ColumnProps<any>
        }
    }
    interface PageProps {
        /** 页面默认参数 */
        defaultSearchParams: {
            [key: string]: any;
        };
        [key: string]: any;
    }
    interface FormProps {
        loadData?: Function | Object;
        defaultValues?: any;
        form?: WrappedFormUtils;
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
interface ColumnProps<T> {
    title?: React.ReactNode | ((options: {
        filters: TableStateFilters;
        sortOrder?: SortOrder;
    }) => React.ReactNode);
    key?: React.Key;
    dataIndex?: string;
    render?: (text: any, record: T, index: number) => React.ReactNode;
    align?: 'left' | 'right' | 'center';
    filters?: ColumnFilterItem[];
    onFilter?: (value: any, record: T) => boolean;
    filterMultiple?: boolean;
    filterDropdown?: React.ReactNode | ((props: Object) => React.ReactNode);
    filterDropdownVisible?: boolean;
    onFilterDropdownVisibleChange?: (visible: boolean) => void;
    sorter?: boolean | CompareFn<T>;
    defaultSortOrder?: SortOrder;
    colSpan?: number;
    width?: string | number;
    className?: string;
    fixed?: boolean | ('left' | 'right');
    filterIcon?: React.ReactNode | ((filtered: boolean) => React.ReactNode);
    filteredValue?: any[];
    sortOrder?: SortOrder | boolean;
    children?: ColumnProps<T>[];
    onCellClick?: (record: T, event: any) => void;
    onCell?: (record: T, rowIndex: number) => any;
    onHeaderCell?: (props: ColumnProps<T>) => any;
    sortDirections?: SortOrder[];
}
declare type WrappedFormUtils<V = any> = {
    /** 获取一组输入控件的值，如不传入参数，则获取全部组件的值 */
    getFieldsValue(fieldNames?: Array<string>): {
        [field: string]: any;
    };
    /** 获取一个输入控件的值 */
    getFieldValue(fieldName: string): any;
    /** 设置一组输入控件的值 */
    setFieldsValue(obj: Object): void;
    /** 设置一组输入控件的值 */
    setFields(obj: Object): void;
    /** 校验并获取一组输入域的值与 Error */
    validateFields(fieldNames: Array<string>, options: ValidateFieldsOptions, callback: ValidateCallback<V>): void;
    validateFields(options: ValidateFieldsOptions, callback: ValidateCallback<V>): void;
    validateFields(fieldNames: Array<string>, callback: ValidateCallback<V>): void;
    validateFields(fieldNames: Array<string>, options: ValidateFieldsOptions): void;
    validateFields(fieldNames: Array<string>): void;
    validateFields(callback: ValidateCallback<V>): void;
    validateFields(options: ValidateFieldsOptions): void;
    validateFields(): void;
    /** 与 `validateFields` 相似，但校验完后，如果校验不通过的菜单域不在可见范围内，则自动滚动进可见范围 */
    validateFieldsAndScroll(fieldNames: Array<string>, options: ValidateFieldsOptions, callback: ValidateCallback<V>): void;
    validateFieldsAndScroll(options: ValidateFieldsOptions, callback: ValidateCallback<V>): void;
    validateFieldsAndScroll(fieldNames: Array<string>, callback: ValidateCallback<V>): void;
    validateFieldsAndScroll(fieldNames: Array<string>, options: ValidateFieldsOptions): void;
    validateFieldsAndScroll(fieldNames: Array<string>): void;
    validateFieldsAndScroll(callback: ValidateCallback<V>): void;
    validateFieldsAndScroll(options: ValidateFieldsOptions): void;
    validateFieldsAndScroll(): void;
    /** 获取某个输入控件的 Error */
    getFieldError(name: string): Object[];
    getFieldsError(names?: Array<string>): Object;
    /** 判断一个输入控件是否在校验状态 */
    isFieldValidating(name: string): boolean;
    isFieldTouched(name: string): boolean;
    isFieldsTouched(names?: Array<string>): boolean;
    /** 重置一组输入控件的值与状态，如不传入参数，则重置所有组件 */
    resetFields(names?: Array<string>): void;
    getFieldDecorator<T extends Object = {}>(id: keyof T, options?: GetFieldDecoratorOptions): (node: React.ReactNode) => React.ReactNode;
};