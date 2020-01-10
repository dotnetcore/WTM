import { Col, Form } from 'ant-design-vue';
import { FieldDecoratorOptions, WrappedFormUtils } from 'ant-design-vue/types/form/form';
import lodash from 'lodash';
import Vue, { CreateElement } from 'vue';
import displayComponents from './display.vue';
import { Observable } from 'rxjs';
interface FormItem {
    label: string | { [key: string]: string };
    options?: FieldDecoratorOptions;
    dataSource?: any[] | Observable<any[]> | Promise<any[]>;
    children: any;
}
export interface EntitiesItems {
    [key: string]: FormItem
}
export interface RenderFormItemParams {
    entities: any;
    form: WrappedFormUtils;
    initialValues?: any;
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
        const ColProps = { xs: 24, sm: 24, md: 12, lg: 12 };
        return Vue.component(key, {
            components: {
                display: displayComponents
            },
            props: ['options', 'display', 'disabled', 'decoratorOptions'],
            template: `
            <a-col
               :xs="${ColProps.xs}"
               :sm="${ColProps.sm}"
               :md="${ColProps.md}"
               :lg="${ColProps.lg}"
            >
                <a-form-item label="${item.label}">
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
                } else if (itemDataSource instanceof Promise || itemDataSource instanceof Observable) {
                    loadData = true
                }
                return {
                    loadData,
                    dataSource
                }
            },
            mounted() {
                if (this.loadData) {
                    this.onLoadData()
                }
            },
            methods: {
                onLoadData: async function () {
                    const itemDataSource = await lodash.get(item, 'dataSource');
                    this.dataSource = lodash.map(itemDataSource, (data: any) => {
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
    let children = lodash.replace(item.children, 'v-decorator', ` 
            v-decorator="decorator" 
            :disabled="isDisabled" 
            WTM
        `);
    // placeholder='请输入 ${item.label}'
    switch (true) {
        // selext 文本框
        case lodash.startsWith(item.children, '<a-select'):
            children = lodash.replace(children, 'WTM', ` 
                :options="dataSource"
                placeholder='请选择 ${item.label}'
            `);
            break;
        case lodash.startsWith(item.children, '<a-transfer'):
            children = lodash.replace(children, 'WTM', ` 
                :dataSource="dataSource"
                :render="item=>item.title"
                placeholder='请选择 ${item.label}'
            `);
            break;

        default:
            break;
    }
    lodash.set(item, 'children', lodash.replace(children, 'WTM', `placeholder='请输入 ${item.label}'`));
    return item;
}