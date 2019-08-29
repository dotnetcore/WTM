import { Input, Switch, Icon, Select, Upload, message, Modal, InputNumber } from 'antd';
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
            /** 权限类型 */
            "DpType": {
                label: "权限类型",
                rules: [{ "required": true, "message": "权限类型不能为空" }],
                formItem: <WtmRadio
                    dataSource={[
                        { Text: "用户组权限", Value: 0 },
                        { Text: "用户权限", Value: 1 },
                    ]} />
            },
            /** 权限名称 */
            "Entity.TableName": {
                label: "权限名称",
                rules: [],
                formItem: <WtmSelect placeholder="选择权限作"
                    dataSource={Request.cache({ url: "/api/_DataPrivilege/GetPrivileges" })}
                />
            },
            /** 允许访问 */
            "SelectedItemsID": {
                label: "允许访问",
                rules: [],
                formItem: <WtmSelect placeholder="选择允许访问的权限"
                    mode="multiple"
                    linkage={['Entity.TableName']}
                    dataSource={(props) => {
                        const table = lodash.get(props, 'Entity.TableName')
                        return table && Request.cache({
                            url: "/api/_DataPrivilege/GetPrivilegeByTableName", body: { table }
                        })
                    }}
                />
            },
            /** 备注 */
            "IsAll": {
                label: "全部权限",
                rules: [{ "required": true, "message": "全部权限不能为空" }],
                formItem: <WtmSelect
                    dataSource={[
                        { Text: "是", Value: true },
                        { Text: "否", Value: false },
                    ]} />
            },
            /** 用户Id */
            "UserItCode": {
                label: "用户账户",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 用户组 */
            "Entity.GroupId": {
                label: "用户组",
                rules: [],
                formItem: <WtmSelect placeholder="选择用户组"
                    dataSource={Request.cache({ url: "/api/_DataPrivilege/GetUserGroups" })}
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
            /** 权限名称 */
            "TableName": {
                label: "权限名称",
                rules: [],
                formItem: <WtmSelect placeholder="全部"
                    dataSource={Request.cache({ url: "/api/_DataPrivilege/GetPrivileges" })}
                />
            },
            /** 权限类型 */
            "DpType": {
                label: "权限类型",
                rules: [],
                formItem: <WtmRadio
                    dataSource={[
                        { Text: "用户组权限", Value: 0 },
                        { Text: "用户权限", Value: 1 },
                    ]} />
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
    },

    getValue(props: WTM.FormProps, fieId, defaultvalue = undefined) {
        var rv = lodash.toString(props.form.getFieldValue(fieId) || lodash.get(props.defaultValues, fieId));
        console.log("rv=" + rv);
        if (rv == "") {
            rv = lodash.toString(defaultvalue);
        }
        return rv;
    }
}