
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
    readonly SchoolCode: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'SchoolCode'],
        // label 字段描述
        label: EnumLocaleLabel.SchoolCode,
        rules: [{ required: true }]
    }
    readonly SchoolName: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'SchoolName'],
        // label 字段描述
        label: EnumLocaleLabel.SchoolName,
        rules: [{ required: true }]

    }
    readonly SchoolType: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'SchoolType'],
        // label 字段描述
        label: EnumLocaleLabel.SchoolType,
        rules: [{ required: true }]
    }


    readonly Photos: WTM_EntitiesField = {
        name: ['Entity','Photos'],
        label: '上传图片',
        valueType: WTM_ValueType.image,
        fieldProps: { max: 9 },
    }

     readonly Files: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'Files'],
        valueType: WTM_ValueType.upload,
        // label 字段描述
        label: EnumLocaleLabel.Files,
        fieldProps: { max: 9 }
    }

    readonly Remark: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'Remark'],
        // label 字段描述
        label: EnumLocaleLabel.Remark,
        rules: [{ required: true }]

    }
    readonly Majors: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'Majors'],
        // label 字段描述
        label: EnumLocaleLabel.Majors,
        valueType: WTM_ValueType.grid
    }
}
export const PageEntity = new Entity()