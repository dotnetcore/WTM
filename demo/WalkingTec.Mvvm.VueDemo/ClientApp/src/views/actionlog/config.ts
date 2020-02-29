// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES: Array<string> = ["delete", "export"];

export const SEARCH_DATA = {
  ITCode: "",
  ActionUrl: "",
  LogType: [],
  ActionName: "",
  ActionTime: [],
  StartActionTime: "",
  EndActionTime: "",
  IP: ""
};

export const TABLE_HEADER: Array<object> = [
  { key: "LogType", label: "类型" },
  { key: "ModuleName", label: "模块" },
  { key: "ActionName", label: "动作" },
  { key: "ITCode", label: "ITCode" },
  { key: "ActionUrl", label: "ActionUrl" },
  { key: "ActionTime", label: "操作时间" },
  { key: "Duration", label: "时长" },
  { key: "IP", label: "IP" },
  { key: "Remark", label: "备注" },
  { isOperate: true, label: "操作", actions: ["detail", "deleted"] } //操作列
];

export const logTypes: Array<any> = [
  { Value: 0, Text: "普通" },
  { Value: 1, Text: "异常" },
  { Value: 2, Text: "调试" }
];
