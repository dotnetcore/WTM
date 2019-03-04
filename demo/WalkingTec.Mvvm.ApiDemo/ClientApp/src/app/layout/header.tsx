
import { Avatar, Col, Dropdown, Icon, Layout, Menu, Row } from 'antd';
import GlobalConfig from 'global.config';
import * as React from 'react';
import Store from 'store/index';
import { observer } from 'mobx-react';
import RequestFiles from 'utils/RequestFiles';
const { Header } = Layout;
export default class App extends React.Component<any, any> {
    shouldComponentUpdate() {
        return false;
    }
    render() {
        return (
            <Header tagName="header" className="app-layout-header">
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
@observer
class UserMenu extends React.Component<any, any> {
    render() {
        return (
            <Dropdown overlay={
                <Menu>
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
                </Menu>
            } placement="bottomCenter">
                <div className="app-user-menu" >
                    <div>
                        {/* PhotoId */}
                        <Avatar size="large" icon="user" src={Store.User.User.PhotoId ? RequestFiles.onFileUrl(Store.User.User.PhotoId) : GlobalConfig.default.avatar} />
                        &nbsp;<span>{Store.User.User.Name}</span>
                    </div>
                </div>
            </Dropdown>
        );
    }
}
