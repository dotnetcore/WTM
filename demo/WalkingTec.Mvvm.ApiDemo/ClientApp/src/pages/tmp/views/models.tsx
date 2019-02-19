import { Input, Select } from 'antd';
import { ColProps } from 'antd/lib/col';
import Form, { GetFieldDecoratorOptions, WrappedFormUtils } from 'antd/lib/form/Form';
import { FormItemProps } from 'antd/lib/form/FormItem';
import UploadImg from 'components/form/uploadImg';
import GlobalConfig from 'global.config'; //全局配置
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Regular from 'utils/Regular'; //正则
import Store from '../store';
const formItemLayout = { ...GlobalConfig.formItemLayout };//布局
/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * formItem  表单组件
 */
export default function CreateModels(props?) {
    return {
        /** ITCode */
        ITCode: {
            label: "账号",
            rules: [{ "required": true, "message": "账号 不能为空" }],
            formItem: <Input placeholder="请输入 ITCode" />
        },
        /** Password */
        Password: {
            label: "密码",
            rules: [{ "required": true, "message": "密码 不能为空" }],
            formItem: <Input placeholder="请输入 Password" />
        },
        /** Email */
        Email: {
            label: "邮箱",
            rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
            formItem: <Input placeholder="请输入 Email" />
        },
        /** Name */
        Name: {
            label: "名称",
            rules: [],
            formItem: <Input placeholder="请输入 Name" />
        },
        /** 照片 */
        PhotoId: {
            label: "照片",
            rules: [],
            formItem: <UploadImg />
        },
        /** 性别 */
        Sex: {
            label: "性别",
            rules: [],
            formItem: <Select placeholder="性别" showArrow allowClear>
                <Select.Option value={0}>男</Select.Option>
                <Select.Option value={1}>女</Select.Option>
            </Select>
        },
    }
}
/**
 * 表单item
 */
interface IFormItemProps {
    fieId: string;
    /** 获取默认值 */
    defaultValue?: boolean;
    /** Form.Item 的 props */
    formItemProps?: FormItemProps;
    decoratorOptions?: GetFieldDecoratorOptions;
    [key: string]: any;
}
@observer
export class FormItem extends React.Component<IFormItemProps, any> {
    Models = CreateModels(this.props);
    render() {
        const { form, fieId, defaultValue, decoratorOptions, formItemProps } = this.props;
        const { getFieldDecorator }: WrappedFormUtils = form;
        const model = lodash.get(this.Models, fieId);
        let options: GetFieldDecoratorOptions = {
            rules: model.rules,
            ...decoratorOptions
        };
        // 获取默认值
        if (defaultValue && typeof defaultValue === "boolean") {
            options.initialValue = lodash.get(Store.details, fieId);
        }
        return <Form.Item label={model.label} {...formItemLayout}  {...formItemProps}>
            {getFieldDecorator(this.props.fieId, options)(model.formItem)}
        </Form.Item >
    }
}