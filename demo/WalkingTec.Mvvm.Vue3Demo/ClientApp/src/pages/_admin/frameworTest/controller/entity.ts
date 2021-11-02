


import { $i18n, WTM_EntitiesField, WTM_ValueType, FieldRequest } from '@/client';
import router from '@/router';
import { ColGroupDef, GridOptions } from 'ag-grid-community';
import { ColDef } from 'ag-grid-community';
import lodash from 'lodash';
//import { EnumLocaleLabel } from '../locales';

/**
 * 页面实体
 */
class Entity {
    
    readonly ITCode: WTM_EntitiesField = {
        name: ['Entity.ITCode', 'ITCode'],
        label: "账号",
        rules:[{ required: true }],
        valueType: WTM_ValueType.textarea,    }
    readonly Password: WTM_EntitiesField = {
        name: ['Entity.Password', 'Password'],
        label: "密码",
        rules:[{ required: true }],
        valueType: WTM_ValueType.textarea,    }
    readonly Name: WTM_EntitiesField = {
        name: ['Entity.Name', 'Name'],
        label: "姓名",
        rules:[{ required: true }],
        valueType: WTM_ValueType.textarea,    }
    readonly IsValid: WTM_EntitiesField = {
        name: ['Entity.IsValid', 'IsValid'],
        label: "是否有效",
        rules:[{ required: true }],
        valueType: WTM_ValueType.switch,    }
    readonly Photo: WTM_EntitiesField = {
        name: ['Entity.Photo', 'Photo'],
        label: "照片",
        valueType: WTM_ValueType.textarea,    }
    readonly Role: WTM_EntitiesField = {
        name: ['Entity.Role', 'Role'],
        label: "角色",
        valueType: WTM_ValueType.textarea,    }
    readonly Group: WTM_EntitiesField = {
        name: ['Entity.Group', 'Group'],
        label: "用户组",
        valueType: WTM_ValueType.textarea,    }
    readonly Email: WTM_EntitiesField = {
        name: ['Entity.Email', 'Email'],
        label: "邮箱",
        valueType: WTM_ValueType.textarea,    }
    readonly Gender: WTM_EntitiesField = {
        name: ['Entity.Gender', 'Gender'],
        label: "性别",
        valueType: WTM_ValueType.textarea,    }
    readonly CellPhone: WTM_EntitiesField = {
        name: ['Entity.CellPhone', 'CellPhone'],
        label: "手机",
        valueType: WTM_ValueType.textarea,    }
    readonly HomePhone: WTM_EntitiesField = {
        name: ['Entity.HomePhone', 'HomePhone'],
        label: "座机",
        valueType: WTM_ValueType.textarea,    }
    readonly ZipCode: WTM_EntitiesField = {
        name: ['Entity.ZipCode', 'ZipCode'],
        label: "邮编",
        valueType: WTM_ValueType.textarea,    }
    readonly Address: WTM_EntitiesField = {
        name: ['Entity.Address', 'Address'],
        label: "地址",
        valueType: WTM_ValueType.textarea,    }
    readonly CreateTime: WTM_EntitiesField = {
        name: ['Entity.CreateTime', 'CreateTime'],
        label: "创建时间",
        valueType: WTM_ValueType.textarea,    }
    readonly UpdateTime: WTM_EntitiesField = {
        name: ['Entity.UpdateTime', 'UpdateTime'],
        label: "修改时间",
        valueType: WTM_ValueType.textarea,    }
    readonly CreateBy: WTM_EntitiesField = {
        name: ['Entity.CreateBy', 'CreateBy'],
        label: "创建人",
        valueType: WTM_ValueType.textarea,    }
    readonly UpdateBy: WTM_EntitiesField = {
        name: ['Entity.UpdateBy', 'UpdateBy'],
        label: "修改人",
        valueType: WTM_ValueType.textarea,    }
}
export const PageEntity = new Entity()

