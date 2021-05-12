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
        key: "MajorCode",
        label: "专业编码"
    },
    {
        key: "MajorName",
        label: "专业名称"
    },
    {
        key: "MajorType",
        label: "专业类别"
    },
    {
        key: "Remark",
        label: "备注"
    },
    {
        key: "SchoolName_view",
        label: "所属学校"
    },
    {
        key: "LoginName_view",
        label: "学生"
    },
  { isOperate: true, label: i18n.t(`table.actions`), actions: ["detail", "edit", "deleted"] } //操作列
];

export const MajorTypeTypes: Array<any> = [
  { Text: "必修", Value: "Required" },
  { Text: "选修", Value: "Optional" }
];

