
import { $i18n, WTM_EntitiesField } from '@/client';
import { EnumLocaleLabel } from '../locales';

/**
 * 页面实体
 */
class Entity {
    ITCode: WTM_EntitiesField = {
        // form 的 name 属性 解析为 Entity.ITCode
        name: ['Entity', 'ITCode'],
        // label 字段描述
        label: EnumLocaleLabel.ITCode,
        // 输入框提示
        placeholder: $i18n.toPlaceholder(EnumLocaleLabel.ITCode),
        // 校验规则
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.ITCode) }],
    }
    Password: WTM_EntitiesField = {
        name: ['Entity', 'Password'],
        label: EnumLocaleLabel.Password,
        placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Password),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Password) }],
    }
    Email: WTM_EntitiesField = {
        name: ['Entity', 'Email'],
        label: EnumLocaleLabel.Email,
        placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Email),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Email) }],
    }
    Name: WTM_EntitiesField = {
        name: ['Entity', 'Name'],
        label: EnumLocaleLabel.Name,
        placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Name),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Name) }],
    }
    Gender: WTM_EntitiesField = {
        name: ['Entity', 'Gender'],
        label: EnumLocaleLabel.Sex,
        placeholder: $i18n.toPlaceholder(EnumLocaleLabel.Sex),
        // 字段 输入类型 radio表现 单选按钮
        valueType: "radio",
        // 远程数据
        request: async () => [
            { label: $i18n.t(EnumLocaleLabel.Sex_Male), value: 'Male' },
            { label: $i18n.t(EnumLocaleLabel.Sex_Female), value: 'Female' }
        ],
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.Sex) }],
    }
    SelectedGroupCodes: WTM_EntitiesField = {
        name: 'SelectedGroupCodes',
        label: EnumLocaleLabel.GroupName,
        placeholder: $i18n.toPlaceholder(EnumLocaleLabel.GroupName),
        // 字段 输入类型 radio表现 单选按钮
        valueType: "checkbox",
        // 远程数据
        // request: async () => Ajax.get('/api/_FrameworkUserBase/GetFrameworkRoles'),
        rules: [{ required: true, message: $i18n.toRulesMessage(EnumLocaleLabel.GroupName) }],
    }
}
export const PageEntity = new Entity()