import { Form } from 'antd';
import { DecoratorsSearch } from 'components/dataView/header/search';
import { DesError } from 'components/decorators';
import * as React from 'react';
import Store from '../store';
import Models from './models';
const FormItem = Form.Item;
const formItemLayout = {
    labelCol: {
        xs: { span: 24 },
        sm: { span: 6 },
    },
    wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
    },
};
/**
 * 搜索条件头
 */
@DesError
@DecoratorsSearch({
    Store,
    // columnCount:3,
    onValueMap(values) {
        console.log("搜索参数", values);
        return values
    },
    FormItems: ({ getFieldDecorator }) => {
        return [
            <FormItem label="ITCode" key="ITCode" {...formItemLayout}>
                {getFieldDecorator('ITCode', {
                    initialValue: Store.searchParams['ITCode'],
                })(Models.ITCode)}
            </FormItem>
            ,
            <FormItem label="Name" key="Name" {...formItemLayout}>
                {getFieldDecorator('Name', {
                    initialValue: Store.searchParams['Name'],
                })(Models.Name)}
            </FormItem>
            ,
            <FormItem label="性别" key="Sex" {...formItemLayout}>
                {getFieldDecorator('Sex', {
                    initialValue: Store.searchParams['Sex'],
                })(Models.Sex)}
            </FormItem>
            ,
            <FormItem label="test2" key="test2" {...formItemLayout}>
                {getFieldDecorator('test2', {
                    initialValue: Store.searchParams['test2'],
                })(Models.Name)}
            </FormItem>
            ,
            <FormItem label="test3" key="test3" {...formItemLayout}>
                {getFieldDecorator('test3', {
                    initialValue: Store.searchParams['test2'],
                })(Models.Name)}
            </FormItem>
        ]
    }
})
export default class extends React.Component<any, any> {
    shouldComponentUpdate() {
        return false
    }
    render() {
        return null
    }
}
