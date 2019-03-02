
import { Button, Form, Icon, Input, Row } from 'antd';
import { DesForm } from 'components/decorators';
import Animate from 'rc-animate';
import * as React from 'react';
import store from 'store/index';
import './style.less';
function hasErrors(fieldsError) {
  return Object.keys(fieldsError).some(field => fieldsError[field]);
}
@DesForm
export default class LoginDemo extends React.Component<any, any>{
  state = {
    loading: false
  }
  onSubmit(e) {
    e.preventDefault();
    if (this.state.loading) {
      return
    }
    this.props.form.validateFields(async (err, values) => {
      if (!err) {
        this.setState({ loading: true })
        await store.User.Login(values);
        this.setState({ loading: false })
      }
    });
  }
  componentDidMount() {
    // To disabled submit button at the beginning.
    this.props.form.validateFields();
  }
  render() {
    const { getFieldDecorator, getFieldsError, isFieldTouched, getFieldError } = this.props.form;
    const userNameError = isFieldTouched('userid') && getFieldError('userid');
    const passwordError = isFieldTouched('password') && getFieldError('password');
    return (
      <Animate transitionName="fade"
        transitionAppear={true} component="">
        <Row type="flex" justify="center" align="middle" className='app-login' >
          <Form onSubmit={this.onSubmit.bind(this)} className="app-login-form">
            <h1>WTM</h1>
            <Form.Item
              validateStatus={userNameError ? 'error' : ''}
              help={userNameError || ''}
            >
              {getFieldDecorator('userid', {
                rules: [{ required: true, message: '请输入 用户名!' }],
              })(
                <Input prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />} placeholder="Username" />
              )}
            </Form.Item>
            <Form.Item
              validateStatus={passwordError ? 'error' : ''}
              help={passwordError || ''}
            >
              {getFieldDecorator('password', {
                rules: [{ required: true, message: '请输入 密码!' }],
              })(
                <Input prefix={<Icon type="lock" style={{ color: 'rgba(0,0,0,.25)' }} />} type="password" placeholder="Password" />
              )}
            </Form.Item>
            <Form.Item>
              <Button disabled={hasErrors(getFieldsError())} type="primary" htmlType="submit" block loading={this.state.loading}>
                <span> Log in</span>
              </Button>
            </Form.Item>
          </Form>
        </Row>

      </Animate  >

    );
  }
}