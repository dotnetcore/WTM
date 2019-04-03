
import { Avatar, Col, Drawer, Dropdown, Form, Icon, Layout, Menu, Radio, Row, Select } from 'antd';
import RadioGroup from 'antd/lib/radio/group';
import { DesForm } from 'components/decorators';
import globalConfig from 'global.config';
import { BindAll } from 'lodash-decorators';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/index';
import RequestFiles from 'utils/RequestFiles';
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
                <Row>
                    <Col span={4}>
                        <Icon onClick={() => { this.props.LayoutStore.onCollapsed() }} className="app-collapsed-trigger" type="menu-fold" theme="outlined" />
                    </Col>
                    <Col span={20} style={{ textAlign: "right" }}>
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
                            {getFieldDecorator('infoType', {
                                rules: [],
                                initialValue: globalConfig.infoType
                            })(
                                <RadioGroup onChange={event => { runInAction(() => globalConfig.infoType = event.target.value) }}>
                                    <Radio value='Modal'>Modal</Radio>
                                    <Radio value='Drawer'>Drawer</Radio>
                                </RadioGroup>
                            )}
                        </Form.Item>
                        <Form.Item
                            label="Tabs页签"
                        >
                            {getFieldDecorator('tabsPage', {
                                rules: [],
                                initialValue: globalConfig.tabsPage
                            })(
                                <RadioGroup onChange={event => { runInAction(() => globalConfig.tabsPage = event.target.value) }}>
                                    <Radio value={true}>开启</Radio>
                                    <Radio value={false}>关闭</Radio>
                                </RadioGroup>
                            )}
                        </Form.Item>
                        <Form.Item
                            label="Tabs页签位置"
                        >
                            {getFieldDecorator('tabPosition', {
                                rules: [],
                                initialValue: globalConfig.tabPosition
                                // top right bottom left
                            })(
                                <Select style={{ width: '100%' }} onChange={(event: any) => {
                                    dispatchEvent(new CustomEvent('resize'));
                                    runInAction(() => globalConfig.tabPosition = event)
                                }}>
                                    <Select.Option value="top">top</Select.Option>
                                    <Select.Option value="right">right</Select.Option>
                                    <Select.Option value="bottom">bottom</Select.Option>
                                    <Select.Option value="left">left</Select.Option>
                                </Select>
                            )}
                        </Form.Item>
                    </Form>
                </Drawer>
            </>
        );
    }
}
