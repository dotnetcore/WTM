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
            /** 名称 */
            "Entity.Name":{
                label: "名称",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('名称') })} />
            },
            /** Level */
            "Entity.Level":{
                label: "Level",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('Level') }} /> }],
                formItem: <InputNumber placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('Level') })} />
            },
            /** 父级 */
            "Entity.ParentId":{
                label: "父级",
                rules: [],
                formItem: <WtmSelect placeholder="父级"
                    dataSource ={ Request.cache({ url: "/api/City/GetCitys" })}
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
            /** 父级 */
            "ParentId":{
                label: "父级",
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')}
                    dataSource ={ Request.cache({ url: "/api/City/GetCitys" })}
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
        var rv = lodash.toString(props.form.getFieldValue(fieId)) || lodash.toString(lodash.get(props.defaultValues, fieId));
        if (rv == "") {
            rv = lodash.toString(defaultvalue);
        }
        return rv;
    }
}
