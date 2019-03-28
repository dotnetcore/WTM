import { Input, Switch, Icon, Select, Upload, message, Modal,InputNumber } from 'antd';
import { WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg,WtmCheckbox } from 'components/form'
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
            /** 名称 */
            "Entity.Name":{
                label: "名称",
                rules: [],
                formItem: <Input placeholder="请输入 名称" />
            },
            /** Parent */
            "Entity.ParentId":{
                label: "Parent",
                rules: [],
                formItem: <WtmSelect placeholder="Parent" 
                    dataSource ={ Store.Request.cache({ url: "/City/GetCitys" })} 
                /> 
            },
            /** Level */
            "Entity.Level":{
                label: "Level",
                rules: [{ "required": true, "message": "Level不能为空" }],
                formItem: <InputNumber placeholder="请输入 Level" />
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?) {
        return {

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