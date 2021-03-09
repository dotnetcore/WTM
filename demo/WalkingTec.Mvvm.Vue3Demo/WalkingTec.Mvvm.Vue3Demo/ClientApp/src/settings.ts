import { getSettings } from "@/util/cookie";

interface ISettings {
  title: string; // Overrides the default title
  theme: string;
  showSettings: boolean; // Controls settings panel display
  showTagsView: boolean; // Controls tagsview display
  showSidebarLogo: boolean; // Controls siderbar logo display
  fixedHeader: boolean; // If true, will fix the header component
  errorLog: string[]; // The env to enable the errorlog component, default 'production' only
  sidebarTextTheme: boolean; // If true, will change active text color for sidebar based on theme
  devServerPort: number; // Port number for webpack-dev-server
  mockServerPort: number; // Port number for mock server
  isDialog: boolean;
  menuBackgroundImg: any;
}
const obj = getSettings() || {};

const settings: ISettings = {
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
