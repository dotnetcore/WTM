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
    { key: "RoleCode", sortable: true, label: i18n.t("frameworkrole.RoleCode") },
    { key: "RoleName", sortable: true, label: i18n.t("frameworkrole.RoleName") },
    { key: "RoleRemark", sortable: true, label: i18n.t("frameworkrole.RoleRemark") },
    { key: "operate", label: i18n.t("table.actions"), isSlot: true }
];
//# sourceMappingURL=config.js.map