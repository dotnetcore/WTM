import * as React from 'react';
import { Link } from 'react-router-dom';
import { Form, Icon, Input, Button } from 'antd';
import { inject, observer } from 'mobx-react';
import Editor from 'components/editer';
const FormItem = Form.Item;
class IApp extends React.Component<any, any> {
    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log(values);
            } else {
                console.log(values);
            }
        });
    }
    public render() {
        const { getFieldDecorator } = this.props.form;
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
        return (
            <Form onSubmit={this.handleSubmit} >
                <FormItem label='内容'>
                    {getFieldDecorator('content', {
                        initialValue: '<p>默认内容</p>',
                        rules: [{ required: true, message: 'Please input content note!' }],
                    })(
                        <Editor />
                    )}
                </FormItem>
                <FormItem label='内容'>
                    {getFieldDecorator('content', {
                        initialValue: '<p>默认内容</p>',
                        rules: [{ required: true, message: 'Please input content note!' }],
                    })(
                        <Editor />
                    )}
                </FormItem>
                <FormItem label='内容'>
                    {getFieldDecorator('content', {
                        initialValue: '<p>默认内容</p>',
                        rules: [{ required: true, message: 'Please input content note!' }],
                    })(
                        <Editor />
                    )}
                </FormItem>
                <FormItem label='内容'>
                    {getFieldDecorator('content', {
                        initialValue: '<p>默认内容</p>',
                        rules: [{ required: true, message: 'Please input content note!' }],
                    })(
                        <Editor />
                    )}
                </FormItem>
                <Button type="primary" htmlType="submit" >
                    提交
          </Button>
            </Form>
        );
    }
}

export default Form.create()(IApp);
