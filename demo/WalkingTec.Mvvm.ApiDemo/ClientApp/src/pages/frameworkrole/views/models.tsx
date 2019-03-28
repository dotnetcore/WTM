import { Input, Switch, Icon, Select, Upload, message, Modal } from 'antd';
import { WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmCheckbox, WtmCascader } from 'components/form'
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
            "Entity.RoleCode":{
                label: "角色编号",
                rules: [{ "required": true, "message": "角色编号不能为空" }],
                formItem: <Input placeholder="请输入 角色编号" />
            },
            "Entity.RoleName":{
                label: "角色名称",
                rules: [{ "required": true, "message": "角色名称不能为空" }],
                formItem: <Input placeholder="请输入 角色名称" />
            },
            "Entity.RoleRemark":{
                label: "备注",
                rules: [],
                formItem: <Input placeholder="请输入 备注" />
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?) {
        return {
            "RoleCode":{
                label: "角色编号",
                rules: [],
                formItem: <Input placeholder="" />
            },
            "RoleName":{
                label: "角色名称",
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