
import { Avatar, Col, Drawer, Dropdown, Icon, Layout, Menu, Row, Form, Radio } from 'antd';
import globalConfig from 'global.config';
import { BindAll } from 'lodash-decorators';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/index';
import RequestFiles from 'utils/RequestFiles';
import { DesForm } from 'components/decorators';
import RadioGroup from 'antd/lib/radio/group';
import { runInAction } from 'mobx';
const { Header } = Layout;
@observer
export default class App extends React.Component<any, any> {
    shouldComponentUpdate() {
        return false
    }
    render() {
        return (
            <Header className="app-layout-header" style={{ marginLeft: this.props.LayoutStore.collapsedWidth }}>
                <Row>
                    <Col span={4}><Icon onClick={() => { this.props.LayoutStore.onCollapsed() }} className="app-collapsed-trigger" type="menu-fold" theme="outlined" /></Col>
                    <Col span={20} style={{ textAlign: "right" }}>
                        <Row type="flex" justify="end" style={{ height: "100%" }}>
                            <Col style={{ height: "100%" }}>
                                <SetUp />
                            </Col>
                            <Col offset={1}>
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
                        <Avatar size="large" icon="user" src={Store.User.User.PhotoId ? RequestFiles.onFileUrl(Store.User.User.PhotoId) : globalConfig.default.avatar} />
                        &nbsp;<span>{Store.User.User.Name}</span>
                    </div>
                </div>
            </Dropdown>
        );
    }
}
@observer
@DesForm
@BindAll()
class SetUp extends React.Component<any, any> {
    state = {
        visible: false
    }
    handleSubmit() {

    }
    onClose() {
        this.setState({ visible: false })
    }
    render() {
        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 4 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 20 },
            },
        };
        const { getFieldDecorator } = this.props.form;
        return (
            <>
                <Icon onClick={() => { this.setState({ visible: true }) }} className="app-collapsed-trigger" type="setting" theme="outlined" />
                <Drawer
                    title="Global Config"
                    placement="right"
                    width={500}
                    closable={false}
                    onClose={this.onClose}
                    visible={this.state.visible}
                >
                    <Form {...formItemLayout} onSubmit={this.handleSubmit}>
                        <Form.Item
                            label="弹框类型"
                        >
                            {getFieldDecorator('email', {
                                rules: [],
                                initialValue: globalConfig.infoType
                            })(
                                <RadioGroup onChange={event => { runInAction(() => globalConfig.infoType = event.target.value) }}>
                                    <Radio value='Modal'>Modal</Radio>
                                    <Radio value='Drawer'>Drawer</Radio>
                                </RadioGroup>
                            )}</Form.Item>
                    </Form>
                </Drawer>
            </>
        );
    }
}
