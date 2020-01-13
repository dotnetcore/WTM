import { Col, Form } from 'ant-design-vue';
import { FieldDecoratorOptions, WrappedFormUtils } from 'ant-design-vue/types/form/form';
import lodash from 'lodash';
import { Observable } from 'rxjs';
import Vue, { CreateElement } from 'vue';
import globalConfig from '../../global.config';
import displayComponents from './display.vue';
type ColSpanType = number | string;
type SpanType = {
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
declare type dataSourceFn = () => dataSource;
interface FormItem {
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
    span?: SpanType;
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
/**
* 渲染 模型
*/
export function renderFormItem({ entities, form, initialValues, ColProps }: RenderFormItemParams, h: CreateElement) {
    function render() {
        return lodash.map(entities, (item: FormItem, key) => {
            let itemProps = {
                label: lodash.isString(item.label) ? item.label : '',
                labelCol: { span: 6 }
            }
            if (initialValues && lodash.hasIn(initialValues, 'key')) {
                // console.log("TCL: render -> initialValues", initialValues)
                lodash.set(item.options, 'initialValue', lodash.get(initialValues, 'key'));
            }
            return <Col props={ColProps}>
                <Form.Item props={itemProps} >
                    {form.getFieldDecorator(key, item.options)(item.children)}
                </Form.Item>
            </Col>
        })
    }
    return render()
    // return <Row props={{ gutter: 20 }}>
    //     {render()}
    // </Row>
}
/**
 * 模型转换为 驼峰 form-item  组件 
 * Entity.ITCode ---->  Entity-ITCode //entityItCode
 * @param param0 
 */
export function createFormItem({ entities }: { entities: any }) {
    entities = lodash.mapValues(entities, (item: FormItem, key) => {
        // const options = JSON.stringify(item.options).replace(/"/g, "'");
        // const children = lodash.replace(item.children, 'v-decorator', `v-decorator="['${key}',${options}]"`)
        // const children = createChildrenTemplate(item);
        item = createChildrenTemplate(item)
        const span = lodash.get(item, 'span', { xs: 24, sm: 24, md: 12, lg: 12, xl: 8, xxl: 6, span: undefined }); //lodash.merge({ xs: 24, sm: 24, md: 12, lg: 12, xl: 8, xxl: 6 }, item.span);
        let label = item.label;
        if (lodash.isObject(label)) {
            label = lodash.get(label, globalConfig.settings.language);
        }
        return Vue.component(key, {
            components: {
                display: displayComponents
            },
            props: ['options', 'display', 'disabled', 'decoratorOptions'],
            template: `
            <a-col
               :span="${span.span}"
               :xs="${span.xs}"
               :sm="${span.sm}"
               :md="${span.md}"
               :lg="${span.lg}"
            >
                <a-form-item label="${label}">
                    <template v-if="isDisplay" >
                      <display v-decorator="decorator" />
                    </template>
                    <template v-else >
                        ${item.children}
                    </template>
                </a-form-item>
            </a-col>
            `,
            data() {
                let dataSource = [],
                    loadData = false,
                    options = lodash.get(this, 'options'),
                    itemDataSource = lodash.get(item, 'dataSource');
                if (lodash.isArray(options)) {
                    dataSource = options;
                } else if (lodash.isArray(itemDataSource)) {
                    dataSource = itemDataSource;
                } else if (itemDataSource instanceof Promise || itemDataSource instanceof Observable || lodash.isFunction(itemDataSource)) {
                    loadData = true
                }
                return {
                    formItem: item,
                    loadData,
                    dataSource
                }
            },
            mounted() {
                if (this.loadData) {
                    this.onLoadData(lodash.get(item, 'dataSource'))
                }
            },
            methods: {
                onLoadData: async function (itemDataSource) {
                    if (lodash.isFunction(itemDataSource)) {
                        return this.onLoadData(itemDataSource(this))
                    } else if (itemDataSource instanceof Observable) {
                        return this.onLoadData(itemDataSource.toPromise())
                    }
                    const dataSource = await itemDataSource;
                    this.dataSource = lodash.map(dataSource, (data: any) => {
                        return {
                            ...data,
                            // select
                            label: data.Text,
                            value: data.Value,
                            // transfer
                            key: data.Value,
                            title: data.Text,
                        }
                    })
                }
            },
            computed: {
                // 计算属性的 getter
                decorator() {
                    return [key, lodash.merge({}, item.options, this.decoratorOptions)]
                },
                // 是否禁用
                isDisabled() {
                    return lodash.hasIn(this, 'disabled') ? this.disabled : false;
                },
                // 是否只显示状态
                isDisplay() {
                    return lodash.hasIn(this, 'display') ? this.display : false;
                },
            },
        })
    })
    // 转换 Entity.ITCode ---->  Entity-ITCode //entityItCode
    return lodash.mapKeys(entities, (value, key) => {
        return key.replace(/(\.)/g, "-")
        // return lodash.camelCase(key)
    })
}
/**
 * 创建 Children
 * @param item 
 */
function createChildrenTemplate(item) {
    item = lodash.cloneDeep(item);
    let label = item.label;
    if (lodash.isObject(label)) {
        label = lodash.get(label, globalConfig.settings.language);
    }
    let children = lodash.replace(item.children, 'v-decorator', ` 
            v-decorator="decorator" 
            :disabled="isDisabled" 
            WTM
        `);
    // placeholder='请输入 ${item.label}'
    switch (true) {
        /**
         *  a-select 
         *  a-checkbox-group 
         *  a-radio-group
         * */
        case lodash.startsWith(item.children, '<a-select'):
        case lodash.startsWith(item.children, '<a-checkbox-group'):
        case lodash.startsWith(item.children, '<a-radio-group'):
            children = lodash.replace(children, 'WTM', ` 
                :options="dataSource"
                placeholder='请选择 ${label}'
            `);
            break;
        // transfer 穿梭框
        case lodash.startsWith(item.children, '<a-transfer'):
            children = lodash.replace(children, 'WTM', ` 
                :dataSource="dataSource"
                :render="item=>item.title"
                placeholder='请选择 ${label}'
            `);
            // 设置 vaule 绑定 属性
            item.options = lodash.merge({ valuePropName: 'targetKeys' }, item.options);
            break;
        // a-switch 
        case lodash.startsWith(item.children, '<a-switch'):
            // 设置 vaule 绑定 属性
            item.options = lodash.merge({ valuePropName: 'checked' }, item.options);
            break;

        default:
            break;
    }
    lodash.set(item, 'children', lodash.replace(children, 'WTM', `placeholder='请输入 ${label}'`));
    return item;
}