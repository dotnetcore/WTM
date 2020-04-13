import { EntitiesPageStore } from '@leng/public/src';
import { Col, Form } from 'ant-design-vue';
import { WrappedFormUtils } from 'ant-design-vue/types/form/form';
import lodash from 'lodash';
import { Observable, Subject, Subscription } from 'rxjs';
import { debounceTime, filter } from "rxjs/operators";
import Vue, { CreateElement } from 'vue';
import { Component, Prop, Inject } from "vue-property-decorator";
import globalConfig from '../../global.config';
import displayComponents from './display.vue';
import { FormItem, FormItemComponents, RenderFormItemParams, SpanType } from './type';
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

declare type CreateFormItem = {
    /**
     * 实体
     */
    entities: any;
    /**
     * 异步组件
     */
    async?: boolean;
    /**
     * col span
     */
    colProps?: SpanType
    /**
     * labelCol span
     */
    labelCol?: SpanType
};
/**
 * 模型转换为 驼峰 form-item  组件 
 * Entity.ITCode ---->  Entity-ITCode //entityItCode
 * 默认 返回 异步组件
 * @param param0 
 */
export function createFilterFormItem({
    entities,
    ...props
}: CreateFormItem): FormItemComponents {
    return createFormItem({
        entities, colProps: { xs: 24, sm: 24, md: 12, lg: 8, xl: 6, xxl: 6 }, labelCol: { span: 6 }, ...props
    })
}
/**
 * 模型转换为 驼峰 form-item  组件 
 * Entity.ITCode ---->  Entity-ITCode //entityItCode
 * 默认 返回 异步组件
 * @param param0 
 */
export function createFormItem({
    entities,
    async = true,
    colProps = { xs: 24, sm: 24, md: 12, lg: 12, xl: 8, xxl: 6, span: undefined },
    labelCol = {},
}: CreateFormItem): FormItemComponents {
    // colProps = ;//{ xs: 24, sm: 24, md: 12, lg: 8 }
    // const FieldsChange = new Subject<{
    //     props: any;
    //     fields: any;
    //     form: WrappedFormUtils;
    // }>();
    entities = lodash.mapValues(entities, (item: FormItem, key) => {
        // const options = JSON.stringify(item.options).replace(/"/g, "'");
        // const children = lodash.replace(item.children, 'v-decorator', `v-decorator="['${key}',${options}]"`)
        // const children = createChildrenTemplate(item);
        item = createChildrenTemplate(item)
        let span: any = lodash.get(item, 'span', colProps); //lodash.merge({ xs: 24, sm: 24, md: 12, lg: 12, xl: 8, xxl: 6 }, item.span);
        if (lodash.isNumber() || lodash.isString(span)) {
            span = { span: span };
        }
        let label = item.label;
        if (lodash.isObject(label)) {
            label = lodash.get(label, globalConfig.settings.language);
        }
        // return createFieldItem({ item, span, label, key })
        /**
         * 创建 异步 组件
         */
        // if (async) {
        //     const asyncComponent = new Observable(sub => {
        //         // 设置 组件 的 onComplete 初始化 完成函数
        //         lodash.set(item, 'onComplete', (params) => {
        //             sub.next(createFieldItem(lodash.merge({ item, span, label, labelCol, key }, params)))
        //             sub.complete()
        //         });
        //     }).toPromise();
        //     return () => asyncComponent
        // }
        return createFieldItem({ item, span, label, labelCol, key })
    })
    // 转换 Entity.ITCode ---->  Entity-ITCode //entityItCode
    return lodash.mapKeys(entities, (value, key) => {
        return key.replace(/(\.)/g, "-")
        // return lodash.camelCase(key)
    })
}
/**
 *  创建 Field 组件
 * @param param0 
 */
