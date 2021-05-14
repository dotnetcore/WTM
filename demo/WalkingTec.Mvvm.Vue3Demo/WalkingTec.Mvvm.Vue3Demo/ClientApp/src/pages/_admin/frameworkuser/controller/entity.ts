
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
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly ITCode: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'ITCode'],
        // label 字段描述
        label: EnumLocaleLabel.ITCode,
        // 输入框提示 非必填 默认如下
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.ITCode),
        // 校验规则
        rules: [{ required: true }],
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly ITCode_Filter: WTM_EntitiesField = {
        name: 'ITCode',
        label: EnumLocaleLabel.ITCode,
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly Password: WTM_EntitiesField = {
        name: ['Entity', 'Password'],
        label: EnumLocaleLabel.Password,
        valueType: WTM_ValueType.password,
        rules: [{ required: true }],
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly Email: WTM_EntitiesField = {
        name: ['Entity', 'Email'],
        label: EnumLocaleLabel.Email,
        // rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Email) }],
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly Name: WTM_EntitiesField = {
        name: ['Entity', 'Name'],
        label: EnumLocaleLabel.Name,
        rules: [{ required: true }],
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly Name_Filter: WTM_EntitiesField = {
        name: 'Name',
        label: EnumLocaleLabel.Name,
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly Gender: WTM_EntitiesField = {
        name: ['Entity', 'Gender'],
        label: EnumLocaleLabel.Sex,
        valueType: WTM_ValueType.radio,
        // 远程数据
        request: async (formState) => {
            return [
                { label: $i18n.t(EnumLocaleLabel.Sex_Male), value: 'Male' },
                { label: $i18n.t(EnumLocaleLabel.Sex_Female), value: 'Female' }
            ]
        },
        rules: [{ required: true }],
    }
    readonly CellPhone: WTM_EntitiesField = {
        name: ['Entity', 'CellPhone'],
        label: EnumLocaleLabel.CellPhone,
    }
    readonly HomePhone: WTM_EntitiesField = {
        name: ['Entity', 'HomePhone'],
        label: EnumLocaleLabel.HomePhone,
    }
    readonly Address: WTM_EntitiesField = {
        name: ['Entity', 'Address'],
        label: EnumLocaleLabel.Address,
    }
    readonly ZipCode: WTM_EntitiesField = {
        name: ['Entity', 'ZipCode'],
        label: EnumLocaleLabel.ZipCode,
    }
    readonly PhotoId: WTM_EntitiesField = {
        name: ['Entity', 'PhotoId'],
        valueType: WTM_ValueType.image,
        label: EnumLocaleLabel.Photo,
    }
    readonly IsValid: WTM_EntitiesField = {
        name: ['Entity', 'IsValid'],
        valueType: WTM_ValueType.switch,
        label: EnumLocaleLabel.IsValid,
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly SelectedRolesCodes: WTM_EntitiesField = {
        name: 'SelectedRolesCodes',
        label: EnumLocaleLabel.RoleName,
        valueType: WTM_ValueType.transfer,
        request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkRoles'),
        // rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.RoleName), type: "array" }],
    }
    /**
    * 备注预留
    * @type {WTM_EntitiesField}
    * @memberof Entity
    */
    readonly SelectedGroupCodes: WTM_EntitiesField = {
        name: 'SelectedGroupCodes',
        label: EnumLocaleLabel.GroupName,
        valueType: WTM_ValueType.transfer,
        request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkGroups'),
        // rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.GroupName), type: "array" }],
    }
}
export const PageEntity = new Entity()