
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
    readonly ITCode: WTM_EntitiesField = {
        name: ['Entity', 'ITCode'],
        label: EnumLocaleLabel.ITCode,
    }
    readonly ITCode_Filter: WTM_EntitiesField = {
        name: 'ITCode',
        label: EnumLocaleLabel.ITCode,
    }
    readonly ActionUrl: WTM_EntitiesField = {
        name: ['Entity', 'ActionUrl'],
        label: EnumLocaleLabel.ActionUrl,
    }
    readonly ActionUrl_Filter: WTM_EntitiesField = {
        name: 'ActionUrl',
        label: EnumLocaleLabel.ActionUrl,
    }
    readonly ActionTime: WTM_EntitiesField = {
        name: ['Entity', 'ActionTime'],
        label: EnumLocaleLabel.ActionTime,
    }
    readonly ActionTime_Filter: WTM_EntitiesField = {
        name: 'ActionTime',
        label: EnumLocaleLabel.ActionTime,
        valueType: WTM_ValueType.dateRange
    }
    readonly IP: WTM_EntitiesField = {
        name: ['Entity', 'IP'],
        label: EnumLocaleLabel.IP,
    }
    readonly IP_Filter: WTM_EntitiesField = {
        name: 'IP',
        label: EnumLocaleLabel.IP,
    }
    readonly LogType: WTM_EntitiesField = {
        name: ['Entity', 'LogType'],
        label: EnumLocaleLabel.LogType,
    }
    readonly LogType_Filter: WTM_EntitiesField = {
        name: 'LogType',
        label: EnumLocaleLabel.LogType,
    }
}
export const PageEntity = new Entity()