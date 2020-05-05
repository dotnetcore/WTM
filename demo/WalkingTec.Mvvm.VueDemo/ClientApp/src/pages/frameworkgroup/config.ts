// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = [
  "add",
  "edit",
  "delete",
  "export",
  "imported"
];

export const TABLE_HEADER: Array<object> = [
    { key: "GroupCode", sortable: "custom", label: "用户组编码" },
    { key: "GroupName", sortable: "custom", label: "用户组名称" },
    { key: "GroupRemark", sortable: "custom", label: "备注" },
  { isOperate: true, label: "操作", actions: ["detail", "edit", "deleted"] } //操作列
];
