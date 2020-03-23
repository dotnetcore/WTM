import {
  VuexModule,
  Module,
  Mutation,
  Action,
  getModule
} from "vuex-module-decorators";
import { RouteConfig } from "vue-router";
import { constantRoutes } from "@/router";
import Menu from "@/router/menu";
import store from "@/store/modules/index";

export interface IRoutesModule {
  // 全部
  routes: RouteConfig[];
  // 动态数据
  dynamicRoutes: RouteConfig[];
  // 数据平级（暂时无用）
  parallelMenus: RouteConfig[];
  // 页面列表
  pageList: any[];
}

@Module({ dynamic: true, store, name: "routes" })
class Routes extends VuexModule implements IRoutesModule {
  public routes: RouteConfig[] = [];
  public dynamicRoutes: RouteConfig[] = [];
  public parallelMenus: RouteConfig[] = [];
  public pageList: any[] = [];

  @Mutation
  private SET_ROUTES(routes: RouteConfig[]) {
    this.routes = constantRoutes.concat(routes);
    this.dynamicRoutes = routes;
  }

  @Mutation
  private SET_PARALLEL_MENUS(routes: RouteConfig[]) {
    this.parallelMenus = routes;
  }

  @Mutation
  private SET_PAGE_LIST(data: any[]) {
    this.pageList = data;
  }

  @Action
  public GenerateRoutes(menu: any[]) {
    const treeMenus = Menu.getTreeMenu(menu);
    this.SET_ROUTES(treeMenus);
  }

  @Action
  public ParallelMenus(menu: any[]) {
    const parallelMenus = Menu.getParallelMenu(menu);
    this.SET_PARALLEL_MENUS(parallelMenus);
  }

  @Action
  public PageList(pages: object) {
    const pagesList: any[] = [];
    _.map(pages, item => {
      if (item.controller) {
        pagesList.push({
          Text: item.name,
          Value: item.controller,
          Url: item.path,
          Icon: item.icon
        });
      }
    });
    this.SET_PAGE_LIST(pagesList);
  }
}

export const RoutesModule = getModule(Routes);
