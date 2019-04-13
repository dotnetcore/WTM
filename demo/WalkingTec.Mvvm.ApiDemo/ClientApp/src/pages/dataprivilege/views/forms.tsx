import { Col } from 'antd';
import { DialogForm, DialogFormDes, DialogFormSubmit, FormItem, InfoShellLayout, DialogLoadData, } from 'components/dataView';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store'; //页面状态
import Models from './models'; //模型
import { bool } from 'prop-types';
import { Help } from 'utils/Help';

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
        const Isgroup = Models.getValue(props, "DpType") == "0"
        const Isall = Models.getValue(props, "IsAll") == "true"
        return <InfoShellLayout>
            <FormItem {...props} fieId="DpType" layout="row" value='0' />
            <FormItem {...props} fieId="UserItCode" hidden={Isgroup} />
            <FormItem {...props} fieId="Entity.GroupId" hidden={!Isgroup} />
            <Col span={24}></Col>
            <FormItem {...props} fieId="Entity.TableName" />
            <FormItem {...props} fieId="IsAll" value='true' />
            <FormItem {...props} fieId="SelectedItemsID" disabled={Isall} />
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
    componentDidMount() {
        console.log("isall in mount", this.props.form.getFieldValue("IsAll"));
    }
    render() {
        const { form } = this.props;
        const { getFieldDecorator } = form;
        const props = {
            ...this.props,
            models: this.models,
        }
        getFieldDecorator('Entity.ID', { initialValue: lodash.get(this.props.defaultValues, 'Entity.ID') })
        const Isgroup = Models.getValue(props,"DpType") == "0"
        const Isall = Models.getValue(props, "IsAll") == "true"
        return <InfoShellLayout>
            <FormItem {...props} fieId="DpType" layout="row" />
            <FormItem {...props} fieId="UserItCode" hidden={Isgroup} />
            <FormItem {...props} fieId="Entity.GroupId" hidden={!Isgroup} />
            <Col span={24}></Col>
            <FormItem {...props} fieId="Entity.TableName" />
            <FormItem {...props} fieId="IsAll" />
            <FormItem {...props} fieId="SelectedItemsID" disabled={Isall} />
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
        const Isgroup = Models.getValue(props, "DpType") == "0"
        const Isall = Models.getValue(props, "IsAll") == "true"
        return <InfoShellLayout>
            <FormItem {...props} fieId="DpType" layout="row" />
            <FormItem {...props} fieId="UserItCode" hidden={Isgroup} />
            <FormItem {...props} fieId="Entity.GroupId" hidden={!Isgroup} />
            <Col span={24}></Col>
            <FormItem {...props} fieId="Entity.TableName" />
            <FormItem {...props} fieId="IsAll" />
            <FormItem {...props} fieId="SelectedActionIDs" disabled={Isall} />
        </InfoShellLayout>
    }
}
