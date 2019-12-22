// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES = ["add", "edit", "delete", "export", "imported"];

export const SEARCH_DATA = {
  RoleCode: "",
  RoleName: ""
};

export const TABLE_HEADER = [
  { key: "RoleCode", sortable: true, label: "角色编号" },
  { key: "RoleName", sortable: true, label: "角色姓名" },
  { key: "RoleRemark", sortable: true, label: "备注" },
  { key: "operate", label: "操作", isSlot: true }
];
