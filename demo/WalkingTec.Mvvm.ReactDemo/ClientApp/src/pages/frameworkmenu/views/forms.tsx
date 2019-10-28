import { DialogFormDes, FormItem, InfoShellLayout } from 'components/dataView';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store'; //页面状态
import Models from './models'; //模型
import { Help } from 'utils/Help';
import { Icon } from 'antd';

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
        const IsInside = Help.FormValueEqual(props, 'Entity.IsInside', true, true);
        return <InfoShellLayout>
            <FormItem {...props} fieId="Entity.IsInside" layout="row" value={true} />
            <FormItem {...props} fieId="Entity.Url" layout="row" hidden={IsInside} />
            <FormItem {...props} fieId="SelectedModule" hidden={!IsInside} />
            <FormItem {...props} fieId="SelectedActionIDs" hidden={!IsInside} />
            <FormItem {...props} fieId="Entity.PageName" />
            <FormItem {...props} fieId="Entity.ParentId" />
            <FormItem {...props} fieId="Entity.FolderOnly" />
            <FormItem {...props} fieId="Entity.ShowOnMenu" value={true} />
            <FormItem {...props} fieId="Entity.IsPublic" />
            <FormItem {...props} fieId="Entity.DisplayOrder" />
            <FormItem {...props} fieId="Entity.ICon" />
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
        const IsInside = Help.FormValueEqual(props, 'Entity.IsInside', true, true);
        return <InfoShellLayout>
            <FormItem {...props} fieId="Entity.ID" hidden />
            <FormItem {...props} fieId="Entity.IsInside" layout="row" />
            <FormItem {...props} fieId="Entity.Url" layout="row" hidden={IsInside} />
            <FormItem {...props} fieId="SelectedModule" hidden={!IsInside} />
            <FormItem {...props} fieId="SelectedActionIDs" hidden={!IsInside} />
            <FormItem {...props} fieId="Entity.PageName" />
            <FormItem {...props} fieId="Entity.ParentId" />
            <FormItem {...props} fieId="Entity.FolderOnly" />
            <FormItem {...props} fieId="Entity.ShowOnMenu" />
            <FormItem {...props} fieId="Entity.IsPublic" />
            <FormItem {...props} fieId="Entity.DisplayOrder" />
            <FormItem {...props} fieId="Entity.ICon" />
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
        const IsInside = Help.FormValueEqual(props, 'Entity.IsInside', true, true);
        return <InfoShellLayout>
            <FormItem {...props} fieId="Entity.IsInside" layout="row" />
            <FormItem {...props} fieId="Entity.Url" layout="row" hidden={IsInside} />
            <FormItem {...props} fieId="SelectedModule" hidden={!IsInside} />
            <FormItem {...props} fieId="SelectedActionIDs" hidden={!IsInside} />
            <FormItem {...props} fieId="Entity.PageName" />
            <FormItem {...props} fieId="Entity.ParentId" />
            <FormItem {...props} fieId="Entity.FolderOnly" />
            <FormItem {...props} fieId="Entity.ShowOnMenu" />
            <FormItem {...props} fieId="Entity.IsPublic" />
            <FormItem {...props} fieId="Entity.DisplayOrder" />
            <FormItem {...props} fieId="Entity.ICon" render={event => {
                return <Icon type={lodash.get(event, 'defaultValues.Entity.ICon')} />
            }} />
        </InfoShellLayout>
    }
}
