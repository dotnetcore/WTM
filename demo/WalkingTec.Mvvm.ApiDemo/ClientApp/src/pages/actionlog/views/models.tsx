import { Input, Switch, Icon, Select, Upload, message, Modal } from 'antd';
import UploadImg from 'components/form/uploadImg';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular'; 

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
            ModuleName:{
                label: "模块",
                rules: [],
                formItem: <Input placeholder="请输入 模块" />
            },
            ActionName:{
                label: "动作",
                rules: [],
                formItem: <Input placeholder="请输入 动作" />
            },
            ITCode:{
                label: "ITCode",
                rules: [],
                formItem: <Input placeholder="请输入 ITCode" />
            },
            ActionUrl:{
                label: "Url",
                rules: [],
                formItem: <Input placeholder="请输入 Url" />
            },
            ActionTime:{
                label: "操作时间",
                rules: [],
                formItem: <Input placeholder="请输入 操作时间" />
            },
            Duration:{
                label: "时长",
                rules: [{ "required": true, "message": "时长不能为空" }],
                formItem: <Input placeholder="请输入 时长" />
            },
            Remark:{
                label: "备注",
                rules: [],
                formItem: <Input placeholder="请输入 备注" />
            },
            IP:{
                label: "IP",
                rules: [],
                formItem: <Input placeholder="请输入 IP" />
            },
            LogType:{
                label: "类型",
                rules: [{ "required": true, "message": "类型不能为空" }],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
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
                label: "ITCode",
                rules: [],
                formItem: <Input placeholder="" />
            },
            ActionUrl:{
                label: "Url",
                rules: [],
                formItem: <Input placeholder="" />
            },
            IP: {
                label: "IP",
                rules: [],
                formItem: <Input placeholder="请输入 IP" />
            }


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