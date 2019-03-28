import { Input, Switch, Icon, Select, Upload, message, Modal, InputNumber } from 'antd';
import { WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmCheckbox, WtmCascader } from 'components/form'
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import Regular from 'utils/Regular';
import Store from '../store';

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
    editModels(props?) {
        return {
            /** 学校编码 */
            "Entity.SchoolCode": {
                label: "学校编码",
                rules: [{ "required": true, "message": "学校编码不能为空" }],
                formItem: <Input placeholder="请输入 学校编码" />
            },
            /** 学校名称 */
           "Entity.SchoolName": {
                label: "学校名称",
                rules: [{ "required": true, "message": "学校名称不能为空" }],
                formItem: <Input placeholder="请输入 学校名称" />
            },
            /** 学校类型 */
            "Entity.SchoolType": {
                 label: "学校类型",
                 rules: [{ "required": true, "message": "学校类型不能为空" }],
                 formItem: <WtmSelect placeholder="学校类型" dataSource={[
                     { Text: "公立学校", Value: 0 },
                     { Text: "私立学校", Value: 1 }
                 ]} />
             },
            /** 备注 */
            "Entity.Remark": {
                label: "备注",
                rules: [{ "required": true, "message": "备注不能为空" }],
                formItem: <Input placeholder="请输入 备注" />
            },
            /** 地点 */
            "Entity.PlaceId": {
                label: "地点",
                rules: [],
                formItem: <WtmCascader placeholder="地点"
                    dataSource={Store.Request.cache({ url: "/School/GetCitys" })}
                />
            },
            /** 地点2 */
            "Entity.Place2_Sheng": {
                label: "省",
                rules: [],
                formItem: <WtmSelect placeholder="省"
                    dataSource={Store.Request.cache({ url: "/School/GetSubCities", body: { parentid: null } })}
                />
            },
            /** 地点2 */
            "Entity.Place2_Shi": {
                label: "市",
                rules: [],
                formItem: <WtmSelect placeholder="市"
                    linkageModels="Entity.Place2_Sheng"
                    dataSource={(parentid) => Store.Request.cache({ url: "/School/GetSubCities", body: { parentid } })}
                />
            },
            /** 地点2 */
            "Entity.Place2Id": {
                label: "区",
                rules: [],
                formItem: <WtmSelect placeholder="区"
                    linkageModels="Entity.Place2_Shi"
                    dataSource={(parentid) => Store.Request.cache({ url: "/School/GetSubCities", body: { parentid } })}
                />
            },

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?) {
        return {
            /** 学校编码 */
            "SchoolCode": {
                label: "学校编码",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 学校名称 */
            "SchoolName": {
                label: "学校名称",
                rules: [],
                formItem: <Input placeholder="" />
            },
            /** 学校类型 */
            "SchoolType": {
                label: "学校类型",
                rules: [],
                formItem: <WtmSelect placeholder="全部" dataSource={[
                    { Text: "公立学校", Value: 0 },
                    { Text: "私立学校", Value: 1 }
                ]} />
            },
            /** 地点 */
            "PlaceId": {
                label: "地点",
                rules: [],
                formItem: <WtmSelect placeholder="全部" 
                    dataSource={Store.Request.cache({ url: "/School/GetCitys" })}
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
    }
}