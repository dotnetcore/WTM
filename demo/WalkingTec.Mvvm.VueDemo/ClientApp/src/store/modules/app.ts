import {
  VuexModule,
  Module,
  Mutation,
  Action,
  getModule
} from "vuex-module-decorators";
import {
  getSidebarStatus,
  getSize,
  setSidebarStatus,
  setLanguage,
  setSize
} from "@/util/cookie";
import { getLocale } from "@/lang";
import store from "@/store/modules/index";
import { Component } from "vue-property-decorator";

export enum DeviceType {
  Mobile,
  Desktop
}

export interface IAppState {
  device: DeviceType;
  sidebar: {
    opened: boolean;
    withoutAnimation: boolean;
  };
  language: string;
  size: string;
}

@Module({ dynamic: true, store, name: "app" })
class App extends VuexModule implements IAppState {
  public sidebar = {
    opened: getSidebarStatus() !== "closed",
    withoutAnimation: false
  };
  public device = DeviceType.Desktop;
  public language = getLocale();
  public size = getSize() || "small";


  @Mutation
  private TOGGLE_SIDEBAR(withoutAnimation: boolean) {
    this.sidebar.opened = !this.sidebar.opened;
    this.sidebar.withoutAnimation = withoutAnimation;
    if (this.sidebar.opened) {
      setSidebarStatus("opened");
    } else {
      setSidebarStatus("closed");
    }
  }

  @Mutation
  private CLOSE_SIDEBAR(withoutAnimation: boolean) {
    this.sidebar.opened = false;
    this.sidebar.withoutAnimation = withoutAnimation;
    setSidebarStatus("closed");
  }

  @Mutation
  private TOGGLE_DEVICE(device: DeviceType) {
    this.device = device;
  }

  @Mutation
  private SET_LANGUAGE(language: string) {
    this.language = language;
    setLanguage(this.language);
  }

  @Mutation
  private SET_SIZE(size: string) {
    this.size = size;
    setSize(this.size);
  }

  @Action
  public ToggleSideBar(withoutAnimation: boolean) {
    this.TOGGLE_SIDEBAR(withoutAnimation);
  }

  @Action
  public CloseSideBar(withoutAnimation: boolean) {
    this.CLOSE_SIDEBAR(withoutAnimation);
  }

  @Action
  public ToggleDevice(device: DeviceType) {
    this.TOGGLE_DEVICE(device);
  }

  @Action
  public SetLanguage(language: string) {
    this.SET_LANGUAGE(language);
  }

  @Action
  public SetSize(size: string) {
    this.SET_SIZE(size);
  }

  @Action
  public ImportPage(url: string): any {
    const languagePage = this.language === 'en' ? '.en' : '';
    return import(`${url}${languagePage}.vue`).catch(() => {
        return import(`${url}.vue`);
    });
  }
}

export const AppModule = getModule(App);
