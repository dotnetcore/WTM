
import Login from 'ant-design-pro/lib/Login';
import { Alert, Checkbox } from 'antd';
import Animate from 'rc-animate';
import * as React from 'react';
import store from 'store/index';
import './style.less';
const { Tab, UserName, Password, Mobile, Captcha, Submit } = Login;
export default class LoginDemo extends React.Component {
  state = {
    notice: '',
    type: 'tab1',
    autoLogin: true,
  }
  onSubmit = (err, values) => {
    console.log('value collected ->', { ...values, autoLogin: this.state.autoLogin });
    if (this.state.type === 'tab1') {
      this.setState({
        notice: '',
      }, () => {
        if (!err && (values.username !== 'admin' || values.password !== '888888')) {
          setTimeout(() => {
            store.User.Login(values);
          }, 500);
        }
      });
    }
  }
  onTabChange = (key) => {
    this.setState({
      type: key,
    });
  }
  changeAutoLogin = (e) => {
    this.setState({
      autoLogin: e.target.checked,
    });
  }
  render() {
    return (
      <Animate transitionName="fade"
        transitionAppear={true} component="">
        <div className="app-login">
          <div className="app-login-form">
            <Login
              defaultActiveKey={this.state.type}
              onTabChange={this.onTabChange}
              onSubmit={this.onSubmit}
            >
              <Tab key="tab1" tab="Account">
                {
                  this.state.notice &&
                  <Alert style={{ marginBottom: 24 }} message={this.state.notice} type="error" showIcon closable />
                }
                <UserName name="username" />
                <Password name="password" />
              </Tab>
              <Tab key="tab2" tab="Mobile">
                <Mobile name="mobile" />
                <Captcha onGetCaptcha={() => console.log('Get captcha!')} name="captcha" />
              </Tab>
              <div>
                <Checkbox checked={this.state.autoLogin} onChange={this.changeAutoLogin}>Keep me logged in</Checkbox>
                <a style={{ float: 'right' }} href="">Forgot password</a>
              </div>
              <Submit>Login</Submit>
              <div>
                Other login methods
             <span className="icon icon-alipay" />
                <span className="icon icon-taobao" />
                <span className="icon icon-weibo" />
                <a style={{ float: 'right' }} href="">Register</a>
              </div>
            </Login>
          </div>
        </div>
      </Animate  >

    );
  }
}