import { Button, Drawer, Form, Input, Divider } from 'antd';
import decoForm from 'components/decorators/form';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import { toJS } from 'mobx';
import Regular from 'utils/Regular';
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
@observer
export default class extends React.Component<any, any> {
    /**
     * 根据状态类型 渲染  添加。修改，详情信息
     * @param detailsType 
     */
    renderBody(detailsType) {
        switch (detailsType) {
            case 'Insert':
                return <InsertForm {...this.props} />
                break;
            case 'Update':
                return <UpdateForm {...this.props} />
                break;
            default:
                return <InfoForm {...this.props} />
                break;
        }
    }
    render() {
        const { detailsType, visibleEdit } = Store.pageState
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
const formItems = {
    ITCode: <Input placeholder="请输入 ITCode" />,
    Password: <Input placeholder="请输入 Password" />,
    Email: <Input placeholder="请输入 Email" />,
    Name: <Input placeholder="请输入 Name" />
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
            <div className="app-drawer-formItem">
                <FormItem label="账号" {...formItemLayout}>
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                    })(formItems.ITCode)}
                </FormItem>
                <FormItem label="密码" {...formItemLayout}>
                    {getFieldDecorator('Password', {
                        rules: [{ "required": true, "message": "密码不能为空" }],
                    })(formItems.Password)}
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    {getFieldDecorator('Email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }]
                    })(formItems.Email)}
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                    })(formItems.Name)}
                </FormItem>
            </div>
            <div className="app-drawer-btns" >
                <Button onClick={() => Store.onPageState("visibleEdit", false)} >取消 </Button>
                <Divider type="vertical" />
                <Button loading={Store.pageState.loadingEdit} type="primary" htmlType="submit"  >提交 </Button>
            </div>
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
        const details = toJS(Store.details);
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <div className="app-drawer-formItem">
                <FormItem label="账号" {...formItemLayout}>
                    {getFieldDecorator('itCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                        initialValue: details['itCode']
                    })(formItems.ITCode)}
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    {getFieldDecorator('email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
                        initialValue: details['email']
                    })(formItems.Email)}
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    {getFieldDecorator('name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                        initialValue: details['name']
                    })(formItems.Name)}
                </FormItem>
            </div>
            <div className="app-drawer-btns" >
                <Button onClick={() => Store.onPageState("visibleEdit", false)} >取消 </Button>
                <Divider type="vertical" />
                <Button loading={Store.pageState.loadingEdit} type="primary" htmlType="submit"  >提交 </Button>
            </div>
        </Form>
    }
}
/**
 * 详情
 */
@observer
class InfoForm extends React.Component<any, any> {
    render() {
        const details = toJS(Store.details);
        return <Form >
            <div className="app-drawer-formItem">
                <FormItem label="账号" {...formItemLayout}>
                    <span>{details['itCode']}</span>
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    <span>{details['itCode']}</span>
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    <span>{details['Name']}</span>
                </FormItem>
            </div>
            <div className="app-drawer-btns" >
                <Button onClick={() => Store.onPageState("visibleEdit", false)} >取消 </Button>
            </div>
        </Form>
    }
}
