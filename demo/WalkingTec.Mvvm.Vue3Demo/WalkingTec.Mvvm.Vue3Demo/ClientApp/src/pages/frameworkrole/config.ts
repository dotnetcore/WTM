import i18n from "@/lang";
// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = [
  "add",
  "edit",
  "delete",
  "export",
  "imported"
];

export const TABLE_HEADER: Array<object> = [
  { key: "RoleCode", sortable: true, label: i18n.t(`frameworkrole.RoleCode`) },
  { key: "RoleName", sortable: true, label: i18n.t(`frameworkrole.RoleName`) },
  { key: "RoleRemark", sortable: true, label: i18n.t(`frameworkrole.RoleRemark`) },
  { key: "operate", label: i18n.t(`table.actions`), isSlot: true }
];
