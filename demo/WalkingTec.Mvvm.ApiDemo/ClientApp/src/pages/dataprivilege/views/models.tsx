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
            /** 用户 */
            UserId:{
                label: "用户",
                rules: [],
                formItem: <WtmSelect placeholder="用户" 
                    dataSource ={ Store.Request.cache({ url: "/DataPrivilege/GetFrameworkUserBases" })} 
                /> 
            },
            /** 用户组 */
            GroupId:{
                label: "用户组",
                rules: [],
                formItem: <WtmSelect placeholder="用户组" 
                    dataSource ={ Store.Request.cache({ url: "/DataPrivilege/GetFrameworkGroups" })} 
                /> 
            },
            /** 数据权限 */
            TableName:{
                label: "数据权限",
                rules: [{ "required": true, "message": "数据权限不能为空" }],
                formItem: <Input placeholder="请输入 数据权限" />
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?) {
        return {
            /** 用户 */
            UserId:{
                label: "用户",
                rules: [],
                formItem: <WtmSelect placeholder="全部" 
                    dataSource ={ Store.Request.cache({ url: "/DataPrivilege/GetFrameworkUserBases" })} 
                /> 
            },
            /** 用户组 */
            GroupId:{
                label: "用户组",
                rules: [],
                formItem: <WtmSelect placeholder="全部" 
                    dataSource ={ Store.Request.cache({ url: "/DataPrivilege/GetFrameworkGroups" })} 
                /> 
            },
            /** 数据权限 */
            TableName:{
                label: "数据权限",
                rules: [],
                formItem: <Input placeholder="" />
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