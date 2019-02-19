import { Form } from 'antd';
import { GetFieldDecoratorOptions, WrappedFormUtils } from 'antd/lib/form/Form';
import { FormItemProps } from 'antd/lib/form/FormItem';
import { DataViewSearch } from 'components/dataView/header/search';
import { DesForm } from 'components/decorators';
import GlobalConfig from 'global.config'; //全局配置
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import ModelsCreate from './models'; //模型
const Models = ModelsCreate();
const formItemLayout = {
    ...GlobalConfig.formItemLayout
};
@DesForm
@observer
export default class extends React.Component<any, any> {
    render() {
        const { getFieldDecorator } = this.props.form;
        return <DataViewSearch
            // columnCount={4} 默认全局
            // onReset={() => { }} 覆盖默认方法
            // onSubmit={() => { }} 覆盖默认方法
            Store={Store}
            form={this.props.form}
        >
            <FormItem {...this.props} fieId="ITCode" />
            <FormItem {...this.props} fieId="Name" />
        </DataViewSearch>
    }
}
/**
 * 表单item
 */
interface IFormItemProps {
    fieId: string;
    defaultValue?: boolean;
    FormItemProps?: FormItemProps;
    decoratorOptions?: GetFieldDecoratorOptions
    [key: string]: any;
}
@observer
class FormItem extends React.Component<IFormItemProps, any> {
    render() {
        const { form, fieId, defaultValue, decoratorOptions } = this.props;
        const { getFieldDecorator }: WrappedFormUtils = form;
        const model = lodash.get(Models, fieId);
        let options: GetFieldDecoratorOptions = {
            // rules: model.rules,
            ...decoratorOptions
        };
        // 获取默认值
        if (defaultValue && typeof defaultValue === "boolean") {
            options.initialValue = lodash.get(Store.searchParams, fieId);
        }
        return <Form.Item label={model.label}  {...formItemLayout}>
            {getFieldDecorator(this.props.fieId, options)(model.formItem)}
        </Form.Item >
    }
}