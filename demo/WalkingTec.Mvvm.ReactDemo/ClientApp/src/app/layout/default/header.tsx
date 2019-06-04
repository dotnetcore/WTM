
import { Avatar, Col, Dropdown, Icon, Layout, Menu, Row } from 'antd';
import globalConfig from 'global.config';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/index';
import RequestFiles from 'utils/RequestFiles';
import SetUp from './setUp'
import Sider from './sider'
const SubMenu = Menu.SubMenu;
const MenuItemGroup = Menu.ItemGroup;
const { Header } = Layout;
export default class App extends React.Component<any, any> {
  shouldComponentUpdate() {
    return false
  }
  render() {
    return (
      <PageHeader {...this.props} />
    );
  }
}
@observer
class PageHeader extends React.Component<any, any> {
  render() {
    return (
      <Header className="app-layout-header" style={{ marginLeft: this.props.LayoutStore.collapsedWidth }}>
        <Row type="flex">
          <Col >
            {/* <Test/> */}
            <Icon onClick={() => { this.props.LayoutStore.onCollapsed() }} className="app-collapsed-trigger" type="menu-fold" theme="outlined" />
          </Col>
          <Col style={{ textAlign: "right", flex: 1 }}>
            <Row type="flex" justify="end" style={{ height: "100%" }}>
              <Col style={{ height: "100%", marginRight: 10 }}>
                <SetUp />
              </Col>
              <Col style={{ height: "100%" }}>
                <UserMenu {...this.props} />
              </Col>
            </Row>
          </Col>
        </Row>
      </Header>
    );
  }
}

@observer
class UserMenu extends React.Component<any, any> {
  render() {
    return (
      <Dropdown overlay={
        globalConfig.development ? <Menu>
          <Menu.Item>
            <a href="/_codegen?ui=react" target="_blank">  <Icon type={'appstore'} />代码生成器</a>
          </Menu.Item>
          <Menu.Item>
            <a href="/swagger" target="_blank">  <Icon type={'appstore'} />API文档</a>
          </Menu.Item>
          <Menu.Item>
            <a >  <Icon type={'appstore'} />设置</a>
          </Menu.Item>
          <Menu.Item>
            <a onClick={e => { Store.User.outLogin() }}>  <Icon type={'appstore'} />退出</a>
          </Menu.Item>
        </Menu> : <Menu>
            <Menu.Item>
              <a >  <Icon type={'appstore'} />设置</a>
            </Menu.Item>
            <Menu.Item>
              <a onClick={e => { Store.User.outLogin() }}>  <Icon type={'appstore'} />退出</a>
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

class Test extends React.Component {
  state = {
    current: 'mail',
  };

  handleClick = e => {
    console.log('click ', e);
    this.setState({
      current: e.key,
    });
  };

  render() {
    return (
      <Menu onClick={this.handleClick} selectedKeys={[this.state.current]} mode="horizontal" theme="dark">
        <Menu.Item key="mail">
          <Icon type="mail" />
          Navigation One
          </Menu.Item>
        <Menu.Item key="app" disabled>
          <Icon type="appstore" />
          Navigation Two
          </Menu.Item>
        <SubMenu
          title={
            <span className="submenu-title-wrapper">
              <Icon type="setting" />
              Navigation Three - Submenu
              </span>
          }
        >
          <MenuItemGroup title="Item 1">
            <Menu.Item key="setting:1">Option 1</Menu.Item>
            <Menu.Item key="setting:2">Option 2</Menu.Item>
          </MenuItemGroup>
          <MenuItemGroup title="Item 2">
            <Menu.Item key="setting:3">Option 3</Menu.Item>
            <Menu.Item key="setting:4">Option 4</Menu.Item>
          </MenuItemGroup>
        </SubMenu>
        <Menu.Item key="alipay">
          <a href="https://ant.design" target="_blank" rel="noopener noreferrer">
            Navigation Four - Link
            </a>
        </Menu.Item>
      </Menu>
    );
  }
}