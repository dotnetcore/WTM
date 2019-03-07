import { Col } from 'antd';
import { DialogForm, DialogFormDes, DialogFormSubmit, FormItem, InfoShellLayout, DialogLoadData, } from 'components/dataView';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store'; //页面状态
import Models from './models'; //模型

@DialogFormDes({
    onFormSubmit(values) {
        return Store.onInsert(values)
    }
})
@observer
export class InsertForm extends React.Component<any, any> {
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
        }
        return <InfoShellLayout>
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
                <Col span={24}>
                    <FormItem {...props} fieId="UserRoles" layout="row" />
                </Col>
                <Col span={24}>
                    <FormItem {...props} fieId="UserGroups" layout="row" />
                </Col>

            </InfoShellLayout>        
    }
}
/**
 * 修改表单
 */
@DialogFormDes({
    onFormSubmit(values) {
        return Store.onUpdate(values)
    },
    onLoadData(values, props) {
        return Store.onDetails(values)
    }
})
@observer
export class UpdateForm extends React.Component<{ loadData: Function | Object }, any> {
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
        }
        return <InfoShellLayout>
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
                <Col span={24}>
                    <FormItem {...props} fieId="UserRoles" layout="row" />
                </Col>
                <Col span={24}>
                    <FormItem {...props} fieId="UserGroups" layout="row" />
                </Col>

        </InfoShellLayout>
    }
}
/**
 * 详情
 */
@DialogFormDes({
    onLoadData(values, props) {
        return Store.onDetails(values)
    }
})
@observer
export class InfoForm extends React.Component<{ loadData: Function | Object }, any> {
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
        }
        return <InfoShellLayout >
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
                <Col span={24}>
                    <FormItem {...props} fieId="UserRoles" layout="row" />
                </Col>
                <Col span={24}>
                    <FormItem {...props} fieId="UserGroups" layout="row" />
                </Col>

        </InfoShellLayout>
    }
}
