import { Input } from 'antd';
import { FormItem } from 'components/dataView';
import { WtmCheckbox, WtmEditTable } from 'components/form';
import lodash from 'lodash';
import * as React from 'react';
import { mergeLocales, getLocalesValue, getLocalesTemplate } from 'locale';
import { FormattedMessage } from 'react-intl';

mergeLocales({
    "zh-CN": {
        'frameworkrole.RoleCode': '角色编码',
        'frameworkrole.RoleName': '角色名称',
        'frameworkrole.RoleRemark': '备注',
        'frameworkrole.Page': '页面',
        'frameworkrole.Action': '动作',
    },
    "en-US": {
        'frameworkrole.RoleCode': 'RoleCode',
        'frameworkrole.RoleName': 'RoleName',
        'frameworkrole.RoleRemark': 'Remark',
        'frameworkrole.Page': 'Page',
        'frameworkrole.Action': 'Action(s)',
   }
});

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
                label: <FormattedMessage id='frameworkrole.RoleCode' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkrole.RoleCode') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkrole.RoleCode') })} />
            },
            /** 角色名称 */
            "Entity.RoleName": {
                label: <FormattedMessage id='frameworkrole.RoleName' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkrole.RoleName') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkrole.RoleName') })} />
            },
            /** 备注 */
            "Entity.RoleRemark": {
                label: <FormattedMessage id='frameworkrole.RoleRemark' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkrole.RoleRemark') })} />
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
                label: <FormattedMessage id='frameworkrole.RoleCode' />,
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 角色名称 */
            "RoleName": {
                label: <FormattedMessage id='frameworkrole.RoleName' />,
                rules: [],
                formItem: <Input placeholder="" />
            },

        }
    },

    pageModels(props?): WTM.FormItem {
        return {
            /** 角色编号 */
            "Entity.RoleCode": {
                label: <FormattedMessage id='frameworkrole.RoleCode' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkrole.RoleCode') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkrole.RoleCode') })} />,
                formItemProps: { display: true }
            },
            /** 角色名称 */
            "Entity.RoleName": {
                label: <FormattedMessage id='frameworkrole.RoleName' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkrole.RoleName') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkrole.RoleName') })} />,
                formItemProps: { display: true }
            },
            /** 备注 */
            "Pages": {
                label: <FormattedMessage id='frameworkrole.RoleRemark' />,
                rules: [],
                formItem: (props) => {
                    console.log(props)
                    return <WtmEditTable
                        rowKey="ID"
                        models={{
                            "Name": {
                                label: <FormattedMessage id='frameworkrole.Page' />,
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
                                label: <FormattedMessage id='frameworkrole.Action' />,
                                rules: [],
                                formItem: (props) => {
                                    if (props.defaultValues.AllActions == null || props.defaultValues.AllActions.length == 0) {
                                        return <span></span>;
                                    }
                                    else {
                                        return <WtmCheckbox placeholder={getLocalesTemplate('tips.placeholder.choose', { txt: getLocalesValue('frameworkrole.Action') })}
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
