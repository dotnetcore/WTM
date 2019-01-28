import { Col, Form } from 'antd';
import { DecoratorsSearch } from 'components/dataView/header/search';
import decError from 'components/decorators/error';
import * as React from 'react';
import Store from '../store';
import Models from './models';
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
/**
 * 搜索条件头
 */
@decError
@DecoratorsSearch({
    Store,
    FormItems: ({ getFieldDecorator }) => {
        return [
            <Col {...colLayout} key="ITCode">
                <FormItem label="ITCode" {...formItemLayout}>
                    {getFieldDecorator('ITCode', {
                        initialValue: Store.searchParams['ITCode'],
                    })(Models.ITCode)}
                </FormItem>
            </Col>,
            <Col {...colLayout} key="Name">
                <FormItem label="Name" {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        initialValue: Store.searchParams['Name'],
                    })(Models.Name)}
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
