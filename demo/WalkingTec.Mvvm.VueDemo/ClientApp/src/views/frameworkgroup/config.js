// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES = ["add", "edit", "delete", "export", "imported"];

export const SEARCH_DATA = {
  GroupCode: "",
  GroupName: ""
};

export const TABLE_HEADER = [
  { key: "GroupCode", sortable: true, label: "用户组编码" },
  { key: "GroupName", sortable: true, label: "用户组名称" },
  { key: "GroupRemark", sortable: true, label: "备注" },
  { key: "operate", label: "操作", isSlot: true }
];
