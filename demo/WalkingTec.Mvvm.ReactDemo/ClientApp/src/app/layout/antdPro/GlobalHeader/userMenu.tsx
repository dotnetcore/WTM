
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
import { FormattedMessage } from 'react-intl';
import { getLocalesTemplate, getLocalesValue } from 'locale';
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
            <a href="/_codegen?ui=react" target="_blank">  <Icon type='code' /> <FormattedMessage id='action.user.codeGenerator' /></a>
          </Menu.Item>}
          {globalConfig.development && <Menu.Item>
            <a href="/swagger" target="_blank">  <Icon type='bars' /> <FormattedMessage id='action.user.apiDocument' /></a>
          </Menu.Item>}
          <Menu.Item>
            <DialogForm
              title={<FormattedMessage id='action.user.changePassword' />}
              icon="user"
              type="a"
            >
              <InsertForm />
            </DialogForm>
          </Menu.Item>
          <Menu.Item>
            <a onClick={e => { Store.User.outLogin() }}>  <Icon type='logout' /> <FormattedMessage id='action.user.logout' /></a>
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
      label: <FormattedMessage id='update.pwd.old' />,
      rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('update.pwd.old') }} /> }],
      formItem: <Input.Password placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('update.pwd.old') })} />
    },
    "NewPassword": {
      label: <FormattedMessage id='update.pwd.new' />,
      rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('update.pwd.new') }} /> }],
      formItem: <Input.Password placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('update.pwd.new') })} />
    },
    "NewPasswordComfirm": {
      label: <FormattedMessage id='update.pwd.confirm' />,
      rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('update.pwd.confirm') }} /> }, {
        validator: (rule, value, callback) => {
          const form = this.props.form;
          if (value && value !== form.getFieldValue('NewPassword')) {
            callback(getLocalesValue('update.pwd.inconsistent'));
          } else {
            callback();
          }
        }
      }],
      formItem: <Input.Password placeholder={getLocalesValue('update.pwd.confirm')} />
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
