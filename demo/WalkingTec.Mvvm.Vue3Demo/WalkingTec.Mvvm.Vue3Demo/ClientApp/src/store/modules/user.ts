import {
  VuexModule,
  Module,
  Action,
  Mutation,
  getModule,
  MutationAction
} from "vuex-module-decorators";
import { getToken, setToken, removeToken } from "@/util/cookie";
import { resetRouter } from "@/router";
import store from "@/store/modules/index";
import _request from "@/util/service";
import serviceUrl from "@/service/modules/user";

export interface IUserState {
  token: string;
  name: string;
  roles: Array<any>;
  actionList: string[];
  menus: any[];
  info: any;
}

@Module({ dynamic: true, store, name: "user" })
class User extends VuexModule implements IUserState {
  public token = getToken() || "";
  public name = "";
  public roles: Array<any> = [];
  public actionList: string[] = [];
  public menus: Array<any> = [];
  public info = {};

  @Mutation
  private SET_TOKEN(token: string) {
    this.token = token;
  }

  @Mutation
  private SET_NAME(name: string) {
    this.name = name;
  }

  @Mutation
  private SET_ROLES(roles) {
    this.roles = roles;
  }

  @Mutation
  private SET_ACTIONS(actionList: string[]) {
    this.actionList = actionList;
  }

  @Mutation
  private SET_INFO(info: any) {
    this.info = info;
  }

  @Mutation
  private SET_MENUS(menus: any) {
    this.menus = menus;
  }

  @Action
  public async GetUserInfo() {
    const data = await _request({
      ...serviceUrl.loginCheckLogin,
      data: { ID: this.token }
    });
    if (!data) {
      throw Error("Verification failed, please Login again.");
    }
    const { Id, ITCode, Name, PhotoId, Roles, Attributes } = data;
    this.SET_ROLES(Roles);
    this.SET_NAME(Name);
    this.SET_ACTIONS(Attributes.Actions);
    this.SET_MENUS(Attributes.Menus);
    this.SET_INFO({ Id, ITCode, Name, PhotoId });
  }

  @Action
  public async LogOut() {
    if (this.token === "") {
      throw Error("LogOut: token is undefined!");
    }
    await _request({
      ...serviceUrl.loginLogout,
      data: { ID: this.token }
    });
    removeToken();
    // 单页面
    this.ResetToken();
  }

  @Action
  public ChangePassword(params) {
    return _request({
      ...serviceUrl.ChangePassword,
      data: { ...params }
    }).catch(({ response }) => {
      return { error: true, data: response.data };
    });
  }
  /********************* 单页面 需要如下***********************/
  @Action
  public async Login(userInfo: { username: string; password: string }) {
    let { username, password } = userInfo;
    username = username.trim();
    // const { data } = await login({ username, password });
    const data = { accessToken: "" };
    setToken(data.accessToken);
    this.SET_TOKEN(data.accessToken);
  }

  @Action
  public ResetToken() {
    removeToken();
    resetRouter();
    this.SET_TOKEN("");
    this.SET_ROLES([]);
    this.SET_ACTIONS([]);
  }
  /********************* 单页面 end ***********************/
}

export const UserModule = getModule(User);
