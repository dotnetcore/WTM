import { EntitiesPageStore } from '@leng/public/src';
import { Col, Form } from 'ant-design-vue';
import { WrappedFormUtils } from 'ant-design-vue/types/form/form';
import lodash from 'lodash';
import { Observable, Subject, Subscription } from 'rxjs';
import { debounceTime, filter } from "rxjs/operators";
import Vue, { CreateElement } from 'vue';
import { Component, Prop, Inject, Provide } from "vue-property-decorator";
import globalConfig from '../../global.config';
import displayComponents from './display.vue';
import { FormItem, FormItemComponents, RenderFormItemParams, SpanType, FormItemsOptions } from './type';
export function createFilterFormItem(options: FormItemsOptions): FormItemComponents {
    return createFormItem(lodash.merge({ colProps: { xs: 24, sm: 24, md: 12, lg: 8, xl: 6, xxl: 6 }, labelCol: { span: 6 } }, options));
};
export function createFormItem(options: FormItemsOptions): FormItemComponents {
    const { entities } = options = lodash.merge({ colProps: { xs: 24, sm: 24, md: 12, lg: 12, xl: 8, xxl: 6, span: undefined } }, options);
    const FormItems = lodash.mapValues(entities, (item: FormItem, key) => {
        return CreateFormItem(item, key, options)
    })
    // 转换 Entity.ITCode ---->  Entity-ITCode //entityItCode
    return lodash.mapKeys(FormItems, (value, key) => key.replace(/(\.)/g, "-"));
};
function CreateFormItem(item: FormItem, key, options: FormItemsOptions) {
    let colProps: any = lodash.get(item, 'span', options.colProps); //lodash.merge({ xs: 24, sm: 24, md: 12, lg: 12, xl: 8, xxl: 6 }, item.span);
    if (lodash.isNumber(colProps) || lodash.isString(colProps)) {
        colProps = { span: colProps };
    }
    const components = lodash.merge({
        display: displayComponents,
        // FormItemChildren: () => lodash.isObject(item.children) && item.children,
    }, item.components);
    if (lodash.isObject(item.children)) {
        lodash.set(components, 'FormItemChildren', item.children);
    }
    @Component({
        components,
        template: `
        <a-col
        :span="${colProps.span}"
        :xs="${colProps.xs}"
        :sm="${colProps.sm}"
        :md="${colProps.md}"
        :lg="${colProps.lg}"
        >
            <a-form-item :label="label" :label-col="labelCol" >
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
                        ${CreateChildrenTemplate(item)}
                    </template>
                </a-spin>
            </a-form-item>
        </a-col>
        `,

    })
    class FieldItem extends Vue {
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
            return lodash.merge({}, options.labelCol)
        }

        get label() {
            if (lodash.isObject(item.label)) {
                return lodash.get(item.label, this.$GlobalConfig.settings.language);
            }
            return item.label
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
    return FieldItem
}
/**
 * 创建 Children
 * @param item 
 */
function CreateChildrenTemplate(item: FormItem) {
    // item = lodash.cloneDeep(item);
    let label = item.label;
    if (lodash.isObject(label)) {
        label = lodash.get(label, globalConfig.settings.language);
    }
    let placeholder = `$t('placeholder.input', { label:'${label}'  })`
    if (lodash.isObject(item.children)) {
        return `<FormItemChildren v-decorator="decorator"  :placeholder="${placeholder}" />`
    }
    let children = lodash.replace(item.children, 'v-decorator', ` 
            v-decorator="decorator" 
            :disabled="isDisabled" 
            allowClear
            WTM
        `);
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
    // lodash.set(item, 'children', );
    return lodash.replace(children, 'WTM', `:placeholder="${placeholder}"`);
}