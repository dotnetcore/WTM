import i18n from "@/lang";
// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export var ASSEMBLIES = ["add", "edit", "delete", "export"];
export var TABLE_HEADER = [
    { key: "Name", sortable: true, label: i18n.t("dataprivilege.Name") },
    { key: "PName", sortable: true, label: i18n.t("dataprivilege.TableName") },
    { key: "RelateIDs", sortable: true, label: i18n.t("dataprivilege.RelateIDs") },
    { isOperate: true, label: i18n.t("table.actions"), actions: ["detail", "edit", "deleted"] } //操作列
];
//# sourceMappingURL=config.js.map