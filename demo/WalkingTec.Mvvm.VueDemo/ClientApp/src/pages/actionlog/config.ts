import i18n from "@/lang";
// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = ["delete", "export"];

export const TABLE_HEADER: Array<object> = [
  { key: "LogType", label: i18n.t(`actionlog.LogType`) },
  { key: "ModuleName", label: i18n.t(`actionlog.ModuleName`) },
  { key: "ActionName", label: i18n.t(`actionlog.ActionName`) },
  { key: "ITCode", label: i18n.t(`actionlog.ITCode`) },
  { key: "ActionUrl", label: i18n.t(`actionlog.ActionUrl`) },
  { key: "ActionTime", label: i18n.t(`actionlog.ActionTime`) },
  { key: "Duration", label:i18n.t(`actionlog.Duration`) },
  { key: "IP", label: "IP" },
  { key: "Remark", label: i18n.t(`actionlog.Remark`) },
  { isOperate: true, label: i18n.t(`table.actions`), actions: ["detail", "deleted"] } //操作列
];

export const logTypes: Array<any> = [
  { Value: 0, Text: i18n.t(`actionlog.Ordinary`) },
  { Value: 1, Text: i18n.t(`actionlog.Abnormal`) },
  { Value: 2, Text: i18n.t(`actionlog.Debugging`) }
];
