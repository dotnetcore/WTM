import { Button, Col, Divider, List, notification, Row } from 'antd';
import { DrawerProps } from 'antd/lib/drawer';
import Form from 'antd/lib/form/Form';
import { ModalProps } from 'antd/lib/modal';
import lodash from 'lodash';
import * as React from 'react';
import RequestFiles from 'utils/RequestFiles';
import { DesError, DesForm } from '../../../decorators'; //错误
import { InfoShell } from '../infoShell';
declare type Props = {
    // form?: WrappedFormUtils;
    InfoShell?: DrawerProps | ModalProps;
    /** 标题 */
    title?: React.ReactNode;
    /** 按钮可用状态 */
    disabled?: boolean;
    /** 显示 提交按钮 */
    showSubmit?: boolean;
    /** 关闭按钮 文案 */
    closeText?: string;
    /** 提交按钮 文案 */
    submitText?: string;
    /** 图标 */
    icon?: string;
    /** 加载 */
    loading?: boolean;
    /**
     * 表单提交 方法     */
    onFormSubmit?: (values, onClose?) => Promise<boolean> | void | boolean;
    [key: string]: any
};
@DesError
@DesForm
export class DialogForm extends React.Component<Props, any> {

    state = {
        visible: false,
        loading: false
    }

    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        // 加载中 返回。
        if (this.state.loading) return;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.setState({ loading: true }, async () => {
                    try {
                        const callbackValue = this.props.onFormSubmit(values, this.onVisible.bind(this, false));
                        let res = false;
                        if (lodash.isBoolean(callbackValue)) {
                            res = callbackValue;
                        } else if (callbackValue) {
                            res = await callbackValue
                        }
                        if (res) {
                            this.onVisible(false)
                        }
                    } catch (error) {
                        this.setState({ loading: false })
                        onErrorMessage("操作失败", lodash.map(error, (value, key) => ({ value, key })))
                    }
                });
            } else {
                this.setState({ loading: false })
            }

        });

    }
    onVisible(visible = !this.state.visible) {
        this.setState(state => {
            if (state.loading) {
                state.loading = false;
            }
            state.visible = visible;
            return state
        })
    }
    render() {
        const children = React.cloneElement(this.props.children as any, { form: this.props.form }, null);
        const option = {
            title: lodash.get(this.props, 'title', '未设置标题'),
            disabled: lodash.get(this.props, "disabled", false),
            showSubmit: lodash.get(this.props, 'showSubmit', true),
            closeText: lodash.get(this.props, 'closeText', '关闭'),
            submitText: lodash.get(this.props, 'submitText', '提交'),
            icon: lodash.get(this.props, 'icon')
        }
        return (
            <>
                <Button
                    icon={option.icon}
                    disabled={option.disabled}
                    onClick={this.onVisible.bind(this, true)}>{option.title}</Button>
                <InfoShell
                    title={option.title}
                    visible={this.state.visible}
                    onCancel={this.onVisible.bind(this, false)}
                    {...this.props.InfoShell}
                >
                    <Form onSubmit={this.onSubmit.bind(this)}>
                        {children}
                        <div className="data-view-form-btns" >
                            <Button onClick={this.onVisible.bind(this, false)} > {option.closeText} </Button>
                            {option.showSubmit && <>
                                <Divider type="vertical" />
                                <Button loading={this.state.loading} type="primary" htmlType="submit"  >{option.submitText} </Button>
                            </>}
                        </div>
                    </Form>
                </InfoShell>
            </>
        );
    }
}
/**
   * 错误提示
   * @param message 
   * @param dataSource 
   */
function onErrorMessage(message, dataSource?: { key: string, value: string, FileId?: string }[]) {
    notification.error({
        duration: 5,
        message: message,
        description: dataSource && dataSource.length > 0 && <List
            itemLayout="horizontal"
            dataSource={dataSource}
            renderItem={item => (
                <List.Item>
                    <Row style={{ width: "100%" }}>
                        {/* <Col span={10}>{item.key}</Col> */}
                        <Col span={14}>{item.value}</Col>
                        {item.FileId && <Col span={10}>
                            <Button type="primary" onClick={e => {
                                RequestFiles.download({ url: RequestFiles.onFileDownload(item.FileId, "/"), method: "get" })
                            }}>下载错误文件</Button>
                        </Col>}
                    </Row>
                </List.Item>
            )}
        />
    })
}