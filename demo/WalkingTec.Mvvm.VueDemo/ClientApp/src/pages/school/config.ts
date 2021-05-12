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
        key: "SchoolCode",
        label: "学校编码"
    },
    {
        key: "SchoolName",
        label: "学校名称"
    },
    {
        key: "FileId",
        label: "文件",
        isSlot: true 
    },
    {
        key: "SchoolType",
        label: "学校类型"
    },
    {
        key: "Remark",
        label: "备注"
    },
  { isOperate: true, label: i18n.t(`table.actions`), actions: ["detail", "edit", "deleted"] } //操作列
];

export const SchoolTypeTypes: Array<any> = [
  { Text: "公立学校", Value: "PUB" },
  { Text: "私立学校", Value: "PRI" }
];

