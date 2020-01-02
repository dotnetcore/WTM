import { Col, Form, Row } from 'ant-design-vue';
import { FieldDecoratorOptions, WrappedFormUtils } from 'ant-design-vue/types/form/form';
import lodash from 'lodash';
import Vue, { CreateElement } from 'vue';
import { Help } from '@leng/public/src/utils/helps';
interface FormItem {
    label: string | { [key: string]: string };
    options?: FieldDecoratorOptions;
    children: any;
}
export interface EntitiesItems {
    [key: string]: FormItem
}
/**
* 渲染 模型
*/
export function renderFormItem({ entities, form, ColProps }: { entities: any, form: WrappedFormUtils, ColProps?: any }, h: CreateElement) {
    function render() {
        return lodash.map(entities, (item: FormItem, key) => {
            let itemProps = {
                label: lodash.isString(item.label) ? item.label : '',
                labelCol: { span: 6 }
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
        const options = JSON.stringify(item.options).replace(/"/g, "'");
        // const children = lodash.replace(item.children, 'v-decorator', `v-decorator="['${key}',${options}]"`)
        const children = lodash.replace(item.children, 'v-decorator', ` 
            v-decorator="decorator" 
            :disabled="isDisabled" 
        `)
        return Vue.component(key, {
            props: ['options', 'disabled', 'decoratorOptions'],
            template: `
            <a-form-item label="${item.label}">
               ${children}
            </a-form-item>
            `,
            mounted() {
            },
            computed: {
                // 计算属性的 getter
                decorator: function () {
                    return [key, lodash.merge({}, item.options, this.decoratorOptions)]
                },
                isDisabled: function () {
                    return lodash.hasIn(this, 'disabled') ? this.disabled : false;
                },
            },
        })
    })
    return lodash.mapKeys(entities, (value, key) => {
        return key.replace(/(\.)/g, "-")
        // return lodash.camelCase(key)
    })
}