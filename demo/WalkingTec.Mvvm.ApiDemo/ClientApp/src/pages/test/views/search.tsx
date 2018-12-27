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
            <Col {...colLayout} key="ITCode">
                <FormItem label="ITCode" {...formItemLayout}>
                    {getFieldDecorator('ITCode', {
                        initialValue: Store.searchParams['ITCode'],
                    })(
                        <Input placeholder="请输入 ITCode" />
                    )}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="Name">
                <FormItem label="Name" {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        initialValue: Store.searchParams['Name'],
                    })(
                        <Input placeholder="请输入 Name" />
                    )}
                </FormItem>
            </Col>,
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
