import { Form } from 'antd';
import { InfoShell, InfoShellFooter, InfoShellFormItem, ToImg, toValues } from 'components/dataView';
import { DesError, DesForm } from 'components/decorators'; //错误
import GlobalConfig from 'global.config'; //全局配置
import { observer } from 'mobx-react';
import * as React from 'react';
import Regular from 'utils/Regular'; //正则
import Store from '../store'; //页面状态
import Models from './models'; //模型
const formItemLayout = { ...GlobalConfig.formItemLayout };//布局
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
                <InfoShellFormItem label="账号" hasFeedback {...formItemLayout} >
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                    })(Models.ITCode)}
                </InfoShellFormItem>
                <InfoShellFormItem label="密码" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Password', {
                        rules: [{ "required": true, "message": "密码不能为空" }],
                    })(Models.Password)}
                </InfoShellFormItem>
                <InfoShellFormItem label="邮箱" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }]
                    })(Models.Email)}
                </InfoShellFormItem>
                <InfoShellFormItem label="姓名" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                    })(Models.Name)}
                </InfoShellFormItem>
                <InfoShellFormItem label="性别" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Sex', {
                        rules: [],
                    })(Models.Sex)}
                </InfoShellFormItem>
                <InfoShellFormItem label="照片"  {...formItemLayout}>
                    {getFieldDecorator('PhotoId', {
                    })(
                        <Models.PhotoId {...this.props} />)}
                </InfoShellFormItem>
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
                <InfoShellFormItem label="账号" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                        initialValue: toValues(details['ITCode'])
                    })(Models.ITCode)}
                </InfoShellFormItem>
                <InfoShellFormItem label="邮箱" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
                        initialValue: toValues(details['Email'])
                    })(Models.Email)}
                </InfoShellFormItem>
                <InfoShellFormItem label="姓名" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                        initialValue: toValues(details['Name'])
                    })(Models.Name)}
                </InfoShellFormItem>
                <InfoShellFormItem label="性别" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Sex', {
                        rules: [],
                        initialValue: toValues(details['Sex'])
                    })(Models.Sex)}
                </InfoShellFormItem>
                <InfoShellFormItem label="照片"  {...formItemLayout}>
                    {getFieldDecorator('PhotoId', {
                        initialValue: details['PhotoId']
                    })(<Models.PhotoId {...this.props} />)}
                </InfoShellFormItem>
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
                <InfoShellFormItem label="账号" {...formItemLayout}>
                    <span>{toValues(details['ITCode'], "span")}</span>
                </InfoShellFormItem>
                <InfoShellFormItem label="邮箱" {...formItemLayout}>
                    <span>{toValues(details['Email'], "span")}</span>
                </InfoShellFormItem>
                <InfoShellFormItem label="姓名" {...formItemLayout}>
                    <span>{toValues(details['Name'], "span")}</span>
                </InfoShellFormItem>
                <InfoShellFormItem label="头像" {...formItemLayout}>
                    <span>
                        <ToImg fileID={details['PhotoId']} />
                    </span>
                </InfoShellFormItem>
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