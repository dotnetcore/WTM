import { DialogFormDes, FormItem, InfoShellLayout } from 'components/dataView';
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
            <FormItem {...props} fieId="Entity.ModuleName" />
            <FormItem {...props} fieId="Entity.ActionName" />
            <FormItem {...props} fieId="Entity.ITCode" />
            <FormItem {...props} fieId="Entity.ActionUrl" />
            <FormItem {...props} fieId="Entity.ActionTime" />
            <FormItem {...props} fieId="Entity.Duration" />
            <FormItem {...props} fieId="Entity.Remark" />
            <FormItem {...props} fieId="Entity.IP" />
            <FormItem {...props} fieId="Entity.LogType" />

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
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
        }
        // 声明 ID
        getFieldDecorator('Entity.ID', { initialValue: lodash.get(this.props.defaultValues, 'Entity.ID') })
        return <InfoShellLayout>
            <FormItem {...props} fieId="Entity.ModuleName" />
            <FormItem {...props} fieId="Entity.ActionName" />
            <FormItem {...props} fieId="Entity.ITCode" />
            <FormItem {...props} fieId="Entity.ActionUrl" />
            <FormItem {...props} fieId="Entity.ActionTime" />
            <FormItem {...props} fieId="Entity.Duration" />
            <FormItem {...props} fieId="Entity.Remark" />
            <FormItem {...props} fieId="Entity.IP" />
            <FormItem {...props} fieId="Entity.LogType" />

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
            <FormItem {...props} fieId="Entity.ModuleName" />
            <FormItem {...props} fieId="Entity.ActionName" />
            <FormItem {...props} fieId="Entity.ITCode" />
            <FormItem {...props} fieId="Entity.ActionUrl" />
            <FormItem {...props} fieId="Entity.ActionTime" />
            <FormItem {...props} fieId="Entity.Duration" />
            <FormItem {...props} fieId="Entity.Remark" />
            <FormItem {...props} fieId="Entity.IP" />
            <FormItem {...props} fieId="Entity.LogType" />

        </InfoShellLayout>
    }
}
