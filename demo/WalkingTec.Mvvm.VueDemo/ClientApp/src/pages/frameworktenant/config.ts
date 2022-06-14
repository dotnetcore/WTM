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
        label:  i18n.t("frameworktenant.TCode")
    },
    {
        key: "TName",
        label:  i18n.t("frameworktenant.TName")
    },
    {
        key: "TDb",
        label:  i18n.t("frameworktenant.TDb")
    },
    {
        key: "TDbType",
        label:  i18n.t("frameworktenant.TDbType")
    },
    {
        key: "DbContext",
        label:  i18n.t("frameworktenant.DbContext")
    },
    {
        key: "TDomain",
        label:  i18n.t("frameworktenant.TDomain")
    },
    {
        key: "EnableSub",
        label:  i18n.t("frameworktenant.EnableSub"),
        isSlot: true 
    },
    {
        key: "Enabled",
        label:  i18n.t("frameworktenant.Enabled"),
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

