import { FieldDecoratorOptions, WrappedFormUtils } from 'ant-design-vue/types/form/form';
import { Observable } from 'rxjs';
import { AsyncComponent, Component as VueComponent } from 'vue';
type ColSpanType = number | string;

export type SpanType = {
    /**
   * raster number of cells to occupy, 0 corresponds to display: none
   * @default none (0)
   * @type ColSpanType
   */
    span?: ColSpanType;
    /**
   * <576px and also default setting, could be a span value or an object containing above props
   * @type { span: ColSpanType, offset: ColSpanType } | ColSpanType
   */
    xs?: { span: ColSpanType; offset: ColSpanType } | ColSpanType;

    /**
     * ≥576px, could be a span value or an object containing above props
     * @type { span: ColSpanType, offset: ColSpanType } | ColSpanType
     */
    sm?: { span: ColSpanType; offset: ColSpanType } | ColSpanType;

    /**
     * ≥768px, could be a span value or an object containing above props
     * @type { span: ColSpanType, offset: ColSpanType } | ColSpanType
     */
    md?: { span: ColSpanType; offset: ColSpanType } | ColSpanType;

    /**
     * ≥992px, could be a span value or an object containing above props
     * @type { span: ColSpanType, offset: ColSpanType } | ColSpanType
     */
    lg?: { span: ColSpanType; offset: ColSpanType } | ColSpanType;

    /**
     * ≥1200px, could be a span value or an object containing above props
     * @type { span: ColSpanType, offset: ColSpanType } | ColSpanType
     */
    xl?: { span: ColSpanType; offset: ColSpanType } | ColSpanType;

    /**
     * ≥1600px, could be a span value or an object containing above props
     * @type { span: ColSpanType, offset: ColSpanType } | ColSpanType
     */
    xxl?: { span: ColSpanType; offset: ColSpanType } | ColSpanType;
};
declare type dataSource = any[] | Observable<any[]> | Promise<any[]>;
interface dataSourceFnParams {
    linkageValue?: any
    props?: any
    fields?: any
    form?: WrappedFormUtils
}
declare type dataSourceFn = (params: dataSourceFnParams) => dataSource;
export declare type FormItemComponents = { [key: string]: VueComponent<any, any, any, any> | AsyncComponent<any, any, any, any> };
export interface FormItem {
    /**
     * 显示 标签 文字
     *
     * @type {(string | { [key: string]: string })}
     * @memberof FormItem
     */
    label: string | { [key: string]: string };
    /**
     * 表单 配置  getFieldDecorator
     * https://www.antdv.com/components/form-cn/
     * @type {FieldDecoratorOptions}
     * @memberof FormItem
     */
    options?: FieldDecoratorOptions;
    /**
     * 将 value 转换 为对象 { [mapKey]:value }
     * 字符串 或者 函数
     */
    mapKey?: string;
    /**
     * 联动
     */
    linkage?: string[];
    /**
     * 数据源 
     *
     * @type {(any[] | Observable<any[]> | Promise<any[]>)}
     * @memberof FormItem
     */
    dataSource?: dataSource | dataSourceFn;
    /**
     * 栅格布局 Col span   
     * https://www.antdv.com/components/grid-cn/
     * @type {SpanType}
     * @memberof FormItem
     */
    span?: SpanType | ColSpanType;
    /**
     * 注入 组件
     *
     * @type {({ [key: string]: Component<any, any, any, any> | AsyncComponent<any, any, any, any> })}
     * @memberof FormItem
     */
    components?: FormItemComponents;
    /**
     * 表单组件
     *
     * @type {*}
     * @memberof FormItem
     */
    children: any;
}
export interface EntitiesItems {
    [key: string]: FormItem
}
export type FormItemsOptions = {
    /**
     * 实体
     */
    entities: EntitiesItems;
    /**
     * col span
     */
    colProps?: SpanType
    /**
     * labelCol span
     */
    labelCol?: SpanType
};
export interface RenderFormItemParams {
    /**
     * 实体模型
     *
     * @type {*}
     * @memberof RenderFormItemParams
     */
    entities: any;
    /**
     * 表单对象
     *
     * @type {WrappedFormUtils}
     * @memberof RenderFormItemParams
     */
    form: WrappedFormUtils;
    /**
     * 默认值
     *
     * @type {*}
     * @memberof RenderFormItemParams
     */
    initialValues?: any;
    /**
     * 栅格布局 Col span   
     *
     * @type {*}
     * @memberof RenderFormItemParams
     */
    ColProps?: any;
}