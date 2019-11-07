
import { Button, Form, Icon, Input, Popconfirm } from 'antd';
import { DesForm } from 'components/decorators';
import { default as GlobalConfig, default as globalConfig } from 'global.config';
import * as React from 'react';
import store from 'store/index';
import ImgCode from './imgCode';
import { LayoutLogin } from './layout';
import './style.less';

function hasErrors(fieldsError) {
  return Object.keys(fieldsError).some(field => fieldsError[field]);
}
@DesForm
export default class LoginDemo extends React.Component<any, any>{
  state = {
    loading: false,
    notCode: false, //!globalConfig.development,取消注释可以在发布环境出现滑块验证
    visible: false
  }
  onSubmit(e) {
    e.preventDefault();
    if (this.state.loading) {
      return
    }
    this.setState({ loading: true })
    this.props.form.validateFields(async (err, values) => {
      if (!err) {
        try {
          await store.User.Login(values);
        } catch (error) {
          this.setState({ loading: false })
        }
      }
    });
  }
  onSuccess() {
    this.setState({ notCode: false })
  }
  componentDidMount() {
    // To disabled submit button at the beginning.
    try {
      this.props.form.validateFields();
    } catch (error) {

    }
  }
  render() {
    const { getFieldDecorator, getFieldsError, isFieldTouched, getFieldError } = this.props.form;
    const userNameError = isFieldTouched('userid') && getFieldError('userid');
    const passwordError = isFieldTouched('password') && getFieldError('password');
    const disabled = hasErrors(getFieldsError())
    return (
      <LayoutLogin>
        <Form onSubmit={this.onSubmit.bind(this)} >
          <h1>{GlobalConfig.settings.title}</h1>
          <Form.Item
            validateStatus={userNameError ? 'error' : ''}
            help={userNameError || ''}
          >
            {getFieldDecorator('userid', {
              rules: [{ required: true, message: '请输入 用户名!' }],
              initialValue: globalConfig.development ? "admin" : ''
            })(
              <Input prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />} placeholder="Username" size="large" />
            )}
          </Form.Item>
          <Form.Item
            validateStatus={passwordError ? 'error' : ''}
            help={passwordError || ''}
          >
            {getFieldDecorator('password', {
              rules: [{ required: true, message: '请输入 密码!' }],
              initialValue: globalConfig.development ? "000000" : ""
            })(
              <Input prefix={<Icon type="lock" style={{ color: 'rgba(0,0,0,.25)' }} />} type="password" placeholder="Password" size="large"/>
            )}
          </Form.Item>
          <div
            onMouseEnter={e => {
              if (this.state.notCode && !disabled) {
                this.setState({ visible: true })
              }
            }}
            onMouseOut={e => {
              if (!this.state.notCode) {
                this.setState({ visible: false })
              }
            }}>
            <Form.Item >
              <Popconfirm
                overlayClassName="app-login-imgcode"
                placement="top"
                title={<ImgCode onSuccess={this.onSuccess.bind(this)} key={String(this.state.notCode)} />}
                trigger="hover"
                visible={this.state.visible && this.state.notCode}
                icon="" >
                <Button
                  disabled={disabled || this.state.notCode}
                  type="primary" htmlType="submit" block size="large"
                  loading={this.state.loading}>
                  <span > Log in</span>
                </Button>
              </Popconfirm>
            </Form.Item>
          </div>
        </Form>
      </LayoutLogin>
    );
  }
}
