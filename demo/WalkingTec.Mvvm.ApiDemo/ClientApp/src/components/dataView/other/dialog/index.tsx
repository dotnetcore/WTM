import { Button, Col, Divider, List, notification, Row, Alert } from 'antd';
import { DrawerProps } from 'antd/lib/drawer';
import Form, { WrappedFormUtils } from 'antd/lib/form/Form';
import { ModalProps } from 'antd/lib/modal';
import lodash from 'lodash';
import * as React from 'react';
import RequestFiles from 'utils/RequestFiles';
import { DesError, DesForm } from '../../../decorators'; //错误
import { InfoShell } from '../infoShell';
declare type Props = {
    form?: WrappedFormUtils;
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
     * 按钮类型
     */
    type?: 'button' | 'a';
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
        warning: false,
        loading: false
    }

    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        const onFormSubmit = this.props.onFormSubmit || lodash.get(this.props.children, 'type.onFormSubmit');
        if (lodash.isFunction(onFormSubmit)) {
            // debugger
            // 加载中 返回。
            if (this.state.loading) return;
            try {
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
                                this.onSetErrorMsg(error || {})
                                // onErrorMessage("操作失败", lodash.map(error, (value, key) => ({ value, key })))

                            }
                        });
                    } else {
                        console.log(err)
                        this.setState({ loading: false })
                    }
                });
            } catch (error) {
                console.log(error)
            }
        } else {
            this.setState({ warning: true })
        }
    }
    onSetErrorMsg(errors = { Entity: { "Name": "姓名是必填项" } }) {
        const { setFields } = this.props.form;
        setFields(lodash.mapValues(errors.Entity, data => {
            return {
                errors: [new Error(data)]
            }
        }))
        // [fieldName]: { value: any, errors: [Error] }
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

        const option = {
            title: lodash.get(this.props, 'title', '未设置标题'),
            disabled: lodash.get(this.props, "disabled", false),
            showSubmit: lodash.get(this.props, 'showSubmit', true),
            closeText: lodash.get(this.props, 'closeText', '关闭'),
            submitText: lodash.get(this.props, 'submitText', '提交'),
            icon: lodash.get(this.props, 'icon'),
            type: lodash.get(this.props, 'type', 'button'),
        }
        const children = option.disabled ? <div></div> : React.cloneElement(this.props.children as any, { form: this.props.form }, null);
        const { warning } = this.state;
        const button = option.type === "button" ? <Button
            icon={option.icon}
            disabled={option.disabled}
            onClick={this.onVisible.bind(this, true)}>{option.title}</Button> :
            <a onClick={this.onVisible.bind(this, true)} >{option.title}</a>
        return (
            <>
                {button}
                <InfoShell
                    title={option.title}
                    visible={this.state.visible}
                    onCancel={this.onVisible.bind(this, false)}
                    {...this.props.InfoShell}
                >
                    <Form onSubmit={this.onSubmit.bind(this)}>
                        {warning && <>
                            <Alert message="请指定 onFormSubmit 函数 或者 Form 静态 方法" type="warning" showIcon />
                            <Divider />
                        </>}
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