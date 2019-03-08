import { Input, Switch, Icon, Select, Upload, message, Modal } from 'antd';
import UploadImg from 'components/form/uploadImg';
import Transfer from 'components/form/transfer';
import Selects from 'components/form/select';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Store from '../store';

/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * formItem  表单组件
 */
export default {
    /**
     * 表单模型 
     * @param props 
     */
    editModels(props?) {
        return {
            ITCode:{
                label: "账号",
                rules: [{ "required": true, "message": "账号不能为空" }],
                formItem: <Input placeholder="请输入 账号" />
            },
            Password:{
                label: "密码",
                rules: [{ "required": true, "message": "密码不能为空" }],
                formItem: <Input placeholder="请输入 密码" />
            },
            Email:{
                label: "邮箱",
                rules: [],
                formItem: <Input placeholder="请输入 邮箱" />
            },
            Name:{
                label: "姓名",
                rules: [{ "required": true, "message": "姓名不能为空" }],
                formItem: <Input placeholder="请输入 姓名" />
            },
            Sex:{
                label: "性别",
                rules: [],
                formItem: <Selects placeholder="性别" dataSource={[  
                    { Text: "男", Value: 0 },
                    { Text: "女", Value: 1 }
                ]}/>
            },
            CellPhone:{
                label: "手机",
                rules: [],
                formItem: <Input placeholder="请输入 手机" />
            },
            HomePhone:{
                label: "座机",
                rules: [],
                formItem: <Input placeholder="请输入 座机" />
            },
            Address:{
                label: "住址",
                rules: [],
                formItem: <Input placeholder="请输入 住址" />
            },
            ZipCode:{
                label: "邮编",
                rules: [],
                formItem: <Input placeholder="请输入 邮编" />
            },
            PhotoId:{
                label: "照片",
                rules: [],
                formItem: <UploadImg />
            },
            IsValid:{
                label: "是否有效",
                rules: [{ "required": true, "message": "是否有效不能为空" }],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            UserRoles:{
                label: "角色",
                rules: [],
                formItem: <Transfer
                    dataSource={Store.Request.cache({ url: "/_FrameworkUserBase/GetFrameworkRoles" })}
                    dataKey="RoleId"
                /> 
            },
            UserGroups:{
                label: "用户组",
                rules: [],
                formItem: <Transfer
                    dataSource={Store.Request.cache({ url: "/_FrameworkUserBase/GetFrameworkGroups" })}
                    dataKey="GroupId"
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
            ITCode:{
                label: "账号",
                rules: [],
                formItem: <Input placeholder="" />
            },
            Name:{
                label: "姓名",
                rules: [],
                formItem: <Input placeholder="" />
            },
            IsValid:{
                label: "是否有效",
                rules: [],
                formItem: <Select placeholder="全部" showArrow allowClear>
                    <Select.Option value={1}>是</Select.Option>
                    <Select.Option value={0}>否</Select.Option>
                </Select>
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