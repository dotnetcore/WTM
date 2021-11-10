
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
    readonly RoleCode: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'RoleCode'],
        // label 字段描述
        label: EnumLocaleLabel.RoleCode,
        rules: [{ required: true }],
    }
    readonly RoleCode_Filter: WTM_EntitiesField = {
        name: 'RoleCode',
        label: EnumLocaleLabel.RoleCode,
    }
    readonly RoleName: WTM_EntitiesField = {
        name: ['Entity', 'RoleName'],
        label: EnumLocaleLabel.RoleName,
        rules: [{ required: true }],
    }
    readonly RoleName_Filter: WTM_EntitiesField = {
        name: 'RoleName',
        label: EnumLocaleLabel.RoleName,
    }
    readonly RoleRemark: WTM_EntitiesField = {
        name: ['Entity', 'RoleRemark'],
        label: EnumLocaleLabel.RoleRemark,
    }

}
export const PageEntity = new Entity()