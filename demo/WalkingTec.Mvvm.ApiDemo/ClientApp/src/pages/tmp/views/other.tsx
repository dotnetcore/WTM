import { Form, Modal, Button, Divider, message } from 'antd';
import { FormItem, ImportModal, InfoShellFooter } from 'components/dataView';
import { DesError, DesForm, DesModel } from 'components/decorators'; //错误
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import myModel from '../store/myModel';
import Models from './models'; //模型


/**
 * 其他弹框类组件
 */
export default class extends React.Component<any, any>{
    shouldComponentUpdate() {
        return false
    }
    render() {
        return <React.Fragment key="page-other">
            <ImportModal Store={Store} />
            <MyModel />
        </React.Fragment>
    }
}

/**
 * 自定义弹框
 */
@observer
class MyModel extends React.Component<any, any> {
    render() {
        return (
            <Modal
                width={1000}
                visible={myModel.visible}
                footer={null}
                onCancel={e => { myModel.visible = false }}
            >
                <InsertForm />
            </Modal>
        );
    }
}
/**
 * 添加表单
 */
@DesError
@DesForm
@observer
class InsertForm extends React.Component<any, any> {
    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            console.log("数据", values);
            if (!err) {
                myModel.onSubmit(values)
            }
        });
    }
    onClick() {
        message.info("点击了按钮")
    }
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
        }
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <InfoShellFooter loadingEdit={false} btns={
                <div className="data-view-form-btns" >
                    <Button type="primary" onClick={e => message.info("点击了按钮")} >按钮1 </Button>
                    <Divider type="vertical" />
                    <Button type="primary" onClick={this.onClick.bind(this)}>按钮2 </Button>
                    <Divider type="vertical" />
                    <Button onClick={() => myModel.visible = false} >取消 </Button>
                    <Divider type="vertical" />
                    <Button type="primary" htmlType="submit"  >提交 </Button>
                </div>
            }>
                <FormItem {...props} fieId="ITCode" />
                <FormItem {...props} fieId="Password" />
                <FormItem {...props} fieId="Email" />
                <FormItem {...props} fieId="Name" />
                <FormItem {...props} fieId="Sex" />
                <FormItem {...props} fieId="UserGroups" />
                <FormItem {...props} fieId="UserRoles" layout="row" />
                <FormItem {...props} fieId="PhotoId" layout="row" />
                <FormItem {...props} fieId="CreateTime" layout="row" />
                <FormItem {...props} fieId="Date2" layout="row" />
            </InfoShellFooter>
        </Form>
    }
}