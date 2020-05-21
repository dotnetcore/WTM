import i18n from "@/lang";
// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = ["add", "edit", "delete", "export"];

export const TABLE_HEADER: Array<object> = [
  { key: "Name", sortable: true, label: i18n.t(`dataprivilege.Name`) }, // "授权对象"
  { key: "TableName", sortable: true, label: i18n.t(`dataprivilege.TableName`) },// "权限名称"
  { key: "RelateIDs", sortable: true, label: i18n.t(`dataprivilege.RelateIDs`) }, // "权限"
  { isOperate: true, label: i18n.t(`table.actions`), actions: ["detail", "edit", "deleted"] } //操作列
];
