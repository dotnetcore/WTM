import { Form, Input } from 'antd';
import * as React from 'react';
import { DecoratorsTableEdit } from 'components/table/tableEdit';
import Store from '../store';
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
 * 组件继承 支持重写,
 */
@DecoratorsTableEdit(Store)
export default class EditComponent extends React.Component<any, any>{
    render() {
        const { form, initialValue } = this.props;
        const { getFieldDecorator } = form;
        /**
         *  label 显示描述 
         *  
         *  rules 验证条件 空着就行。
         * 
         *              属性key   类型 '' 就行
         *  initialValue('id', 'int64'),
         * 
         *  
         */
        return <>
            <FormItem label="id" {...formItemLayout}>
                {getFieldDecorator('id', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                    initialValue: initialValue('id', 'int64'),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
            <FormItem label="名称" {...formItemLayout}>
                {getFieldDecorator('name', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }, { "min": 0, "message": "min length 0!" }, { "max": 50, "message": "max length 50!" }],
                    initialValue: initialValue('name', ''),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
            <FormItem label="地址" {...formItemLayout}>
                {getFieldDecorator('address', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }, { "min": 0, "message": "min length 0!" }, { "max": 500, "message": "max length 500!" }],
                    initialValue: initialValue('address', ''),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
            <FormItem label="备注" {...formItemLayout}>
                {getFieldDecorator('remark', {
                    rules: [{ "min": 0, "message": "min length 0!" }, { "max": 500, "message": "max length 500!" }],
                    initialValue: initialValue('remark', ''),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
            <FormItem label="创建日期" {...formItemLayout}>
                {getFieldDecorator('createdate', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                    initialValue: initialValue('createdate', 'date-time'),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
            <FormItem label="创建人" {...formItemLayout}>
                {getFieldDecorator('createby', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }, { "min": 0, "message": "min length 0!" }, { "max": 50, "message": "max length 50!" }],
                    initialValue: initialValue('createby', ''),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
            <FormItem label="修改日期" {...formItemLayout}>
                {getFieldDecorator('updatedate', {
                    rules: [],
                    initialValue: initialValue('updatedate', 'date-time'),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
            <FormItem label="修改人" {...formItemLayout}>
                {getFieldDecorator('updateby', {
                    rules: [{ "min": 0, "message": "min length 0!" }, { "max": 50, "message": "max length 50!" }],
                    initialValue: initialValue('updateby', ''),
                })(
                    <Input placeholder="请输入" />
                )}
            </FormItem>
        </>
    }
}