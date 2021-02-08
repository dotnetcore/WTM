import { Input, Switch, Icon, Select, Upload, message, Modal,InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload ,WtmRangePicker} from 'components/form';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Request from 'utils/Request';
import { mergeLocales, getLocalesValue, getLocalesTemplate } from 'locale';
import { FormattedMessage } from 'react-intl';

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
            /** 学校编码 */
            "Entity.SchoolCode":{
                label: "学校编码",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('学校编码') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('学校编码') })} />
            },
            /** 学校名称 */
            "Entity.SchoolName":{
                label: "学校名称",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('学校名称') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('学校名称') })} />
            },
            /** 学校类型 */
            "Entity.SchoolType":{
                label: "学校类型",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('学校类型') }} /> }],
                formItem: <WtmSelect placeholder="学校类型" dataSource={[  
                    { Text: "公立学校", Value: "PUB" },
                    { Text: "私立学校", Value: "PRI" }
                ]}/>
            },
            /** 备注 */
            "Entity.Remark":{
                label: "备注",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('备注') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('备注') })} />
            },
            /** 级别 */
            "Entity.Level":{
                label: "级别",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('级别') }} /> }],
                formItem: <InputNumber placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('级别') })} />
            },
            /** 地点 */
            "Entity.PlaceId":{
                label: "地点",
                rules: [],
                formItem: <WtmSelect placeholder="地点"
                    dataSource ={ Request.cache({ url: "/api/School/GetCitys" })}
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
            /** 学校编码 */
            "SchoolCode":{
                label: "学校编码",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 学校名称 */
            "SchoolName":{
                label: "学校名称",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 地点 */
            "PlaceId":{
                label: "地点",
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')}
                    dataSource ={ Request.cache({ url: "/api/School/GetCitys" })}
                /> 
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
    
    getValue(props: WTM.FormProps, fieId, defaultvalue = undefined) {
        var rv = lodash.toString(props.form.getFieldValue(fieId) || lodash.get(props.defaultValues, fieId));
        if (rv == "") {
            rv = lodash.toString(defaultvalue);
        }
        return rv;
    }
}
