import { Button, Col, Divider, Form, message, Modal, Row } from 'antd';
import { DialogForm, FormItem, InfoShell, InfoShellFooter, InfoShellLayout } from 'components/dataView';
import { DesError, DesForm, DialogModel } from 'components/decorators'; //错误
import lodash from 'lodash';
import { toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store'; //页面状态
import myModel from '../store/myModel'; //页面状态
import Models from './models'; //模型

/**
 *  详情 窗口 
 *  根据 类型 显示不同的 窗口
 */
@DesError
@observer
export default class extends React.Component<any, any> {
    /**
     * 根据状态类型 渲染  添加。修改，详情信息
     * @param detailsType 
     */
    renderBody(detailsType) {
        switch (detailsType) {
            case 'Insert':
                //添加
                return <InsertForm {...this.props} />
                break;
            case 'Update':
                // 修改
                return <UpdateForm {...this.props} />
                break;
            default:
                // 详情
                return <InfoForm {...this.props} />
                break;
        }
    }

    render() {
        const enums = {
            Insert: "新建",
            Update: "修改",
            Info: "详情"
        };
        const { detailsType, visibleEdit, loadingEdit } = Store.pageState;
        return <InfoShell
            title={enums[detailsType]}
            onClose={() => { Store.onPageState("visibleEdit", false) }}
            visible={visibleEdit}
        >

            {this.renderBody(detailsType)}
        </InfoShell>
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
                Store.onEdit(values);
            }
        });
    }
    /**
    * 弹个框
    */
    myModelTwo = DialogModel({ title: "弹个框" }, InsertForm);
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
        }
        console.log(props.models)
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <Row>
                <Button onClick={e => { myModel.visible = true }}>弹个框</Button>
                <Divider type="vertical" />
                <Button onClick={e => {
                    Modal.confirm({
                        width: 800,
                        content: <InsertForm />
                    })
                }}>弹个框</Button>
                <Divider type="vertical" />
                <DialogForm onFormSubmit={(err, values) => {
                    // 没有错误 提交 数据
                    if (!err) {
                        // return Store.Request.post("", values).toPromise()
                        return new Promise((resolve, reject) => {
                            // 2秒后 返回 treu 关闭窗口
                            lodash.delay(resolve, 2000, true)
                        })
                    }
                }}>>
                    <TestForm />
                </DialogForm>
            </Row>
            <Divider />
            <FooterFormItem submit>
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

            </FooterFormItem>

        </Form>
    }
}
/**
 * 测试
 */
@DesError
@observer
export class TestForm extends React.Component<any, any> {
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
        }
        return (
            <InfoShellLayout>
                <Col span={24}>
                    <DialogForm onFormSubmit={(err, values) => {
                        // 没有错误 提交 数据
                        if (!err) {
                            message.success("模拟提交数据 2 秒 后成功")
                            // return Store.Request.post("", values).toPromise()
                            return new Promise((resolve, reject) => {
                                // 2秒后 返回 treu 关闭窗口
                                lodash.delay(resolve, 2000, true)
                            })
                        } else {
                            message.error("数据不完整")
                        }
                        return false
                    }}>
                        <TestForm />
                    </DialogForm>
                </Col>
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
            </InfoShellLayout>
        )
    }
}
/**
 * 修改表单
 */
@DesError
@DesForm
@observer
class UpdateForm extends React.Component<any, any> {
    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            console.log("数据", values);
            if (!err) {
                // values = mapValues(values, "YYYY-MM-DD")
                Store.onEdit(values);
            }
        });
    }
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
            // 默认值
            defaultValues: toJS(Store.details),
        }
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <FooterFormItem submit>
                <FormItem {...props} fieId="ITCode" />
                <FormItem {...props} fieId="Password" disabled />
                <FormItem {...props} fieId="Email" />
                <FormItem {...props} fieId="Name" />
                <FormItem {...props} fieId="Sex" />
                <FormItem {...props} fieId="UserGroups" />
                <FormItem {...props} fieId="UserRoles" layout="row" disabled />
                <FormItem {...props} fieId="PhotoId" layout="row" disabled />
                <FormItem {...props} fieId="CreateTime" layout="row" />
            </FooterFormItem>
        </Form>
    }
}
/**
 * 详情
 */
@DesError
@observer
class InfoForm extends React.Component<any, any> {
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
            // 禁用
            display: true,
            // 默认值
            defaultValues: toJS(Store.details),
        }
        return <Form >
            <FooterFormItem>
                <FormItem {...props} fieId="ITCode" />
                <FormItem {...props} fieId="Password" />
                <FormItem {...props} fieId="Email" />
                <FormItem {...props} fieId="Name" />
                <FormItem {...props} fieId="Sex" />
                <FormItem {...props} fieId="UserGroups" />
                <FormItem {...props} fieId="UserRoles" layout="row" />
                <FormItem {...props} fieId="PhotoId" layout="row" />
            </FooterFormItem>
        </Form>
    }
}
/**
 * Items 外壳
 */
@DesError
@observer
class FooterFormItem extends React.Component<{ submit?: boolean, onCancel?: () => void }, any> {
    onCancel() {
        if (typeof this.props.onCancel === "function") {
            this.props.onCancel()
        } else {
            Store.onPageState("visibleEdit", false)
        }
    }
    render() {
        const { loadingEdit } = Store.pageState;
        return <InfoShellFooter
            submit={this.props.submit}
            loadingEdit={loadingEdit}
            onCancel={this.onCancel.bind(this)}>
            {this.props.children}
        </InfoShellFooter>
    }
}
