import { Button, Divider, Drawer, Form, Spin } from 'antd';
import decoForm from 'components/decorators/form';
import { observer } from 'mobx-react';
import * as React from 'react';
import Regular from 'utils/Regular';
import ToImg from 'components/dataView/help/toImg';
import Store from '../store';
import Models from './models';
const FormItem = Form.Item;
const formItemLayout = {
    labelCol: {
        xs: { span: 24 },
        sm: { span: 6 },
    },
    wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
    },
};
/**
 *  详情 窗口 
 *  根据 类型 显示不同的 窗口
 */
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
        const { detailsType, visibleEdit, loadingEdit } = Store.pageState
        return <Drawer
            title="编辑"
            className="app-drawer"
            width={500}
            placement="right"
            closable={false}
            onClose={() => { Store.onPageState("visibleEdit", false) }}
            visible={visibleEdit}
            destroyOnClose={true}
        >
            {this.renderBody(detailsType)}
        </Drawer>
    }
}
/**
 * 添加表单
 */
@decoForm
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
            <DrawerFormItem submit>
                <FormItem label="账号" {...formItemLayout}>
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                    })(Models.ITCode)}
                </FormItem>
                <FormItem label="密码" {...formItemLayout}>
                    {getFieldDecorator('Password', {
                        rules: [{ "required": true, "message": "密码不能为空" }],
                    })(Models.Password)}
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    {getFieldDecorator('Email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }]
                    })(Models.Email)}
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                    })(Models.Name)}
                </FormItem>
                <FormItem label="性别" {...formItemLayout}>
                    {getFieldDecorator('Sex', {
                        rules: [],
                    })(Models.Sex)}
                </FormItem>
                <FormItem label="照片" {...formItemLayout}>
                    {getFieldDecorator('PhotoId', {
                    })(<Models.PhotoId {...this.props} />)}
                </FormItem>
            </DrawerFormItem>

        </Form>
    }
}
/**
 * 修改表单
 */
@decoForm
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
            <DrawerFormItem submit>
                <FormItem label="账号" {...formItemLayout}>
                    {getFieldDecorator('itCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                        initialValue: details['itCode']
                    })(Models.ITCode)}
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    {getFieldDecorator('email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
                        initialValue: details['email']
                    })(Models.Email)}
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    {getFieldDecorator('name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                        initialValue: details['name']
                    })(Models.Name)}
                </FormItem>
                <FormItem label="性别" {...formItemLayout}>
                    {getFieldDecorator('sex', {
                        rules: [],
                        initialValue: String(details['sex'])
                    })(Models.Sex)}
                </FormItem>
                <FormItem label="照片" {...formItemLayout}>
                    {getFieldDecorator('PhotoId', {
                        initialValue: details['photoId']
                    })(<Models.PhotoId {...this.props} initialValue={Store.onGetFile(details['photoId'])} />)}
                </FormItem>
            </DrawerFormItem>
        </Form>
    }
}
/**
 * 详情
 */
@observer
class InfoForm extends React.Component<any, any> {
    render() {
        const details = { ...Store.details };
        return <Form >
            <DrawerFormItem>
                <FormItem label="账号" {...formItemLayout}>
                    <span>{details['itCode']}</span>
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    <span>{details['email']}</span>
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    <span>{details['name']}</span>
                </FormItem>
                <FormItem label="头像" {...formItemLayout}>
                    <span>
                        {/* <img style={{ width: 200 }} src={Store.onGetFile(details['photoId'])} /> */}
                        <ToImg download={Store.onFileDownload(details['photoId'])} url={Store.onGetFile(details['photoId'])} />
                    </span>
                </FormItem>
            </DrawerFormItem>
        </Form>
    }
}
/**
 * Items 外壳
 */
@observer
class DrawerFormItem extends React.Component<{ submit?: boolean }, any> {
    render() {
        const { loadingEdit } = Store.pageState;
        return < >
            <div className="app-drawer-formItem">
                <Spin tip="Loading..." spinning={loadingEdit}>
                    {this.props.children}
                </Spin>
            </div>
            <div className="app-drawer-btns" >
                <Button onClick={() => Store.onPageState("visibleEdit", false)} >取消 </Button>
                {this.props.submit && <>
                    <Divider type="vertical" />
                    <Button loading={Store.pageState.loadingEdit} type="primary" htmlType="submit"  >提交 </Button>
                </>}
            </div>
        </>
    }
}
