import { Col, Form } from 'antd';
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
        const props = {
            ...this.props,
            models: this.models,
        }
        return <InfoShellLayout>
                <FormItem {...props} fieId="Entity.ITCode" />
                <FormItem {...props} fieId="Entity.Password" />
                <FormItem {...props} fieId="Entity.Email" />
                <FormItem {...props} fieId="Entity.Name" />
                <FormItem {...props} fieId="Entity.Sex" />
                <FormItem {...props} fieId="Entity.CellPhone" />
                <FormItem {...props} fieId="Entity.HomePhone" />
                <FormItem {...props} fieId="Entity.Address" />
                <FormItem {...props} fieId="Entity.ZipCode" />
                <FormItem {...props} fieId="Entity.PhotoId" />
                <FormItem {...props} fieId="Entity.IsValid" />
                <Col span={24}>
                    <FormItem {...props} fieId="Entity.UserRoles" layout="row" />
                </Col>
                <Col span={24}>
                    <FormItem {...props} fieId="Entity.UserGroups" layout="row" />
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
export class UpdateForm extends React.Component<WTM.FormProps, any> {
    // 创建模型
    models = Models.editModels(this.props);
    render() {
        const { form } = this.props;
        const { getFieldDecorator } = form;
        const props = {
            ...this.props,
            models: this.models,
        }
        getFieldDecorator('Entity.ID', { initialValue: lodash.get(this.props.defaultValues, 'Entity.ID') })
        getFieldDecorator('Entity.Password', { initialValue: lodash.get(this.props.defaultValues, 'Entity.Password') });
       return <InfoShellLayout>
                <FormItem {...props} fieId="Entity.ITCode"  />
                <FormItem {...props} fieId="Entity.Email" />
                <FormItem {...props} fieId="Entity.Name" />
                <FormItem {...props} fieId="Entity.Sex" />
                <FormItem {...props} fieId="Entity.CellPhone" />
                <FormItem {...props} fieId="Entity.HomePhone" />
                <FormItem {...props} fieId="Entity.Address" />
                <FormItem {...props} fieId="Entity.ZipCode" />
                <FormItem {...props} fieId="Entity.PhotoId" />
                <FormItem {...props} fieId="Entity.IsValid" />
                <Col span={24}>
                    <FormItem {...props} fieId="Entity.UserRoles" layout="row" />
                </Col>
                <Col span={24}>
                    <FormItem {...props} fieId="Entity.UserGroups" layout="row" />
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
export class InfoForm extends React.Component<WTM.FormProps, any> {
    models = Models.editModels(this.props);
    render() {
        const props = {
            ...this.props,
            models: this.models,
            display: true,
        }
        return <InfoShellLayout >
                <FormItem {...props} fieId="Entity.ITCode" />
                <FormItem {...props} fieId="Entity.Email" />
                <FormItem {...props} fieId="Entity.Name" />
                <FormItem {...props} fieId="Entity.Sex" />
                <FormItem {...props} fieId="Entity.CellPhone" />
                <FormItem {...props} fieId="Entity.HomePhone" />
                <FormItem {...props} fieId="Entity.Address" />
                <FormItem {...props} fieId="Entity.ZipCode" />
                <FormItem {...props} fieId="Entity.PhotoId" />
                <FormItem {...props} fieId="Entity.IsValid" />
                <Col span={24}>
                    <FormItem {...props} fieId="Entity.UserRoles" layout="row" />
                </Col>
                <Col span={24}>
                    <FormItem {...props} fieId="Entity.UserGroups" layout="row" />
                </Col>

        </InfoShellLayout>
    }
}
