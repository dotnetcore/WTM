
import { $i18n, WTM_EntitiesField, WTM_ValueType, FieldRequest } from '@/client';
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
    readonly GroupCode: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'GroupCode'],
        // label 字段描述
        label: EnumLocaleLabel.GroupCode,
        rules: [{ required: true }],
    }
    readonly GroupCode_Filter: WTM_EntitiesField = {
        name: 'GroupCode',
        label: EnumLocaleLabel.GroupCode,
    }
    readonly GroupName: WTM_EntitiesField = {
        name: ['Entity', 'GroupName'],
        label: EnumLocaleLabel.GroupName,
        rules: [{ required: true }],
    }
    readonly GroupName_Filter: WTM_EntitiesField = {
        name: 'GroupName',
        label: EnumLocaleLabel.GroupName,
    }
    readonly GroupRemark: WTM_EntitiesField = {
        name: ['Entity', 'GroupRemark'],
        label: EnumLocaleLabel.GroupRemark,
    }

}
export const PageEntity = new Entity()