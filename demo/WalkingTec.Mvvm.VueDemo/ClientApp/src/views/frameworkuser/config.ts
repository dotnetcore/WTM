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
  { key: "ITCode", sortable: true, label: "账号" },
  { key: "Name", sortable: true, label: "姓名" },
  { key: "Sex", sortable: true, label: "性别" },
  { key: "PhotoId", label: "照片", isSlot: true },
  { key: "IsValid", label: "是否生效", isSlot: true },
  { key: "RoleName_view", label: "角色" },
  { key: "GroupName_view", label: "用户组" },
  { isOperate: true, label: "操作", actions: ["detail", "edit", "deleted"] } //操作列
];
