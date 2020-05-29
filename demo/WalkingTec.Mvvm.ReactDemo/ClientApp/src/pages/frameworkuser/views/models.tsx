import { Input, Switch, Icon, Select, Upload, message, Modal, InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
import { FormItem } from 'components/dataView';
import { DesValueFormatter } from 'components/decorators';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Request from 'utils/Request';
import { mergeLocales, getLocalesValue, getLocalesTemplate } from 'locale';
import { FormattedMessage } from 'react-intl';

mergeLocales({
    "zh-CN": {
        'frameworkuser.ITCode': '账号',
        'frameworkuser.Password': '密码',
        'frameworkuser.Email': '邮箱',
        'frameworkuser.Name': '姓名',
        'frameworkuser.Sex': '性别',
        'frameworkuser.Sex.0': '男',
        'frameworkuser.Sex.1': '女',
        'frameworkuser.CellPhone': '手机',
        'frameworkuser.HomePhone': '座机',
        'frameworkuser.Address': '住址',
        'frameworkuser.ZipCode': '邮编',
        'frameworkuser.PhotoId': '照片',
        'frameworkuser.IsValid': '是否有效',
        'frameworkuser.UserRoles': '角色',
        'frameworkuser.UserGroups': '用户组',
    },
    "en-US": {
        'frameworkuser.ITCode': 'Account',
        'frameworkuser.Password': 'Password',
        'frameworkuser.Email': 'Email',
        'frameworkuser.Name': 'Name',
        'frameworkuser.Sex': 'Gender',
        'frameworkuser.Sex.0': 'Male',
        'frameworkuser.Sex.1': 'Female',
        'frameworkuser.CellPhone': 'CellPhone',
        'frameworkuser.HomePhone': 'HomePhone',
        'frameworkuser.Address': 'Address',
        'frameworkuser.ZipCode': 'ZipCode',
        'frameworkuser.PhotoId': 'Photo',
        'frameworkuser.IsValid': 'IsValid',
        'frameworkuser.UserRoles': 'Roles',
        'frameworkuser.UserGroups': 'Groups',
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
            /** 账号 */
            "Entity.ITCode": {
                label: <FormattedMessage id='frameworkuser.ITCode' />,
                rules: [{
                    "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkuser.ITCode') }} />
                }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.ITCode') })} />
            },
            /** 密码 */
            "Entity.Password": {
                label: <FormattedMessage id='frameworkuser.Password' />,
                rules: [{
                    "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkuser.Password') }} />
                }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.Password') })} />
            },
            /** 邮箱 */
            "Entity.Email": {
                label: <FormattedMessage id='frameworkuser.Email' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.Email') })} />
            },
            /** 姓名 */
            "Entity.Name": {
                label: <FormattedMessage id='frameworkuser.Name' />,
                rules: [{
                    "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkuser.Name') }} />
                }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.Name') })} />
            },
            /** 性别 */
            "Entity.Sex": {
                label: <FormattedMessage id='frameworkuser.Sex' />,
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesTemplate('tips.placeholder.choose', { txt: getLocalesValue('frameworkuser.Sex') })} dataSource={[
                    { Text: <FormattedMessage id='frameworkuser.Sex.0' />, Value: 0 },
                    { Text: <FormattedMessage id='frameworkuser.Sex.1' />, Value: 1 }
                ]} />
            },
            /** 手机 */
            "Entity.CellPhone": {
                label: <FormattedMessage id='frameworkuser.CellPhone' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.CellPhone') })} />
            },
            /** 座机 */
            "Entity.HomePhone": {
                label: <FormattedMessage id='frameworkuser.HomePhone' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.HomePhone') })} />
            },
            /** 住址 */
            "Entity.Address": {
                label: <FormattedMessage id='frameworkuser.Address' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.Address') })} />
            },
            /** 邮编 */
            "Entity.ZipCode": {
                label: <FormattedMessage id='frameworkuser.ZipCode' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkuser.ZipCode') })} />
            },
            /** 照片 */
            "Entity.PhotoId": {
                label: <FormattedMessage id='frameworkuser.PhotoId' />,
                rules: [],
                formItem: <WtmUploadImg />
            },
            /** 是否有效 */
            "Entity.IsValid": {
                label: <FormattedMessage id='frameworkuser.IsValid' />,
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 角色 */
            "Entity.UserRoles": {
                label: <FormattedMessage id='frameworkuser.UserRoles' />,
                rules: [],
                formItem: <WtmTransfer
                    listStyle={undefined}
                    dataSource={Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" })}
                    mapKey="RoleId"
                />
            },
            /** 用户组 */
            "Entity.UserGroups": {
                label: <FormattedMessage id='frameworkuser.UserGroups' />,
                rules: [],
                formItem: <WtmTransfer
                    listStyle={undefined}
                    dataSource={Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkGroups" })}
                    mapKey="GroupId"
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
            /** 账号 */
            "ITCode": {
                label: <FormattedMessage id='frameworkuser.ITCode' />,
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 姓名 */
            "Name": {
                label: <FormattedMessage id='frameworkuser.Name' />,
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
