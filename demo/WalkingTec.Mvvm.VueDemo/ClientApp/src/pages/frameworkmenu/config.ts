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
  { key: "PageName", sortable: true, label: i18n.t(`frameworkmenu.PageName`), align: "left" },
  { key: "DisplayOrder", sortable: true, label: i18n.t(`frameworkmenu.DisplayOrder`) },
  { key: "Icon", sortable: true, label: i18n.t(`frameworkmenu.Icon`), isSlot: true },
  { key: "FolderOnly", sortable: true, label: i18n.t(`frameworkmenu.Directory`), isSlot: true },
  { key: "ShowOnMenu", sortable: true, label: i18n.t(`frameworkmenu.ShowOnMenu`),isSlot: true },
  { key: "IsPublic", sortable: true, label: i18n.t(`frameworkmenu.Public`),  isSlot: true },
  { key: "TenantAllowed", sortable: true, label: i18n.t(`frameworkmenu.TenantAllowed`), isSlot: true },
  { isOperate: true, label: i18n.t(`table.actions`), actions: ["detail", "edit", "deleted"] } //操作列
];

