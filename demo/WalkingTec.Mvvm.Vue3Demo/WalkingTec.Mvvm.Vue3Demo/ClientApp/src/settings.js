import { getSettings } from "@/util/cookie";
var obj = getSettings() || {};
var settings = {
    title: obj.title || "WTM",
    theme: obj.theme,
    fixedHeader: obj.fixedHeader === undefined ? true : obj.fixedHeader,
    showSettings: obj.showSettings === undefined ? true : obj.showSettings,
    showTagsView: obj.showTagsView === undefined ? true : obj.showTagsView,
    showSidebarLogo: obj.showSidebarLogo === undefined ? true : obj.showSidebarLogo,
    errorLog: ["production"],
    sidebarTextTheme: obj.sidebarTextTheme === undefined ? true : obj.sidebarTextTheme,
    isDialog: obj.isDialog === undefined ? true : obj.isDialog,
    devServerPort: 9527,
    mockServerPort: 9528,
    menuBackgroundImg: obj.menuBackgroundImg || {
        image: require("static/img/sidebar-2.jpg"),
        key: "sidebar-2"
    }
};
export default settings;
//# sourceMappingURL=settings.js.map