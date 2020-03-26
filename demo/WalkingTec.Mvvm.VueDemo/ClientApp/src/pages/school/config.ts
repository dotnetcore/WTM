// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = [
  "add",
  "edit",
  "delete",
  "export",
  "imported"
];
// 列表
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
  { isOperate: true, label: "操作", actions: ["detail", "edit", "deleted"] } //操作列
];

export const SchoolTypeTypes: Array<any> = [
  { Text: "公立学校", Value: 0 },
  { Text: "私立学校", Value: 1 }
];