function createFieldItem({ item, label, labelCol, span, key, FieldsChange, form }: any) {
    @Component({
        components: lodash.merge({
            display: displayComponents
        }, item.components),
        template: `
        <a-col
        :span="${span.span}"
        :xs="${span.xs}"
        :sm="${span.sm}"
        :md="${span.md}"
        :lg="${span.lg}"
        >
            <a-form-item label="${label}" :label-col="labelCol" >
                <a-spin :spinning="spinning" >
                    <a-icon slot="indicator" type="loading" style="font-size: 24px" spin />
                    <template v-if="isDisplay" >
                        <display 
                           v-decorator="decorator" 
                           :dataSource="dataSource"
                           :Entitie="Entitie"
                        />
                    </template>
                    <template v-else >
                        ${item.children}
                    </template>
                </a-spin>
            </a-form-item>
        </a-col>
        `,

    })
    class FieldItem extends Vue {
        @Prop() private PageStore: EntitiesPageStore;
        // @Prop({ default: () => FieldsChange })
        @Inject('FieldsChangeSubject')
        private FieldsChange: Subject<{
            props: any;
            fields: any;
            form: WrappedFormUtils;
        }>;
        @Prop() private display: any;
        @Prop() private disabled: any;
        @Prop() private decoratorOptions: any;
        FieldsChangeSubscription: Subscription;
        // get span() {
        //     return span
        // };
        Entitie = item;
        loadData = false;
        spinning = false;
        dataSource = [];
        // 计算属性的 getter
        get decorator() {
            const options = lodash.merge({}, item.options, this.decoratorOptions);
            // 显示 文本状态 去除 valuePropName 转换
            if (this.isDisplay) {
                lodash.unset(options, 'valuePropName');
            }
            return [key, options];
        }
        // 是否禁用
        get isDisabled() {
            if (lodash.isNil(this.disabled)) {
                return false
            }
            if (lodash.isBoolean(this.disabled)) {
                return this.disabled
            }
            return true
        }
        // 是否只显示状态
        get isDisplay() {
            if (lodash.isNil(this.display)) {
                return false
            }
            if (lodash.isBoolean(this.display)) {
                return this.display
            }
            return true
        }
        get labelCol() {
            return lodash.merge({}, labelCol)
        }
        /**
         * 初始化 数据状态
         */
        created() {
            let dataSource = [],
                loadData = false,
                options = lodash.get(this, 'options'),
                itemDataSource = lodash.get(item, 'dataSource');
            if (lodash.isArray(options)) {
                dataSource = options;
            } else if (lodash.isArray(itemDataSource)) {
                dataSource = this.map(itemDataSource);
            } else if (itemDataSource instanceof Promise || itemDataSource instanceof Observable || lodash.isFunction(itemDataSource)) {
                loadData = true
            }
            this.loadData = loadData;
            this.dataSource = dataSource;
            this.onCreateSubscribe();
        }
        mounted() {
            if (this.loadData) {
                this.onLoadData(lodash.get(item, 'dataSource'))
            }

            // console.log(this)
        }
        beforeDestroy() {
            this.FieldsChangeSubscription && this.FieldsChangeSubscription.unsubscribe();
            this.FieldsChangeSubscription = undefined;
        }
        /**
         * 创建 FieldsChange 监控
         */
        onCreateSubscribe() {
            const { linkage } = item;
            if (this.FieldsChange && linkage) {
                this.FieldsChangeSubscription = this.FieldsChange.pipe(
                    // debounceTime(300),
                    filter(({ props, fields }) => {
                        return linkage.some(link => lodash.hasIn(fields, link))
                    }),
                    // map(({ props, fields }) => {
                    //     let values = {};
                    //     lodash.mapValues(fields, val => {
                    //         console.log("TCL: FieldItem -> onCreateSubscribe -> val", val)
                    //     })
                    //     return {
                    //         values,
                    //         props,
                    //         fields,
                    //     }
                    // })
                ).subscribe(({ props, fields, form }) => {
                    const itemDataSource = lodash.get(item, 'dataSource');
                    if (lodash.isFunction(itemDataSource)) {
                        // console.log("TCL: FieldItem -> lodash.merge({}, form.getFieldsValue(linkage))", lodash.merge({}, form.getFieldsValue(linkage)))
                        const linkageValue = lodash.merge({}, form.getFieldsValue(linkage));
                        console.warn("FieldItem -> createFieldItem -> linkageValue", linkageValue)
                        // form.resetFields([key])
                        return this.onLoadData(
                            itemDataSource({
                                linkageValue,
                                props,
                                fields,
                                form
                            })
                        )
                    } else {
                        console.warn('联动 dataSource 必须为 函数', item)
                    }
                    // console.log("TCL: ViewAction -> beforeCreate -> props", props, fields.Entity.ITCode.value);
                });
            }
        }
        /**
         * 加载 数据
         * @param itemDataSource 
         */
        async  onLoadData(itemDataSource) {
            this.spinning = true;
            if (lodash.isFunction(itemDataSource)) {
                return this.onLoadData(itemDataSource(this))
            } else if (itemDataSource instanceof Observable) {
                return this.onLoadData(itemDataSource.toPromise())
            }
            const dataSource = await itemDataSource;
            this.dataSource = this.map(dataSource);
            this.spinning = false;
        }
        map(dataSource) {
            return lodash.map(dataSource, (data: any) => {
                return {
                    // select
                    label: data.Text,
                    value: data.Value,
                    // transfer
                    key: data.Value,
                    title: data.Text,
                    ...data,
                }
            })
        }
    }
    return FieldItem;
}
/**
 * 创建 Children
 * @param item 
 */
function createChildrenTemplate(item) {
    // item = lodash.cloneDeep(item);
    let label = item.label;
    if (lodash.isObject(label)) {
        label = lodash.get(label, globalConfig.settings.language);
    }
    let children = lodash.replace(item.children, 'v-decorator', ` 
            v-decorator="decorator" 
            :disabled="isDisabled" 
            allowClear
            WTM
        `);
    let placeholder = `$t('placeholder.input', { label:'${label}'  })`
    switch (true) {
        /**
         *  a-select 
         *  a-checkbox-group 
         *  a-radio-group
         * */
        case lodash.startsWith(item.children, '<a-select'):
        case lodash.startsWith(item.children, '<a-checkbox-group'):
        case lodash.startsWith(item.children, '<a-radio-group'):
            placeholder = lodash.replace(placeholder, 'placeholder.input', 'placeholder.choice')
            children = lodash.replace(children, 'WTM', ` 
                :options="dataSource"
                :placeholder="${placeholder}"
            `);
            break;
        // transfer 穿梭框
        case lodash.startsWith(item.children, '<a-transfer'):
            placeholder = lodash.replace(placeholder, 'placeholder.input', 'placeholder.choice')
            children = lodash.replace(children, 'WTM', ` 
                :dataSource="dataSource"
                :render="item=>item.title"
                :placeholder="${placeholder}"
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
    lodash.set(item, 'children', lodash.replace(children, 'WTM', `:placeholder="${placeholder}"`));
    return item;
}