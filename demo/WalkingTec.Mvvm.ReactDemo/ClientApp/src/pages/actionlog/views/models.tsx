import { Input, Switch, Icon, Select, Upload, message, Modal,InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
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
    editModels(props?): WTM.FormItem {
        return {
            /** 模块 */
            "Entity.ModuleName":{
                label: "模块",
                rules: [],
                formItem: <Input placeholder="请输入 模块" />
            },
            /** 动作 */
            "Entity.ActionName":{
                label: "动作",
                rules: [],
                formItem: <Input placeholder="请输入 动作" />
            },
            /** ITCode */
            "Entity.ITCode":{
                label: "ITCode",
                rules: [],
                formItem: <Input placeholder="请输入 ITCode" />
            },
            /** Url */
            "Entity.ActionUrl":{
                label: "Url",
                rules: [],
                formItem: <Input placeholder="请输入 Url" />
            },
            /** 操作时间 */
            "Entity.ActionTime":{
                label: "操作时间",
                rules: [],
                formItem: <WtmDatePicker placeholder="请输入 操作时间" />
            },
            /** 时长 */
            "Entity.Duration":{
                label: "时长",
                rules: [{ "required": true, "message": "时长不能为空" }],
                formItem: <InputNumber placeholder="请输入 时长" />
            },
            /** 备注 */
            "Entity.Remark":{
                label: "备注",
                rules: [],
                formItem: <Input placeholder="请输入 备注" />
            },
            /** IP */
            "Entity.IP":{
                label: "IP",
                rules: [],
                formItem: <Input placeholder="请输入 IP" />
            },
            /** 类型 */
            "Entity.LogType":{
                label: "类型",
                rules: [{ "required": true, "message": "类型不能为空" }],
                formItem: <WtmSelect placeholder="类型" dataSource={[  
                    { Text: "普通", Value: 0 },
                    { Text: "异常", Value: 1 },
                    { Text: "调试", Value: 2 }
                ]}/>
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?): WTM.FormItem {
        return {
            /** ITCode */
            "ITCode":{
                label: "ITCode",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** Url */
            "ActionUrl":{
                label: "Url",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 操作时间 */
            "ActionTime":{
                label: "操作时间",
                rules: [],
                formItem: <WtmDatePicker placeholder="" />
            },
            /** IP */
            "IP":{
                label: "IP",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 类型 */
            "LogType":{
                label: "类型",
                rules: [],
                formItem: <WtmSelect placeholder="全部" dataSource={[  
                    { Text: "普通", Value: 0 },
                    { Text: "异常", Value: 1 },
                    { Text: "调试", Value: 2 }
                ]}/>
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