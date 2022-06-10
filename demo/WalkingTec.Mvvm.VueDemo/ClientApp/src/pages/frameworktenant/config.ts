import i18n from "@/lang";

export const ASSEMBLIES: Array<string> = [
  "add",
  "edit",
  "delete",
  "export",
  "imported"
];

export const TABLE_HEADER: Array<object> = [


    {
        key: "TCode",
        label: "租户编号"
    },
    {
        key: "TName",
        label: "租户名称"
    },
    {
        key: "TDb",
        label: "租户数据库"
    },
    {
        key: "TDbType",
        label: "数据库类型"
    },
    {
        key: "DbContext",
        label: "数据库架构"
    },
    {
        key: "TDomain",
        label: "租户域名"
    },
    {
        key: "TenantCode",
        label: "租户"
    },
    {
        key: "EnableSub",
        label: "允许子租户",
        isSlot: true 
    },
    {
        key: "Enabled",
        label: "启用",
        isSlot: true 
    },
  { isOperate: true, label: i18n.t(`table.actions`), actions: ["detail", "edit", "deleted"] } //操作列
];

export const TDbTypeTypes: Array<any> = [
  { Text: "SqlServer", Value: "SqlServer" },
  { Text: "MySql", Value: "MySql" },
  { Text: "PgSql", Value: "PgSql" },
  { Text: "Memory", Value: "Memory" },
  { Text: "SQLite", Value: "SQLite" },
  { Text: "Oracle", Value: "Oracle" }
];

