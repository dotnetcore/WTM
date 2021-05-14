
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

}
export const PageEntity = new Entity()