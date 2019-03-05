import { Button } from 'antd';
import { ButtonProps } from 'antd/lib/button';
import { DrawerProps } from 'antd/lib/drawer';
import { ModalProps } from 'antd/lib/modal';
import * as React from 'react';
import { DesError, DesForm } from '../../../decorators'; //错误
import { InfoShell, InfoShellFooter } from '../infoShell';
import Form, { WrappedFormUtils } from 'antd/lib/form/Form';
import lodash from 'lodash';
declare type Props = {
    form?: WrappedFormUtils;
    InfoShell?: DrawerProps | ModalProps;
    title?: React.ReactNode;
    visible?: boolean;
    /**
     *  提交 事件， 返回 true promise 关闭窗口
     */
    onFormSubmit: (err, values, callBack) => void
} & ButtonProps;
@DesError
@DesForm
export class DialogForm extends React.Component<Props, any> {
    state = {
        visible: false
    }
    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        this.props.form.validateFields(async (err, values) => {
            this.props.onFormSubmit(err, values, () => {
                this.setState({ visible: false })
            });
        });
    }
    render() {
        const title = lodash.get(this.props, 'title', '未设置标题')
        return (
            <>
                <Button onClick={e => {
                    this.setState({ visible: true })
                }}>{title}</Button>
                <InfoShell
                    title={title}
                    visible={lodash.get(this.props, 'visible', this.state.visible)}
                    onCancel={e => this.setState({ visible: false })}
                    {...this.props.InfoShell}
                >
                    <Form onSubmit={this.onSubmit.bind(this)}>
                        {React.cloneElement(this.props.children as any, { form: this.props.form })}
                        <InfoShellFooter
                            onCancel={() => this.setState({ visible: false })}
                            submit />
                    </Form>
                </InfoShell>
            </>
        );
    }
}
