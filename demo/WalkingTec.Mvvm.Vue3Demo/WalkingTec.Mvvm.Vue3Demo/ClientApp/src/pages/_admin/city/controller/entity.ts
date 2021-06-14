
import { $i18n, FieldRequest, WTM_EntitiesField, WTM_ValueType } from '@/client';
import { EnumLocaleLabel } from '../locales';

/**
 * 页面实体
 */
class Entity {
    /**
     * 备注预留
     * @type {WTM_EntitiesField}
     * @memberof Entity
     */
    readonly ID: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'ID'],
        // label 字段描述
        label: EnumLocaleLabel.ID,
    }
    readonly Name: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'Name'],
        // label 字段描述
        label: EnumLocaleLabel.Name,
    }
    readonly Level: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'Level'],
        // label 字段描述
        label: EnumLocaleLabel.Level,
    }
    readonly ParentId: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'ParentId'],
        // label 字段描述
        label: EnumLocaleLabel.ParentId,
        valueType: WTM_ValueType.select,
        request: async () => FieldRequest('/api/City/GetCitys'),
        // rules: [{ required: true }]
    }
    readonly ParentId_Filter: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: 'ParentId',
        // label 字段描述
        label: EnumLocaleLabel.ParentId,
    }
}
export const PageEntity = new Entity()