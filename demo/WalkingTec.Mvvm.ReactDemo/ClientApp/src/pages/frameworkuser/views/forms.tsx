import { Col, Form } from 'antd';
import { DialogForm, DialogFormDes, DialogFormSubmit, FormItem, InfoShellLayout, DialogLoadData, } from 'components/dataView';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store'; //页面状态
import Models from './models'; //模型
/**
 * 转换表单中的 UserRoles和UserGroups为对象数组
 * @param values 
 */
function toUpdateUserRGs(values) {
    values = lodash.update(values, 'Entity.UserRoles', data => {
        return data && data.map(role => {
            if (lodash.isString(role)) {
                role = {
                    RoleId: role
                }
            }
            return role
        });
    })
    values = lodash.update(values, 'Entity.UserGroups', data => {
        return data && data.map(group => {
            if (lodash.isString(group)) {
                group = {
                    GroupId: group
                }
            }
            return group
        })
    })
    return values
}
@DialogFormDes({
    onFormSubmit(values) {
        values = toUpdateUserRGs(values)
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
        values = toUpdateUserRGs(values)
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
        return <InfoShellLayout>
            <FormItem {...props} fieId="Entity.ID" hidden />
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
                <FormItem {...props} fieId="Entity.UserRoles" layout="row" value={lodash.map(lodash.get(props.defaultValues, 'Entity.UserRoles'), "RoleId")} />
            </Col>
            <Col span={24}>
                <FormItem {...props} fieId="Entity.UserGroups" layout="row" value={lodash.map(lodash.get(props.defaultValues, 'Entity.UserGroups'), "GroupId")} />
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
                <FormItem {...props} fieId="Entity.UserRoles" layout="row" value={lodash.map(lodash.get(props.defaultValues, 'Entity.UserRoles'), "RoleId")} />
            </Col>
            <Col span={24}>
                <FormItem {...props} fieId="Entity.UserGroups" layout="row" value={lodash.map(lodash.get(props.defaultValues, 'Entity.UserGroups'), "GroupId")} />
            </Col>

        </InfoShellLayout>
    }
}
