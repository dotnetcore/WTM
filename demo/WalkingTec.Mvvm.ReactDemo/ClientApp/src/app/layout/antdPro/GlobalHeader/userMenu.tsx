
import { Avatar, Dropdown, Icon, Input, Menu } from 'antd';
import classNames from 'classnames';
import { DialogForm, DialogFormDes, FormItem, InfoShellLayout } from 'components/dataView';
import globalConfig from 'global.config';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/index';
import Request from 'utils/Request';
import RequestFiles from 'utils/RequestFiles';
import styles from './index.module.less';
@observer
export default class UserMenu extends React.Component<any, any> {
  render() {
    const { theme, layout = 'sidemenu' } = this.props;
    let className = styles.userName;
    if (theme === 'dark' && layout === 'topmenu') {
      className = `${styles.userName}  ${styles.dark}`;
    }
    return (
      <Dropdown overlayClassName={classNames(styles.userDropdown)} overlay={
        <Menu>
          {globalConfig.development && <Menu.Item>
            <a href="/_codegen?ui=react" target="_blank">  <Icon type='code' /> 代码生成器</a>
          </Menu.Item>}
          {globalConfig.development && <Menu.Item>
            <a href="/swagger" target="_blank">  <Icon type='bars' /> API文档</a>
          </Menu.Item>}
          <Menu.Item>
            <DialogForm
              title="修改密码"
              icon="user"
              type="a"
            >
              <InsertForm />
            </DialogForm>
          </Menu.Item>
          <Menu.Item>
            <a onClick={e => { Store.User.outLogin() }}>  <Icon type='logout' /> 退出</a>
          </Menu.Item>
        </Menu>
      } placement="bottomCenter">
        <div className={classNames(styles.userAvatar)} >
          <Avatar size="large" icon="user" src={Store.User.UserInfo.PhotoId ? RequestFiles.onFileUrl(Store.User.UserInfo.PhotoId) : globalConfig.default.avatar} />
          &nbsp;<span className={classNames(className)}>{Store.User.UserInfo.Name}</span>
        </div>
      </Dropdown>
    );
  }
}

@DialogFormDes({
  onFormSubmit(values) {
    return Request.ajax({
      url: "/api/_login/ChangePassword",
      method: "post",
      body: values
    }).toPromise()
  }
})
@observer
export class InsertForm extends React.Component<any, any> {
  models = {
    "OldPassword": {
      label: "旧密码",
      rules: [{ "required": true, "message": "旧密码不能为空" }],
      formItem: <Input.Password placeholder="请输入 旧密码" />
    },
    "NewPassword": {
      label: "新密码",
      rules: [{ "required": true, "message": "新密码不能为空" }],
      formItem: <Input.Password placeholder="请输入 新密码" />
    },
    "NewPasswordComfirm": {
      label: "确认秘密",
      rules: [{ "required": true, "message": "确认新密码不能为空" }, {
        validator: (rule, value, callback) => {
          const form = this.props.form;
          if (value && value !== form.getFieldValue('NewPassword')) {
            callback('新密码不一致!');
          } else {
            callback();
          }
        }
      }],
      formItem: <Input.Password placeholder="确认秘密" />
    },
  };
  render() {
    const props = {
      ...this.props,
      models: this.models,
    }
    return <InfoShellLayout>
      <FormItem {...props} fieId="UserId" layout="row" value={lodash.get(Store.User.UserInfo, 'Id')} hidden />
      <FormItem {...props} fieId="OldPassword" layout="row" />
      <FormItem {...props} fieId="NewPassword" layout="row" />
      <FormItem {...props} fieId="NewPasswordComfirm" layout="row" />
    </InfoShellLayout>
  }
}