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
        key: "LoginName",
        label: "账号"
    },
    {
        key: "Password",
        label: "密码"
    },
    {
        key: "Email",
        label: "邮箱"
    },
    {
        key: "Name",
        label: "姓名"
    },
    {
        key: "Sex",
        label: "性别"
    },
    {
        key: "CellPhone",
        label: "手机"
    },
    {
        key: "Address",
        label: "住址"
    },
    {
        key: "ZipCode",
        label: "邮编"
    },
    {
        key: "PhotoId",
        label: "照片",
        isSlot: true 
    },
    {
        key: "IsValid",
        label: "是否有效",
        isSlot: true 
    },
    {
        key: "EnRollDate",
        label: "日期"
    },
    {
        key: "MajorName_view",
        label: "专业"
    },
  { isOperate: true, label: i18n.t(`table.actions`), actions: ["detail", "edit", "deleted"] } //操作列
];

export const SexTypes: Array<any> = [
  { Text: "男", Value: "Male" },
  { Text: "女", Value: "Female" }
];

