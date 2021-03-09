import i18n from "@/lang";
// 页面中，需要展示的动作按钮；增，改，删，导入，导出
export var ASSEMBLIES = [
    "add",
    "edit",
    "delete",
    "export",
    "imported"
];
export var TABLE_HEADER = [
    { key: "PageName", sortable: true, label: i18n.t("frameworkmenu.PageName"), align: "left" },
    { key: "DisplayOrder", sortable: true, label: i18n.t("frameworkmenu.DisplayOrder") },
    { key: "Icon", sortable: true, label: i18n.t("frameworkmenu.Icon"), isSlot: true },
    { isOperate: true, label: i18n.t("table.actions"), actions: ["detail", "edit", "deleted"] } //操作列
];
//# sourceMappingURL=config.js.map