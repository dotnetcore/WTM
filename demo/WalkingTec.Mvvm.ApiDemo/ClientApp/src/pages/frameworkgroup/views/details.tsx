import { Form } from 'antd';
import { InfoShell, InfoShellFooter, InfoShellCol, ToImg, toValues } from 'components/dataView';
import { DesError, DesForm } from 'components/decorators'; //错误
import GlobalConfig from 'global.config'; //全局配置
import { observer } from 'mobx-react';
import * as React from 'react';
import Regular from 'utils/Regular'; //正则
import Store from '../store'; //页面状态
import Models from './models'; //模型
const formItemLayout = { ...GlobalConfig.formItemLayout };//布局
const formItemLayoutRow = { ...GlobalConfig.formItemLayoutRow }
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
    render() {
        const { form } = this.props;
        const { getFieldDecorator } = form;
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <FooterFormItem submit>
                
<Form.Item label="用户组编码" {...formItemLayout}>
    {getFieldDecorator('GroupCode', {
        rules: [{ "required": true, "message": "用户组编码不能为空" }]
    })(Models.GroupCode)}
</Form.Item>


<Form.Item label="用户组名称" {...formItemLayout}>
    {getFieldDecorator('GroupName', {
        rules: [{ "required": true, "message": "用户组名称不能为空" }]
    })(Models.GroupName)}
</Form.Item>


<Form.Item label="备注" {...formItemLayout}>
    {getFieldDecorator('GroupRemark', {
        rules: []
    })(Models.GroupRemark)}
</Form.Item>

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
                // values = mapValues(values, "YYYY-MM-DD")
                Store.onEdit(values);
            }
        });
    }
    render() {
        const { form } = this.props;
        const { getFieldDecorator } = form;
        const details = { ...Store.details };
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <FooterFormItem submit>
                
<Form.Item label="用户组编码" {...formItemLayout}>
    {getFieldDecorator('GroupCode', {
        rules: [{ "required": true, "message": "用户组编码不能为空" }],
        initialValue: toValues(details['GroupCode'])
    })(Models.GroupCode)}
</Form.Item>


<Form.Item label="用户组名称" {...formItemLayout}>
    {getFieldDecorator('GroupName', {
        rules: [{ "required": true, "message": "用户组名称不能为空" }],
        initialValue: toValues(details['GroupName'])
    })(Models.GroupName)}
</Form.Item>


<Form.Item label="备注" {...formItemLayout}>
    {getFieldDecorator('GroupRemark', {
        rules: [],
        initialValue: toValues(details['GroupRemark'])
    })(Models.GroupRemark)}
</Form.Item>


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
    render() {
        const details = { ...Store.details };
        return <Form >
            <FooterFormItem>
                
<Form.Item label="用户组编码" {...formItemLayout}>
    <span>{toValues(details['GroupCode'], "span")}</span>
</Form.Item>


<Form.Item label="用户组名称" {...formItemLayout}>
    <span>{toValues(details['GroupName'], "span")}</span>
</Form.Item>


<Form.Item label="备注" {...formItemLayout}>
    <span>{toValues(details['GroupRemark'], "span")}</span>
</Form.Item>


<Form.Item label="包含用户" {...formItemLayout}>
    <span>{toValues(details['UsersCount'], "span")}</span>
</Form.Item>


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