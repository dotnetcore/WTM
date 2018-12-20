import { Col, Form, Input } from 'antd';
import * as React from 'react';
import { DecoratorsTableHeader } from 'components/table/tableHeader';
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
/**
 * 组件继承 支持重写,
 */
@DecoratorsTableHeader(Store)
export default class HeaderComponent extends React.Component<any, any>{
    render() {
        const { form, initialValue } = this.props;
        const { getFieldDecorator } = form;
        return <>
            <Col {...colLayout} >
                <FormItem label="未配置说明" {...formItemLayout}>
                    {getFieldDecorator('createdateFrom', {
                        initialValue: initialValue('createdateFrom', 'date-time'),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="未配置说明" {...formItemLayout}>
                    {getFieldDecorator('createdateTo', {
                        initialValue: initialValue('createdateTo', 'date-time'),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="未配置说明" {...formItemLayout}>
                    {getFieldDecorator('updatedateFrom', {
                        initialValue: initialValue('updatedateFrom', 'date-time'),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="未配置说明" {...formItemLayout}>
                    {getFieldDecorator('updatedateTo', {
                        initialValue: initialValue('updatedateTo', 'date-time'),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="名称" {...formItemLayout}>
                    {getFieldDecorator('name', {
                        initialValue: initialValue('name', ''),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="地址" {...formItemLayout}>
                    {getFieldDecorator('address', {
                        initialValue: initialValue('address', ''),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="备注" {...formItemLayout}>
                    {getFieldDecorator('remark', {
                        initialValue: initialValue('remark', ''),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="创建日期" {...formItemLayout}>
                    {getFieldDecorator('createdate', {
                        initialValue: initialValue('createdate', 'date-time'),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="创建人" {...formItemLayout}>
                    {getFieldDecorator('createby', {
                        initialValue: initialValue('createby', ''),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="修改日期" {...formItemLayout}>
                    {getFieldDecorator('updatedate', {
                        initialValue: initialValue('updatedate', 'date-time'),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
            <Col {...colLayout} >
                <FormItem label="修改人" {...formItemLayout}>
                    {getFieldDecorator('updateby', {
                        initialValue: initialValue('updateby', ''),
                    })(
                        <Input placeholder="请输入" />
                    )}
                </FormItem>
            </Col>
        </>
    }
}

