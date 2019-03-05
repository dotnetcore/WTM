import { Button, Divider } from 'antd';
import { DrawerProps } from 'antd/lib/drawer';
import Form from 'antd/lib/form/Form';
import { ModalProps } from 'antd/lib/modal';
import lodash from 'lodash';
import * as React from 'react';
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
    /**
     * 表单提交 方法     */
    onFormSubmit?: (err, values, onClose?) => Promise<boolean> | void | boolean;
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
        this.props.form.validateFields(async (err, values) => {
            const callbackValue = this.props.onFormSubmit(err, values, this.onVisible.bind(this, false));
            if (!err) {
                let res = false;
                // 提交数据
                this.setState({ loading: true });
                if (lodash.isBoolean(callbackValue)) {
                    res = callbackValue;
                } else if (callbackValue) {
                    res = await callbackValue;
                }
                console.log("返回值", res, callbackValue)
                if (res) {
                    this.onVisible(false)
                }
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
        }
        return (
            <>
                <Button
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
