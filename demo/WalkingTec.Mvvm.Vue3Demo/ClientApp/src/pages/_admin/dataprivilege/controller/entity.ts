
import { $i18n, WTM_EntitiesField, WTM_ValueType, FieldRequest } from '@/client';
import lodash from 'lodash';
import { EnumLocaleLabel } from '../locales';

/**
 * 页面实体
 */
class Entity {
    readonly ID: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'ID'],
        // label 字段描述
        label: EnumLocaleLabel.ID,
    }
    readonly DpType: WTM_EntitiesField = {
        name: 'DpType',
        label: EnumLocaleLabel.DpType,
        valueType: WTM_ValueType.radio,
        request: this.requestDpType,
        rules: [{ required: true }]
    }
    readonly DpType_Filter: WTM_EntitiesField = {
        name: 'DpType',
        label: EnumLocaleLabel.DpType,
        valueType: WTM_ValueType.radio,
        request: this.requestDpType,
    }
    readonly TableName: WTM_EntitiesField = {
        name: ['Entity', 'TableName'],
        label: EnumLocaleLabel.TableName,
        valueType: WTM_ValueType.select,
        request: async () => FieldRequest('/api/_DataPrivilege/GetPrivileges'),
        rules: [{ required: true }]
    }
    readonly TableName_Filter: WTM_EntitiesField = {
        name: 'TableName',
        label: EnumLocaleLabel.TableName,
        valueType: WTM_ValueType.select,
        request: async () => FieldRequest('/api/_DataPrivilege/GetPrivileges'),
    }
    readonly SelectedItemsID: WTM_EntitiesField = {
        name: 'SelectedItemsID',
        linkage: ['Entity.TableName'],
        valueType: WTM_ValueType.select,
        request: async (formState) => {
            const table = lodash.get(formState, 'Entity.TableName');
            return table ? FieldRequest({ url: '/api/_DataPrivilege/GetPrivilegeByTableName', body: { table } }) : []
        },
        label: EnumLocaleLabel.SelectedItemsID,
    }
    readonly IsAll: WTM_EntitiesField = {
        name: 'IsAll',
        label: EnumLocaleLabel.IsAll,
        valueType: WTM_ValueType.radio,
        request: async () => {
            return [
                { label: $i18n.t(EnumLocaleLabel.IsAll_true), value: true },
                { label: $i18n.t(EnumLocaleLabel.IsAll_false), value: false }
            ]
        },
        rules: [{ required: true, type: 'boolean' }]
    }
    readonly UserCode: WTM_EntitiesField = {
        name: ['Entity', 'UserCode'],
        label: EnumLocaleLabel.UserCode,
        rules: [{ required: true }]
    }
    readonly GroupCode: WTM_EntitiesField = {
        name: ['Entity', 'GroupCode'],
        label: EnumLocaleLabel.GroupCode,
        valueType: WTM_ValueType.select,
        request: async () => FieldRequest('/api/_DataPrivilege/GetUserGroups'),
        rules: [{ required: true }]
    }
    async requestDpType() {
        return [
            { label: $i18n.t(EnumLocaleLabel.DpType_0), value: 'UserGroup' },
            { label: $i18n.t(EnumLocaleLabel.DpType_1), value: 'User' }
        ]
    }
}
export const PageEntity = new Entity()