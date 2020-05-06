// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = [
  "add",
  "edit",
  "delete",
  "export",
  "imported"
];

export const TABLE_HEADER: Array<object> = [
    { key: "RoleCode", sortable: "custom", label: "角色编号" },
    { key: "RoleName", sortable: "custom", label: "角色姓名" },
    { key: "RoleRemark", sortable: "custom", label: "备注" },
  { key: "operate", label: "操作", isSlot: true }
];
