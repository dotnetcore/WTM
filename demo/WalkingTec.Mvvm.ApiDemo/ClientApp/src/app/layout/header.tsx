
import { Avatar, Col, Dropdown, Icon, Layout, Menu, Row } from 'antd';
import GlobalConfig from 'global.config';
import * as React from 'react';
import Store from 'store/index';
const { Header } = Layout;
export default class App extends React.Component<any, any> {
    shouldComponentUpdate() {
        return false;
    }
    render() {
        return (
            <Header className="app-layout-header">
                <Row>
                    <Col span={4}><Icon onClick={() => { Store.Meun.toggleCollapsed() }} className="app-collapsed-trigger" type="menu-fold" theme="outlined" /></Col>
                    <Col span={20} style={{ textAlign: "right" }}>
                        <UserMenu {...this.props} />
                    </Col>
                </Row>
            </Header>
        );
    }
}
class UserMenu extends React.Component<any, any> {
    render() {
        return (
            <Dropdown overlay={
                <Menu>
                     <Menu.Item>
                        <a href="/_codegen?ui=react" target="_blank">  <Icon type={'appstore'} />代码生成器</a>
                    </Menu.Item>
                    <Menu.Item>
                        <a >  <Icon type={'appstore'} />设置</a>
                    </Menu.Item>
                    <Menu.Item>
                        <a >  <Icon type={'appstore'} />退出</a>
                    </Menu.Item>
                </Menu>
            } placement="bottomCenter">
                <div className="app-user-menu" >
                    <div>
                        <Avatar size="large" icon="user" src={GlobalConfig.default.avatar} />
                        &nbsp;<span>UserName</span>
                    </div>
                </div>
            </Dropdown>
        );
    }
}
