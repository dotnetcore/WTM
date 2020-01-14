import { Col, Form } from 'ant-design-vue';
import lodash from 'lodash';
import { Observable, Subject, Subscription } from 'rxjs';
import { debounceTime, filter, map } from "rxjs/operators";
import Vue, { CreateElement } from 'vue';
import { Component, Prop } from "vue-property-decorator";
import globalConfig from '../../global.config';
import displayComponents from './display.vue';
import { FormItem, FormItemComponents, RenderFormItemParams } from './type';
import { WrappedFormUtils } from 'ant-design-vue/types/form/form';
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
export function createFormItem({ entities }: { entities: any }): FormItemComponents {
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
                <a-form-item label="${label}">
                    <a-spin :spinning="spinning" >
                        <a-icon slot="indicator" type="loading" style="font-size: 24px" spin />
                        <template v-if="isDisplay" >
                            <display v-decorator="decorator" />
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
            @Prop() private FieldsChange: Subject<{
                props: any;
                fields: any;
                form: WrappedFormUtils;
            }>;
            @Prop() private display: any;
            @Prop() private disabled: any;
            @Prop() private decoratorOptions: any;
            FieldsChangeSubscription: Subscription;
            formItem = item;
            loadData = false;
            spinning = false;
            dataSource = [];
            // 计算属性的 getter
            get decorator() {
                return [key, lodash.merge({}, item.options, this.decoratorOptions)]
            }
            // 是否禁用
            get isDisabled() {
                return lodash.hasIn(this, 'disabled') ? this.disabled : false;
            }
            // 是否只显示状态
            get isDisplay() {
                return lodash.hasIn(this, 'display') ? this.display : false;
            }
            created() {
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
                this.loadData = loadData;
                this.dataSource = dataSource;
            }
            mounted() {
                if (this.loadData) {
                    this.onLoadData(lodash.get(item, 'dataSource'))
                }
                this.onCreateSubscribe();
            }
            beforeDestroy() {
                this.FieldsChangeSubscription && this.FieldsChangeSubscription.unsubscribe();
                this.FieldsChangeSubscription = undefined;
            }
            onCreateSubscribe() {
                const { linkage } = item;
                if (this.FieldsChange && linkage) {
                    this.FieldsChangeSubscription = this.FieldsChange.pipe(
                        debounceTime(300),
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
                            console.log("TCL: FieldItem -> lodash.merge({}, form.getFieldsValue(linkage))", lodash.merge({}, form.getFieldsValue(linkage)))
                            return this.onLoadData(
                                itemDataSource({
                                    linkageValue: lodash.merge({}, form.getFieldsValue(linkage)),
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
            async  onLoadData(itemDataSource) {
                this.spinning = true;
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
                });
                this.spinning = false;
            }
        }
        return FieldItem
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