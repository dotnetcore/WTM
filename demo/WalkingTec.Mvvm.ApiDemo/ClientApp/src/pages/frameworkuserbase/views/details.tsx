import { Col, Form } from 'antd';
import { FormItem, InfoShell, InfoShellFooter, ToImg } from 'components/dataView';
import { DesError, DesForm } from 'components/decorators';
import { toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import Models from './models';
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
        const { detailsType, visibleEdit, loadingEdit } = Store.pageState
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
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            models: this.models,
        }
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <FooterFormItem submit>
                <FormItem {...props} fieId="ITCode" />
                <FormItem {...props} fieId="Password" />
                <FormItem {...props} fieId="Email" />
                <FormItem {...props} fieId="Name" />
                <FormItem {...props} fieId="Sex" />
                <FormItem {...props} fieId="CellPhone" />
                <FormItem {...props} fieId="HomePhone" />
                <FormItem {...props} fieId="Address" />
                <FormItem {...props} fieId="ZipCode" />
                <FormItem {...props} fieId="PhotoId" />
                <FormItem {...props} fieId="IsValid" />
                <FormItem {...props} fieId="UserRoles" layout="row" />
                <FormItem {...props} fieId="UserGroups" layout="row" />
            </FooterFormItem>

        </Form>
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
            models: this.models,
            defaultValues: toJS(Store.details),
        }
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <FooterFormItem submit>
                <FormItem {...props} fieId="ITCode" />
                <FormItem {...props} fieId="Password" />
                <FormItem {...props} fieId="Email" />
                <FormItem {...props} fieId="Name" />
                <FormItem {...props} fieId="Sex" />
                <FormItem {...props} fieId="CellPhone" />
                <FormItem {...props} fieId="HomePhone" />
                <FormItem {...props} fieId="Address" />
                <FormItem {...props} fieId="ZipCode" />
                <FormItem {...props} fieId="PhotoId" />
                <FormItem {...props} fieId="IsValid" />
                <FormItem {...props} fieId="UserRoles" layout="row" />
                <FormItem {...props} fieId="UserGroups" layout="row" />
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
            models: this.models,
            display: true,
            defaultValues: toJS(Store.details),
        }
        return <Form >
            <FooterFormItem>
                <FormItem {...props} fieId="ITCode" />
                <FormItem {...props} fieId="Password" />
                <FormItem {...props} fieId="Email" />
                <FormItem {...props} fieId="Name" />
                <FormItem {...props} fieId="Sex" />
                <FormItem {...props} fieId="CellPhone" />
                <FormItem {...props} fieId="HomePhone" />
                <FormItem {...props} fieId="Address" />
                <FormItem {...props} fieId="ZipCode" />
                <FormItem {...props} fieId="PhotoId" />
                <FormItem {...props} fieId="IsValid" />
                <FormItem {...props} fieId="UserRoles" layout="row" />
                <FormItem {...props} fieId="UserGroups" layout="row" />
            </FooterFormItem>
        </Form>
    }
}
/**
 * Items 外壳
 */
@DesError
@observer
class FooterFormItem extends React.Component<{ submit?: boolean }, any> {
    render() {
        const { loadingEdit } = Store.pageState;
        return <InfoShellFooter
            submit={this.props.submit}
            loadingEdit={loadingEdit}
            onCancel={() => Store.onPageState("visibleEdit", false)}>
            {this.props.children}
        </InfoShellFooter>
    }
}