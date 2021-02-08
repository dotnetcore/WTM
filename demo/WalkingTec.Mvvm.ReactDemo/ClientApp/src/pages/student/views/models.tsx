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
            /** 账号 */
            "Entity.ID":{
                label: "账号",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('账号') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('账号') })} />
            },
            /** 密码 */
            "Entity.Password":{
                label: "密码",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('密码') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('密码') })} />
            },
            /** 邮箱 */
            "Entity.Email":{
                label: "邮箱",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('邮箱') })} />
            },
            /** 姓名 */
            "Entity.Name":{
                label: "姓名",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('姓名') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('姓名') })} />
            },
            /** 性别 */
            "Entity.Sex":{
                label: "性别",
                rules: [],
                formItem: <WtmSelect placeholder="性别" dataSource={[  
                    { Text: "男", Value: "Male" },
                    { Text: "女", Value: "Female" }
                ]}/>
            },
            /** 手机 */
            "Entity.CellPhone":{
                label: "手机",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('手机') })} />
            },
            /** 住址 */
            "Entity.Address":{
                label: "住址",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('住址') })} />
            },
            /** 邮编 */
            "Entity.ZipCode":{
                label: "邮编",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('邮编') })} />
            },
            /** 照片 */
            "Entity.PhotoId":{
                label: "照片",
                rules: [],
                formItem: <WtmUploadImg />
            },
            /** 附件 */
            "Entity.FileId":{
                label: "附件",
                rules: [],
                formItem: <WtmUploadImg />
            },
            /** 是否有效 */
            "Entity.IsValid":{
                label: "是否有效",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('是否有效') }} /> }],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 日期 */
            "Entity.EnRollDate":{
                label: "日期",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('日期') }} /> }],
                formItem: <WtmDatePicker placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('日期') })} />
            },
            /** 专业 */
            "Entity.StudentMajor":{
                label: "专业",
                rules: [],
                formItem: <WtmTransfer
                    listStyle={undefined}
                    dataSource={Request.cache({ url: "/api/Student/GetMajors" })}
                    mapKey="MajorId"
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
            /** 姓名 */
            "Name":{
                label: "姓名",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 性别 */
            "Sex":{
                label: "性别",
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')} dataSource={[  
                    { Text: "男", Value: "Male" },
                    { Text: "女", Value: "Female" }
                ]}/>
            },
            /** 是否有效 */
            "IsValid":{
                label: "是否有效",
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')}  dataSource={[
                    { Text: <FormattedMessage id='tips.bool.true' />, Value: true },{ Text: <FormattedMessage id='tips.bool.false' />, Value: false }
                ]}/>
            },
            /** 专业 */
            "SelectedStudentMajorIDs":{
                label: "专业",
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesValue('tips.all')}  multiple
                    dataSource ={ Request.cache({ url: "/api/Student/GetMajors" })}
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
