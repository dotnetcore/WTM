import { EntitiesUserStore } from '@leng/public/src';
import { Button, Form, Input, notification } from 'antd';
// import { WrappedFormUtils } from 'antd/lib/form/Form';
import { inject, observer } from 'mobx-react';
import * as React from 'react';
export interface IAppProps {
    UserStore?: EntitiesUserStore,
    form?: any
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
        // const { getFieldDecorator, getFieldsError } = this.props.form;
        // const disabled = hasErrors(getFieldsError())
        return (
            <div></div>
        );
    }
}
export default NormalLoginForm//Form.create({ name: 'normal_login' })(NormalLoginForm);