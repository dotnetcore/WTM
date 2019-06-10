
import { Avatar, Dropdown, Icon, Menu, Input } from 'antd';
import globalConfig from 'global.config';
import { observer } from 'mobx-react';
import * as React from 'react';
import lodash from 'lodash';
import Store from 'store/index';
import RequestFiles from 'utils/RequestFiles';
import Request from 'utils/Request';
import { DialogFormDes, InfoShellLayout, FormItem, DialogForm } from 'components/dataView';
import GlobalConfig from 'global.config';
@observer
export default class UserMenu extends React.Component<any, any> {
  render() {
    return (
      <Dropdown overlay={
        globalConfig.development ? <Menu>
          <Menu.Item>
            <a href="/_codegen?ui=react" target="_blank">  <Icon type={'appstore'} /> 代码生成器</a>
          </Menu.Item>
          <Menu.Item>
            <a href="/swagger" target="_blank">  <Icon type={'appstore'} /> API文档</a>
          </Menu.Item>
          {/* <Menu.Item>
            <a >  <Icon type={'appstore'} />设置</a>
          </Menu.Item> */}
          <Menu.Item>
            {/* <a >  <Icon type={'appstore'} />修改密码</a> */}
            <DialogForm
              title="修改密码"
              icon="appstore"
              type="a"
              width="700"
            >
              <InsertForm />
            </DialogForm>
          </Menu.Item>
          <Menu.Item>
            <a onClick={e => { Store.User.outLogin() }}>  <Icon type={'appstore'} /> 退出</a>
          </Menu.Item>
        </Menu> : <Menu>
            <Menu.Item>
              <a >  <Icon type={'appstore'} /> 设置</a>
            </Menu.Item>
            <Menu.Item>
              <a onClick={e => { Store.User.outLogin() }}>  <Icon type={'appstore'} /> 退出</a>
            </Menu.Item>
          </Menu>

      } placement="bottomCenter">
        <div className="app-user-menu" >
          <div>
            <Avatar size="large" icon="user" src={Store.User.UserInfo.PhotoId ? RequestFiles.onFileUrl(Store.User.UserInfo.PhotoId) : globalConfig.default.avatar} />
            &nbsp;<span>{Store.User.UserInfo.Name}</span>
          </div>
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