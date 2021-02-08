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
            /** 专业编码 */
            "Entity.MajorCode":{
                label: "专业编码",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('专业编码') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('专业编码') })} />
            },
            /** 专业名称 */
            "Entity.MajorName":{
                label: "专业名称",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('专业名称') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('专业名称') })} />
            },
            /** 专业类别 */
            "Entity.MajorType":{
                label: "专业类别",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('专业类别') }} /> }],
                formItem: <WtmSelect placeholder="专业类别" dataSource={[  
                    { Text: "必修", Value: "Required" },
                    { Text: "选修", Value: "Optional" }
                ]}/>
            },
            /** 备注 */
            "Entity.Remark":{
                label: "备注",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('备注') })} />
            },
            /** 所属学校 */
            "Entity.SchoolId":{
                label: "所属学校",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('所属学校') }} /> }],
                formItem: <WtmSelect placeholder="所属学校"
                    dataSource ={ Request.cache({ url: "/api/Major/GetSchools" })}
                /> 
            },
            /** 学生 */
            "Entity.StudentMajors":{
                label: "学生",
                rules: [],
                formItem: <WtmTransfer
                    listStyle={undefined}
                    dataSource={Request.cache({ url: "/api/Major/GetStudents" })}
                    mapKey="StudentId"
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
            /** 专业编码 */
            "MajorCode":{
                label: "专业编码",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 专业名称 */
            "MajorName":{
                label: "专业名称",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 专业类别 */
            "MajorType":{
                label: "专业类别",
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')} dataSource={[  
                    { Text: "必修", Value: "Required" },
                    { Text: "选修", Value: "Optional" }
                ]}/>
            },
            /** 所属学校 */
            "SchoolId":{
                label: "所属学校",
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')}
                    dataSource ={ Request.cache({ url: "/api/Major/GetSchools" })}
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
