import { Input } from 'antd';
import { FormItem } from 'components/dataView';
import { WtmCheckbox, WtmEditTable } from 'components/form';
import lodash from 'lodash';
import * as React from 'react';

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
            /** 角色编号 */
            "Entity.RoleCode": {
                label: "角色编号",
                rules: [{ "required": true, "message": "角色编号不能为空" }],
                formItem: <Input placeholder="请输入 角色编号" />
            },
            /** 角色名称 */
            "Entity.RoleName": {
                label: "角色名称",
                rules: [{ "required": true, "message": "角色名称不能为空" }],
                formItem: <Input placeholder="请输入 角色名称" />
            },
            /** 备注 */
            "Entity.RoleRemark": {
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
            /** 角色编号 */
            "RoleCode": {
                label: "角色编号",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 角色名称 */
            "RoleName": {
                label: "角色名称",
                rules: [],
                formItem: <Input placeholder="" />
            },

        }
    },

    pageModels(props?): WTM.FormItem {
        return {
            /** 角色编号 */
            "Entity.RoleCode": {
                label: "角色编号",
                rules: [{ "required": true, "message": "角色编号不能为空" }],
                formItem: <Input placeholder="请输入 角色编号" />,
                formItemProps: { display: true }
            },
            /** 角色名称 */
            "Entity.RoleName": {
                label: "角色名称",
                rules: [{ "required": true, "message": "角色名称不能为空" }],
                formItem: <Input placeholder="请输入 角色名称" />,
                formItemProps: { display: true }
            },
            /** 备注 */
            "Pages": {
                label: "备注",
                rules: [],
                formItem: (props) => {
                    console.log(props)
                    return <WtmEditTable
                        rowKey="ID"
                        models={{
                            "Name": {
                                label: "页面",
                                rules: [],
                                formItem: (props) => {
                                    var m = props.defaultValues.Level * 20;
                                    const style = {
                                        marginLeft: m + 'px'
                                    };
                                    return <span style={style}>{props.defaultValues.Name}</span>
                                }
                            },
                            "Actions": {
                                label: "动作",
                                rules: [],
                                formItem: (props) => {
                                    if (props.defaultValues.AllActions == null || props.defaultValues.AllActions.length == 0) {
                                        return <span></span>;
                                    }
                                    else {
                                        return <WtmCheckbox placeholder="选择动作"
                                            dataSource={props.defaultValues.AllActions}
                                        />
                                    }
                                }
                            }
                        }}
                        addButton={false}
                        deleteButton={false}
                        setValues={{ abcd: 1234 }}
                    />
                }
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