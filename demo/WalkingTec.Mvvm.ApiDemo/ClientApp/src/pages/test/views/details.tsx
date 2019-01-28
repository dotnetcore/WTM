import { Button, Divider, Drawer, Form, Spin } from 'antd';
import ToImg from 'components/dataView/help/toImg';
import decError from 'components/decorators/error';
import decoForm from 'components/decorators/form';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Regular from 'utils/Regular';
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
@decError
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
        return <Drawer
            title={enums[detailsType]}
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
@decError
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
                    })(
                        <Models.PhotoId {...this.props}
                            action={Store.Request.address + Store.Urls.fileUpload.src}
                            onRemove={id => Store.onFileDelete(id)}
                        />)}
                </FormItem>
            </DrawerFormItem>

        </Form>
    }
}
/**
 * 修改表单
 */
@decError
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
                    {getFieldDecorator('ITCode', {
                        rules: [{ "required": true, "message": "账号不能为空" }],
                        initialValue: toValues(details['ITCode'])
                    })(Models.ITCode)}
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    {getFieldDecorator('Email', {
                        rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
                        initialValue: toValues(details['Email'])
                    })(Models.Email)}
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    {getFieldDecorator('Name', {
                        rules: [{ "required": true, "message": "姓名不能为空" }],
                        initialValue: toValues(details['Name'])
                    })(Models.Name)}
                </FormItem>
                <FormItem label="性别" {...formItemLayout}>
                    {getFieldDecorator('Sex', {
                        rules: [],
                        initialValue: toValues(details['Sex'])
                    })(Models.Sex)}
                </FormItem>
                <FormItem label="照片" {...formItemLayout}>
                    {getFieldDecorator('PhotoId', {
                        initialValue: details['PhotoId']
                    })(<Models.PhotoId {...this.props}
                        initialValue={Store.onGetFile(details['PhotoId'])}
                        action={Store.Request.address + Store.Urls.fileUpload.src}
                        onRemove={id => Store.onFileDelete(id)}
                    />)}
                </FormItem>
            </DrawerFormItem>
        </Form>
    }
}
/**
 * 详情
 */
@decError
@observer
class InfoForm extends React.Component<any, any> {
    render() {
        const details = { ...Store.details };
        return <Form >
            <DrawerFormItem>
                <FormItem label="账号" {...formItemLayout}>
                    <span>{toValues(details['ITCode'], "span")}</span>
                </FormItem>
                <FormItem label="邮箱" {...formItemLayout}>
                    <span>{toValues(details['Email'], "span")}</span>
                </FormItem>
                <FormItem label="姓名" {...formItemLayout}>
                    <span>{toValues(details['Name'], "span")}</span>
                </FormItem>
                <FormItem label="头像" {...formItemLayout}>
                    <span>
                        {/* <img style={{ width: 200 }} src={Store.onGetFile(details['photoId'])} /> */}
                        <ToImg download={Store.onFileDownload(details['PhotoId'])} url={Store.onGetFile(details['PhotoId'])} />
                    </span>
                </FormItem>
            </DrawerFormItem>
        </Form>
    }
}
/**
 * Items 外壳
 */
@decError
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
/**
 * 转换 value 值
 * @param value value 值
 * @param showType 转换类型 span 情况下 非基础类型。需要格式为字符串。不然 react 会报错
 */
function toValues(value: any, showType: "value" | "span" = "value") {
    // 检查 value 是否是 null 或者 undefined。
    if (lodash.isNil(value)) {
        return
    }
    // 检查数字类型 
    // if (lodash.isNumber(value)) {
    //     return lodash.toString(value)
    // }
    // 检查其他
    if (lodash.isArray(value) || lodash.isObject(value)) {
        return lodash.toString(value)
    }
    return value
}