import { Input, Select } from 'antd';
import UploadImg from 'components/form/uploadImg';
import * as React from 'react';
import Regular from 'utils/Regular'; //正则
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