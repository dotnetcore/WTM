import { Form } from 'antd';
import { DataViewSearch } from 'components/dataView/header/search';
import { DesForm } from 'components/decorators';
import GlobalConfig from 'global.config'; //全局配置
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import Models from './models';
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
            <Form.Item label="ITCode" {...formItemLayout}>
                {getFieldDecorator('ITCode', {
                    initialValue: Store.searchParams['ITCode'],
                })(Models.ITCode)}
            </Form.Item>
            <Form.Item label="Name"  {...formItemLayout}>
                {getFieldDecorator('Name', {
                    initialValue: Store.searchParams['Name'],
                })(Models.Name)}
            </Form.Item>
            <Form.Item label="性别" {...formItemLayout}>
                {getFieldDecorator('Sex', {
                    initialValue: Store.searchParams['Sex'],
                })(Models.Sex)}
            </Form.Item>
            <Form.Item label="test2"  {...formItemLayout}>
                {getFieldDecorator('test2', {
                    initialValue: Store.searchParams['test2'],
                })(Models.Name)}
            </Form.Item>
            <Form.Item label="test3" {...formItemLayout}>
                {getFieldDecorator('test3', {
                    initialValue: Store.searchParams['test2'],
                })(Models.Name)}
            </Form.Item>
        </DataViewSearch>
    }
}
