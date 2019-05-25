
import { Drawer, Form, Icon, Radio, Select, Switch } from 'antd';
import RadioGroup from 'antd/lib/radio/group';
import { DesForm } from 'components/decorators';
import globalConfig from 'global.config';
import { BindAll } from 'lodash-decorators';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
@observer
@DesForm
@BindAll()
export default class SetUp extends React.Component<any, any> {
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
                sm: { span: 6 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 18 },
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
                                valuePropName: 'checked',
                                initialValue: globalConfig.tabsPage
                            })(
                                <Switch onChange={event => { runInAction(() => globalConfig.tabsPage = event) }} checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
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
                                    runInAction(() => globalConfig.tabPosition = event);
                                    dispatchEvent(new CustomEvent('resize'));
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
