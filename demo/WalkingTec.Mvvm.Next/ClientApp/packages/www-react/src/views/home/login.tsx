import { EntitiesUserStore } from '@leng/public/src';
import { Button, Form, Icon, Input, notification } from 'antd';
import { WrappedFormUtils } from 'antd/lib/form/Form';
import { inject, observer } from 'mobx-react';
import * as React from 'react';
export interface IAppProps {
    UserStore?: EntitiesUserStore,
    form?: WrappedFormUtils
}
function hasErrors(fieldsError) {
    return Object.keys(fieldsError).some(field => fieldsError[field]);
}
@inject('UserStore')
@observer
class NormalLoginForm extends React.Component<IAppProps> {
    handleSubmit = e => {
        e.preventDefault();
        this.props.form.validateFields(async (err, values) => {
            if (!err) {
                try {
                    await this.props.UserStore.onLogin(values.username, values.password)
                } catch (error) {
                    notification.error({ message: 'login error', description: error.message })
                }
            }
        });
    };
    render() {
        const { getFieldDecorator, getFieldsError } = this.props.form;
        const disabled = hasErrors(getFieldsError())
        return (
            <Form onSubmit={this.handleSubmit} style={{ width: 400 }}>
                <Form.Item>
                    {getFieldDecorator('username', {
                        rules: [{ required: true, message: 'Please input your username!' }],
                        initialValue: 'admin'
                    })(
                        <Input
                            prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />}
                            placeholder="Username"
                        />,
                    )}
                </Form.Item>
                <Form.Item>
                    {getFieldDecorator('password', {
                        rules: [{ required: true, message: 'Please input your Password!' }],
                        initialValue: "000000"
                    })(
                        <Input
                            prefix={<Icon type="lock" style={{ color: 'rgba(0,0,0,.25)' }} />}
                            type="password"
                            placeholder="Password"
                        />,
                    )}
                </Form.Item>
                <Form.Item>
                    <Button
                        disabled={disabled}
                        loading={this.props.UserStore.Loading}
                        type="primary"
                        htmlType="submit"
                        className="login-form-button"
                    >
                        Log in
                    </Button>
                </Form.Item>
            </Form>
        );
    }
}
export default Form.create({ name: 'normal_login' })(NormalLoginForm);