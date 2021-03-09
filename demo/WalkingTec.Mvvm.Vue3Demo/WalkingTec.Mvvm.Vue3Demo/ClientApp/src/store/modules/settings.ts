import {
  VuexModule,
  Module,
  Mutation,
  Action,
  getModule
} from "vuex-module-decorators";
import store from "@/store/modules/index";
import { style as variables } from "@/config/index";
import { setSettings, getSettings } from "@/util/cookie";
import defaultSettings from "@/settings";

export interface ISettingsState {
  title: string;
  theme: string;
  fixedHeader: boolean;
  showSettings: boolean;
  showTagsView: boolean;
  showSidebarLogo: boolean;
  sidebarTextTheme: boolean;
  isDialog: boolean;
  menuBackgroundImg: any;
}

@Module({ dynamic: true, store, name: "settings" })
class Settings extends VuexModule implements ISettingsState {

  public title = defaultSettings.title;
  public theme = variables.theme;
  public fixedHeader = defaultSettings.fixedHeader;
  public showSettings = defaultSettings.showSettings;
  public showTagsView = defaultSettings.showTagsView;
  public showSidebarLogo = defaultSettings.showSidebarLogo;
  public sidebarTextTheme = defaultSettings.sidebarTextTheme;
  public isDialog = defaultSettings.isDialog;
  public menuBackgroundImg = defaultSettings.menuBackgroundImg;

  @Mutation
  private CHANGE_SETTING(payload: { key: string; value: any }) {
    const { key, value } = payload;
    if (Object.prototype.hasOwnProperty.call(this, key)) {
      (this as any)[key] = value;
      const obj = getSettings();
      console.log('{...obj, [key]: value}', {...obj, [key]: value});
      setSettings({...obj, [key]: value});
    }
  }

  @Action
  public ChangeSetting(payload: { key: string; value: any }) {
    this.CHANGE_SETTING(payload);
  }
}

export const SettingsModule = getModule(Settings);
