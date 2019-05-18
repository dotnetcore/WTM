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
            /** 用户组编码 */
            "Entity.GroupCode":{
                label: "用户组编码",
                rules: [{ "required": true, "message": "用户组编码不能为空" }],
                formItem: <Input placeholder="请输入 用户组编码" />
            },
            /** 用户组名称 */
            "Entity.GroupName":{
                label: "用户组名称",
                rules: [{ "required": true, "message": "用户组名称不能为空" }],
                formItem: <Input placeholder="请输入 用户组名称" />
            },
            /** 备注 */
            "Entity.GroupRemark":{
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
    searchModels(props?): WTM.FormItem {
        return {
            /** 用户组编码 */
            "GroupCode":{
                label: "用户组编码",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 用户组名称 */
            "GroupName":{
                label: "用户组名称",
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
    },

    getValue(props: WTM.FormProps, fieId) {
        return lodash.toString(props.form.getFieldValue(fieId) || lodash.get(props.defaultValues, fieId));
    }
}