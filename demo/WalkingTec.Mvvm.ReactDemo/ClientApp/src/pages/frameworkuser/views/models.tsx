import { Input, Switch, Icon, Select, Upload, message, Modal,InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Request from 'utils/Request';

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
    editModels(props?): WTM.FormItem {
        return {
            /** 账号 */
            "Entity.ITCode":{
                label: "账号",
                rules: [{ "required": true, "message": "账号不能为空" }],
                formItem: <Input placeholder="请输入 账号" />
            },
            /** 密码 */
            "Entity.Password":{
                label: "密码",
                rules: [{ "required": true, "message": "密码不能为空" }],
                formItem: <Input placeholder="请输入 密码" />
            },
            /** 邮箱 */
            "Entity.Email":{
                label: "邮箱",
                rules: [],
                formItem: <Input placeholder="请输入 邮箱" />
            },
            /** 姓名 */
            "Entity.Name":{
                label: "姓名",
                rules: [{ "required": true, "message": "姓名不能为空" }],
                formItem: <Input placeholder="请输入 姓名" />
            },
            /** 性别 */
            "Entity.Sex":{
                label: "性别",
                rules: [],
                formItem: <WtmSelect placeholder="性别" dataSource={[  
                    { Text: "男", Value: 0 },
                    { Text: "女", Value: 1 }
                ]}/>
            },
            /** 手机 */
            "Entity.CellPhone":{
                label: "手机",
                rules: [],
                formItem: <Input placeholder="请输入 手机" />
            },
            /** 座机 */
            "Entity.HomePhone":{
                label: "座机",
                rules: [],
                formItem: <Input placeholder="请输入 座机" />
            },
            /** 住址 */
            "Entity.Address":{
                label: "住址",
                rules: [],
                formItem: <Input placeholder="请输入 住址" />
            },
            /** 邮编 */
            "Entity.ZipCode":{
                label: "邮编",
                rules: [],
                formItem: <Input placeholder="请输入 邮编" />
            },
            /** 照片 */
            "Entity.PhotoId":{
                label: "照片",
                rules: [],
                formItem: <WtmUploadImg />
            },
            /** 是否有效 */
            "Entity.IsValid":{
                label: "是否有效",
                rules: [
                    // { "required": true, "message": "是否有效不能为空" }
                ],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 角色 */
            "Entity.UserRoles":{
                label: "角色",
                rules: [],
                formItem: <WtmTransfer
                    dataSource={Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" })}
                    // dataKey="RoleId" 废弃
                /> 
            },
            /** 用户组 */
            "Entity.UserGroups":{
                label: "用户组",
                rules: [],
                formItem: <WtmTransfer
                    dataSource={Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkGroups" })}
                    // dataKey="GroupId" 废弃
                /> 
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?): WTM.FormItem {
        return {
            /** 账号 */
            "ITCode":{
                label: "账号",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 姓名 */
            "Name":{
                label: "姓名",
                rules: [],
                formItem: <Input placeholder="" />
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