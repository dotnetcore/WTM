// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = ["add", "edit", "delete", "export"];

export const TABLE_HEADER: Array<object> = [
  { key: "Name", sortable: true, label: "授权对象" },
  { key: "TableName", sortable: true, label: "权限名称" },
  { key: "RelateIDs", sortable: true, label: "权限" },
  { isOperate: true, label: "操作", actions: ["detail", "edit", "deleted"] } //操作列
];
