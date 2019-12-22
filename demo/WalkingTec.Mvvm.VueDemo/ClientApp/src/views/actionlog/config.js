// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export const ASSEMBLIES = ["add", "edit", "delete", "export", "imported"];

export const SEARCH_DATA = {
  ITCode: "",
  ActionUrl: "",
  LogType: "",
  ActionName: "",
  ActionTime: [],
  StartActionTime: "",
  EndActionTime: "",
  IP: "",
  LogType: ""
};

export const TABLE_HEADER = [
  { key: "LogType", label: "类型" },
  { key: "ModuleName", label: "模块" },
  { key: "ActionName", label: "动作" },
  { key: "ITCode", label: "ITCode" },
  { key: "ActionUrl", label: "ActionUrl" },
  { key: "ActionTime", label: "操作时间" },
  { key: "Duration", label: "时长" },
  { key: "IP", label: "IP" },
  { key: "Remark", label: "备注" },
  { key: "operate", label: "操作", isSlot: true }
];
