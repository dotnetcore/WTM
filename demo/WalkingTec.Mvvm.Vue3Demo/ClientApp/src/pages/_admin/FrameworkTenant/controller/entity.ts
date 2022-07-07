
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
    readonly TCode: WTM_EntitiesField = {
        name: ['Entity', 'TCode'],
        // label 租户编号
        label: EnumLocaleLabel.TCode,
        rules: [{ required: true }],
    }
    readonly TName: WTM_EntitiesField = {
        name: ['Entity', 'TName'],
        // label 租户名称
        label: EnumLocaleLabel.TName,
        rules: [{ required: true }],
    }
    readonly TDb: WTM_EntitiesField = {
        name: ['Entity', 'TDb'],
        // label 租户数据库
        label: EnumLocaleLabel.TDb,
    }
    readonly TDbType: WTM_EntitiesField = {
        name: ['Entity', 'TDbType'],
        // label 数据库类型
        label: EnumLocaleLabel.TDbType,
        valueType: WTM_ValueType.select,
        request: async (formState) => {
            return [
                { label:'SqlServer', value: 'SqlServer' },
                { label:'MySql', value: 'MySql' },
                { label:'PgSql', value: 'PgSql' },
                { label:'Memory', value: 'Memory' },
                { label:'SQLite', value: 'SQLite' },
                { label:'Oracle', value: 'Oracle' },
            ]
        },
    }
    readonly DbContext: WTM_EntitiesField = {
        name: ['Entity', 'DbContext'],
        // label 数据库架构
        label: EnumLocaleLabel.DbContext
    }
    readonly TDomain: WTM_EntitiesField = {
        name: ['Entity', 'TDomain'],
        // label 租户域名
        label: EnumLocaleLabel.TDomain
    }
    readonly EnableSub: WTM_EntitiesField = {
        name: ['Entity', 'EnableSub'],
        // label 允许子租户
        label: EnumLocaleLabel.EnableSub,
        valueType: WTM_ValueType.switch,
        rules: [{ required: true }],
    }
    readonly Enabled: WTM_EntitiesField = {
        name: ['Entity', 'Enabled'],
        // label 启用
        label: EnumLocaleLabel.Enabled,
        valueType: WTM_ValueType.switch,
        rules: [{ required: true }],
    }
    readonly AdminRoleCode: WTM_EntitiesField = {
        name: ['AdminRoleCode'],
        // label 启用
        label: EnumLocaleLabel.AdminRoleCode,
        valueType: WTM_ValueType.select,
        request: async () => FieldRequest('/api/_account/GetFrameworkRoles'),
        rules: [{ required: true }],
    }
}
export const PageEntity = new Entity()
