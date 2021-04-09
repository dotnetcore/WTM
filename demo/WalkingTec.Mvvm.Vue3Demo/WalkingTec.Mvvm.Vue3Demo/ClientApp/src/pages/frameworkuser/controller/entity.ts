
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
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'ITCode'],
        // label 字段描述
        label: EnumLocaleLabel.ITCode,
        // 输入框提示 非必填 默认如下
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.ITCode),
        // 校验规则
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.ITCode) }],
    }
    readonly ITCode_Filter: WTM_EntitiesField = {
        name: 'ITCode',
        label: EnumLocaleLabel.ITCode,
    }
    readonly Password: WTM_EntitiesField = {
        name: ['Entity', 'Password'],
        label: EnumLocaleLabel.Password,
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Password),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Password) }],
    }
    readonly Email: WTM_EntitiesField = {
        name: ['Entity', 'Email'],
        label: EnumLocaleLabel.Email,
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Email),
        valueType: WTM_ValueType.textarea,
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Email) }],
    }
    readonly Name: WTM_EntitiesField = {
        name: ['Entity', 'Name'],
        label: EnumLocaleLabel.Name,
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Name),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Name) }],
    }
    readonly Name_Filter: WTM_EntitiesField = {
        name: 'Name',
        label: EnumLocaleLabel.Name,
    }
    readonly Gender: WTM_EntitiesField = {
        name: ['Entity', 'Gender'],
        label: EnumLocaleLabel.Sex,
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Sex),
        valueType: WTM_ValueType.radio,
        // 远程数据
        request: async (formState) => {
            return [
                { label: $i18n.t(EnumLocaleLabel.Sex_Male), value: 'Male' },
                { label: $i18n.t(EnumLocaleLabel.Sex_Female), value: 'Female' }
            ]
        },
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Sex) }],
    }
    readonly SelectedRolesCodes: WTM_EntitiesField = {
        name: 'SelectedRolesCodes',
        label: EnumLocaleLabel.RoleName,
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.RoleName),
        valueType: WTM_ValueType.checkbox,
        request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkRoles'),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.RoleName), type: "array" }],
    }
    readonly SelectedGroupCodes: WTM_EntitiesField = {
        name: 'SelectedGroupCodes',
        label: EnumLocaleLabel.GroupName,
        // placeholder: $i18n.toPlaceholder(EnumLocaleLabel.GroupName),
        valueType: WTM_ValueType.checkbox,
        request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkGroups'),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.GroupName), type: "array" }],
    }
}
export const PageEntity = new Entity()