import { Input, Select } from 'antd';
import UploadImg from 'components/form/uploadImg';
import Transfer from 'components/form/transfer';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';

import Regular from 'utils/Regular'; //正则
import Store from '../store';
/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * formItem  表单组件
 */
export default {
    /**
     * 编辑 模型 
     * @param props 
     */
    editModels(props?) {
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
            /** 用户组 */
            UserGroups: {
                label: "用户组",
                rules: [],
                formItem: <Select placeholder="性别" showArrow allowClear>
                    <Select.Option value={0}>男</Select.Option>
                    <Select.Option value={1}>女</Select.Option>
                </Select>
            },
            /** 角色 */
            UserRoles: {
                label: "角色",
                rules: [],
                formItem: <Transfer
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({url:"/frameworkuser/GetUserRoles"})}
                    dataKey="RoleId"
                // dataTo={data => data.map(x => ({ RoleId: x }))}
                />
            }
        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?) {
        return {
            /** ITCode */
            ITCode: {
                label: "账号",
                rules: [],
                formItem: <Input placeholder="请输入 ITCode" />
            },
            /** Name */
            Name: {
                label: "名称",
                rules: [],
                formItem: <Input placeholder="请输入 Name" />
            },
        }
    },
    /**
     * 渲染 模型
     */
    renderModels(props?) {
        return lodash.map(props.models, (value, key) => {
            return <FormItem {...props} fieId={key} key={key} />
        })
    }
}