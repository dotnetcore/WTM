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
            /** 租户编号 */
            "Entity.TCode":{
                label: "租户编号",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('租户编号') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('租户编号') })} />
            },
            /** 租户名称 */
            "Entity.TName":{
                label: "租户名称",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('租户名称') }} /> }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('租户名称') })} />
            },
            /** 租户数据库 */
            "Entity.TDb":{
                label: "租户数据库",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('租户数据库') })} />
            },
            /** 数据库类型 */
            "Entity.TDbType":{
                label: "数据库类型",
                rules: [],
                formItem: <WtmSelect placeholder="数据库类型" dataSource={[  
                    { Text: "SqlServer", Value: "SqlServer" },
                    { Text: "MySql", Value: "MySql" },
                    { Text: "PgSql", Value: "PgSql" },
                    { Text: "Memory", Value: "Memory" },
                    { Text: "SQLite", Value: "SQLite" },
                    { Text: "Oracle", Value: "Oracle" }
                ]}/>
            },
            /** 数据库架构 */
            "Entity.DbContext":{
                label: "数据库架构",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('数据库架构') })} />
            },
            /** 租户域名 */
            "Entity.TDomain":{
                label: "租户域名",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('租户域名') })} />
            },
            /** 租户 */
            "Entity.TenantCode":{
                label: "租户",
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('租户') })} />
            },
            /** 允许子租户 */
            "Entity.EnableSub":{
                label: "允许子租户",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('允许子租户') }} /> }],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 启用 */
            "Entity.Enabled":{
                label: "启用",
                rules: [{ "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('启用') }} /> }],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?): WTM.FormItem {
        return {
            /** 租户名称 */
            "TName":{
                label: "租户名称",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 租户域名 */
            "TDomain":{
                label: "租户域名",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 租户 */
            "TenantCode":{
                label: "租户",
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
    
    getValue(props: WTM.FormProps, fieId, defaultvalue = undefined) {
        var rv = lodash.toString(props.form.getFieldValue(fieId) || lodash.get(props.defaultValues, fieId));
        if (rv == "") {
            rv = lodash.toString(defaultvalue);
        }
        return rv;
    }
}
