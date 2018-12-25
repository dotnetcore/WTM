import { Col, Form, Input, Button } from 'antd';
import { DecoratorsSearch } from 'components/dataView/header/search';
import * as React from 'react';
import Insert from './edit';
import Store from '../store';
const FormItem = Form.Item;
const colLayout = {
    xl: 6,
    lg: 8,
    md: 12
}
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
@DecoratorsSearch({
    Store,
    FormItems: ({ getFieldDecorator }) => {
        return [
            <Col {...colLayout} key="test">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test', {
                        initialValue: Store.searchParams['test'],
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test1">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test1', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test2">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test2', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test3">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test3', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test4">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test4', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test5">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test5', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test6">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test6', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test7">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test7', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="test8">
                <FormItem label="测试" {...formItemLayout}>
                    {getFieldDecorator('test8', {
                        initialValue: '',
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
        ]
    }
})
export default class extends React.Component<any, any> {
    shouldComponentUpdate() {
        return false
    }
    render() {
        return this.props.children
    }
}
