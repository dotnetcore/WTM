import { Button, Drawer, Form, Input, Divider } from 'antd';
import decoForm from 'components/decorators/form';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import { toJS } from 'mobx';
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
    render() {
        return <Drawer
            title="编辑"
            className="app-drawer"
            width={500}
            placement="right"
            closable={false}
            onClose={() => { Store.onPageState("visibleEdit", false) }}
            visible={Store.pageState.visibleEdit}
            destroyOnClose={true}
        >
            {Store.pageState.isUpdate ? <UpdateForm /> : <InsertForm />}
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
                // values = mapValues(values, "YYYY-MM-DD")
                Store.onEdit(values);
            }
        });
    }
    render() {
        const { form, initialValue } = this.props;
        const { getFieldDecorator } = form;
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <div className="app-drawer-formItem">
                <FormItem label="账号" {...formItemLayout}>
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "Please input your undefined!" }],
                    })(
                        <Input placeholder="请输入 ITCode" />
                    )}
                </FormItem>
                <FormItem label="密码" {...formItemLayout}>
                    {getFieldDecorator('Password', {
                        rules: [{ "required": true, "message": "Please input your undefined!" }],
                    })(
                        <Input placeholder="请输入 Password" />
                    )}
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    {getFieldDecorator('Email', {
                    })(
                        <Input placeholder="请输入 Email" />
                    )}
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "Please input your undefined!" }],
                    })(
                        <Input placeholder="请输入 Name" />
                    )}
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
        const { form, initialValue } = this.props;
        const { getFieldDecorator } = form;
        const details = toJS(Store.details);
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <div className="app-drawer-formItem">
                <FormItem label="账号" {...formItemLayout}>
                    {getFieldDecorator('itCode', {
                        rules: [{ "required": true, "message": "Please input your undefined!" }],
                        initialValue: details['itCode']
                    })(
                        <Input placeholder="请输入 ITCode" />
                    )}
                </FormItem>
                <FormItem label="密码" {...formItemLayout}>
                    {getFieldDecorator('password', {
                        rules: [{ "required": true, "message": "Please input your undefined!" }],
                        initialValue: details['password']
                    })(
                        <Input placeholder="请输入 Password" />
                    )}
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    {getFieldDecorator('email', {
                        initialValue: details['email']
                    })(
                        <Input placeholder="请输入 Email" />
                    )}
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    {getFieldDecorator('name', {
                        rules: [{ "required": true, "message": "Please input your undefined!" }],
                        initialValue: details['name']
                    })(
                        <Input placeholder="请输入 Name" />
                    )}
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
