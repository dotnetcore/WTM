import { Input, Switch, Icon, Select, Upload, message, Modal,InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Request from 'utils/Request';
import { mergeLocales, getLocalesValue, getLocalesTemplate } from 'locale';
import { FormattedMessage } from 'react-intl';

mergeLocales({
    "zh-CN": {
        'frameworkgroup.GroupCode': '用户组编码',
        'frameworkgroup.GroupName': '用户组名称',
        'frameworkgroup.GroupRemark': '备注',
    },
    "en-US": {
        'frameworkgroup.GroupCode': 'GroupCode',
        'frameworkgroup.GroupName': 'GroupName',
        'frameworkgroup.GroupRemark': 'Remark',
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
            /** 用户组编码 */
            "Entity.GroupCode":{
                label: <FormattedMessage id='frameworkgroup.GroupCode' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkgroup.GroupCode') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkgroup.GroupCode') })} />
            },
            /** 用户组名称 */
            "Entity.GroupName":{
                label: <FormattedMessage id='frameworkgroup.GroupName' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkgroup.GroupName') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkgroup.GroupName') })} />
            },
            /** 备注 */
            "Entity.GroupRemark":{
                label: <FormattedMessage id='frameworkgroup.GroupRemark' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkgroup.GroupRemark') })} />
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
                label: <FormattedMessage id='frameworkgroup.GroupCode' />,
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 用户组名称 */
            "GroupName":{
                label: <FormattedMessage id='frameworkgroup.GroupName' />,
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
