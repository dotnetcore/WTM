// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = [
  "add",
  "edit",
  "delete",
  "export",
  "imported"
];

export const SEARCH_DATA = {
  menuCode: "",
  menuName: ""
};

export const TABLE_HEADER: Array<object> = [
  { key: "PageName", sortable: true, label: "页面名称", align: "left" },
  { key: "DisplayOrder", sortable: true, label: "顺序" },
  { key: "ICon", sortable: true, label: "图标" },
  { key: "operate", label: "操作", isSlot: true }
];
