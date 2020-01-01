import { Input, Switch, Icon, Select, Upload, message, Modal, InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Request from 'utils/Request';
import { mergeLocales, getLocalesValue } from 'locale';
import { FormattedMessage } from 'react-intl';
mergeLocales({
    "zh-CN": {
        'dataprivilege.required': '{txt}是必填项',
        'dataprivilege.placeholder': '请选择 {zhText}',
        'dataprivilege.DpType': '权限类型',
        'dataprivilege.DpType.dataSource.0': '用户组权限',
        'dataprivilege.DpType.dataSource.1': '用户权限',
        'dataprivilege.TableName': '权限名称',
        'dataprivilege.SelectedItemsID': '允许访问',
        'dataprivilege.IsAll': '全部权限',
        'dataprivilege.IsAll.true': '是',
        'dataprivilege.IsAll.false': '否',
        'dataprivilege.UserItCode': '用户Id',
        'dataprivilege.GroupId': '用户组',
    },
    "en-US": {
        'dataprivilege.required': `{enText} it's required`,
        'dataprivilege.placeholder': 'Please choose {enText}',
        'dataprivilege.DpType': 'DpType',
        'dataprivilege.DpType.dataSource.0': 'User group permissions',
        'dataprivilege.DpType.dataSource.1': 'User permissions',
        'dataprivilege.TableName': 'TableName',
        'dataprivilege.SelectedItemsID': 'Allow access to',
        'dataprivilege.IsAll': 'All permissions',
        'dataprivilege.IsAll.true': 'true',
        'dataprivilege.IsAll.false': 'false',
        'dataprivilege.UserItCode': 'UserItCode',
        'dataprivilege.GroupId': 'User Group',
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
            /** 权限类型 */
            "DpType": {
                label: <FormattedMessage id='dataprivilege.DpType' />,
                rules: [{
                    "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('dataprivilege.DpType')}}/> }],
                formItem: <WtmRadio
                    dataSource={[
                        { Text: <FormattedMessage id='dataprivilege.DpType.dataSource.0' />, Value: '0' },
                        { Text: <FormattedMessage id='dataprivilege.DpType.dataSource.1' />, Value: '1' },
                    ]} />
            },
            /** 权限名称 */
            "Entity.TableName": {
                label: <FormattedMessage id='dataprivilege.TableName' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('dataprivilege.TableName') }} /> }],
                formItem: <WtmSelect placeholder={<FormattedMessage id='tips.placeholder.choose' values={{ txt: getLocalesValue('dataprivilege.TableName') }} />}
                    dataSource={Request.cache({ url: "/api/_DataPrivilege/GetPrivileges" })}
                />
            },
            /** 允许访问 */
            "SelectedItemsID": {
                label: <FormattedMessage id='dataprivilege.SelectedItemsID' />,
                rules: [],
                formItem: <WtmSelect placeholder={<FormattedMessage id='tips.placeholder.choose' values={{ txt: getLocalesValue('dataprivilege.SelectedItemsID') }} />}
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
                label: <FormattedMessage id='dataprivilege.IsAll' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('dataprivilege.IsAll') }} /> }],
                formItem: <WtmSelect
                    dataSource={[
                        { Text: <FormattedMessage id='tips.bool.true' />, Value: 'true' },
                        { Text: <FormattedMessage id='tips.bool.false' />, Value: 'false' },
                    ]} />
            },
            /** 用户Id */
            "UserItCode": {
                label: <FormattedMessage id='dataprivilege.UserItCode' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('dataprivilege.UserItCode') }} /> }],
                formItem: <Input placeholder="" />
            },
            /** 用户组 */
            "Entity.GroupId": {
                label: <FormattedMessage id='dataprivilege.GroupId' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('dataprivilege.GroupId') }} /> }],
                formItem: <WtmSelect placeholder={<FormattedMessage id='tips.placeholder.choose' values={{ txt: getLocalesValue('dataprivilege.GroupId') }}/>}
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
                label: <FormattedMessage id='dataprivilege.TableName' />,
                rules: [],
                formItem: <WtmSelect placeholder=""
                    dataSource={Request.cache({ url: "/api/_DataPrivilege/GetPrivileges" })}
                />
            },
            /** 权限类型 */
            "DpType": {
                label: <FormattedMessage id='dataprivilege.DpType' />,
                rules: [],
                formItem: <WtmRadio
                    dataSource={[
                        { Text: <FormattedMessage id='dataprivilege.DpType.dataSource.0' />, Value: '0' },
                        { Text: <FormattedMessage id='dataprivilege.DpType.dataSource.1' />, Value: '1' },
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
