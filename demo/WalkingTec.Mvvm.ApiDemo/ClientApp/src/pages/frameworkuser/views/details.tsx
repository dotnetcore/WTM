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
                <Form.Item label="账号" hasFeedback {...formItemLayout} >
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                        props: this.props
                    })(Models.ITCode)}
                </Form.Item >
                <Form.Item label="密码" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Password', {
                        rules: [{ "required": true, "message": "密码不能为空" }],
                    })(Models.Password)}
                </Form.Item >
                <Form.Item label="邮箱" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }]
                    })(Models.Email)}
                </Form.Item >
                <Form.Item label="姓名" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                    })(Models.Name)}
                </Form.Item >
                <Form.Item label="性别" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Sex', {
                        rules: [],
                    })(Models.Sex)}
                </Form.Item >
                <InfoShellCol span={24}>
                    <Form.Item label="照片"  {...formItemLayoutRow}>
                        {getFieldDecorator('PhotoId', {

                        })(Models.PhotoId)}
                    </Form.Item >
                </InfoShellCol>
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
                <Form.Item label="账号" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                        initialValue: toValues(details['ITCode'])
                    })(Models.ITCode)}
                </Form.Item >
                <Form.Item label="邮箱" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
                        initialValue: toValues(details['Email'])
                    })(Models.Email)}
                </Form.Item >
                <Form.Item label="姓名" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                        initialValue: toValues(details['Name'])
                    })(Models.Name)}
                </Form.Item >
                <Form.Item label="性别" hasFeedback {...formItemLayout}>
                    {getFieldDecorator('Sex', {
                        rules: [],
                        initialValue: toValues(details['Sex'])
                    })(Models.Sex)}
                </Form.Item >
                <InfoShellCol span={24}>
                    <Form.Item label="照片"  {...formItemLayoutRow}>
                        {getFieldDecorator('PhotoId', {
                            initialValue: details['PhotoId']
                        })(Models.PhotoId)}
                    </Form.Item >
                </InfoShellCol>
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
                <Form.Item label="账号" {...formItemLayout}>
                    <span>{toValues(details['ITCode'], "span")}</span>
                </Form.Item >
                <Form.Item label="邮箱" {...formItemLayout}>
                    <span>{toValues(details['Email'], "span")}</span>
                </Form.Item >
                <Form.Item label="姓名" {...formItemLayout}>
                    <span>{toValues(details['Name'], "span")}</span>
                </Form.Item >
                <InfoShellCol span={24}>
                    <Form.Item label="头像" {...formItemLayoutRow}>
                        <span>
                            <ToImg fileID={details['PhotoId']} />
                        </span>
                    </Form.Item >
                </InfoShellCol>
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