
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
    readonly Majors: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: 'Majors',
        // label 字段描述
        label: EnumLocaleLabel.Majors,
        valueType: WTM_ValueType.grid
    }
}
export const PageEntity = new Entity()