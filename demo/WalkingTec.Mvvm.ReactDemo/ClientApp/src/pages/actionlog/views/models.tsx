import { Input, Switch, Icon, Select, Upload, message, Modal,InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload, WtmRangePicker} from 'components/form';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Store from '../store';
import { mergeLocales, getLocalesValue, getLocalesTemplate } from 'locale';
import { FormattedMessage } from 'react-intl';

mergeLocales({
    "zh-CN": {
        'actionlog.ModuleName': '模块',
        'actionlog.ActionName': '动作',
        'actionlog.ITCode': '账户',
        'actionlog.ActionUrl': 'Url',
        'actionlog.ActionTime': '操作时间',
        'actionlog.Duration': '时长',
        'actionlog.Remark': '备注',
        'actionlog.IP': 'IP',
        'actionlog.LogType': '类型',
        'actionlog.LogType.0':'普通',
        'actionlog.LogType.1': '异常',
        'actionlog.LogType.2': '调试',
    },
    "en-US": {
        'actionlog.ModuleName': 'Module',
        'actionlog.ActionName': 'Action',
        'actionlog.ITCode': 'Account',
        'actionlog.ActionUrl': 'Url',
        'actionlog.ActionTime': 'ActionTime',
        'actionlog.Duration': 'Duration',
        'actionlog.Remark': 'Remark',
        'actionlog.IP': 'IP',
        'actionlog.LogType': 'LogType',
        'actionlog.LogType.0': 'Normal',
        'actionlog.LogType.1': 'Exception',
        'actionlog.LogType.2': 'Debug',
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
            /** 模块 */
            "Entity.ModuleName":{
                label: <FormattedMessage id='actionlog.ModuleName' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.ModuleName') })} />
            },
            /** 动作 */
            "Entity.ActionName":{
                label: <FormattedMessage id='actionlog.ActionName' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.ActionName') })} />
            },
            /** ITCode */
            "Entity.ITCode":{
                label: <FormattedMessage id='actionlog.ITCode' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.ITCode') })} />
            },
            /** Url */
            "Entity.ActionUrl":{
                label: <FormattedMessage id='actionlog.ActionUrl' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.ActionUrl') })}/>
            },
            /** 操作时间 */
            "Entity.ActionTime":{
                label: <FormattedMessage id='actionlog.ActionTime' />,
                rules: [],
                formItem: <WtmDatePicker placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.ActionTime') })} />
            },
            /** 时长 */
            "Entity.Duration":{
                label: <FormattedMessage id='actionlog.Duration' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('actionlog.Duration') }} /> }],
                formItem: <InputNumber placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.Duration') })} />
            },
            /** 备注 */
            "Entity.Remark":{
                label: <FormattedMessage id='actionlog.Remark' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.Remark') })} />
            },
            /** IP */
            "Entity.IP":{
                label: <FormattedMessage id='actionlog.IP' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.IP') })} />
            },
            /** 类型 */
            "Entity.LogType":{
                label: <FormattedMessage id='actionlog.LogType' />,
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('actionlog.LogType') }} />}],
                formItem: <WtmSelect placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('actionlog.LogType') })} dataSource={[  
                    { Text: <FormattedMessage id='actionlog.LogType.0' />, Value: 0 },
                    { Text: <FormattedMessage id='actionlog.LogType.1' />, Value: 1 },
                    { Text: <FormattedMessage id='actionlog.LogType.2' />, Value: 2 }
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
                label: <FormattedMessage id='actionlog.ITCode' />,
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** Url */
            "ActionUrl":{
                label: <FormattedMessage id='actionlog.ActionUrl' />,
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 操作时间 */
            "ActionTime":{
                label: <FormattedMessage id='actionlog.ActionTime' />,
                rules: [],
                formItem: <WtmRangePicker placeholder="" />
            },
            /** IP */
            "IP":{
                label: <FormattedMessage id='actionlog.IP' />,
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 类型 */
            "LogType":{
                label: <FormattedMessage id='actionlog.LogType' />,
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')} mode="multiple" dataSource={[  
                    { Text: <FormattedMessage id='actionlog.LogType.0' />, Value: 0 },
                    { Text: <FormattedMessage id='actionlog.LogType.1' />, Value: 1 },
                    { Text: <FormattedMessage id='actionlog.LogType.2' />, Value: 2 }
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
