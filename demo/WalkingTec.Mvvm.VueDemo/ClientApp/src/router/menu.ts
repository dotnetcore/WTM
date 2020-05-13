import { RouteConfig } from "vue-router";
import config from "@/config/index";
import Layout from "@/components/layout/index.vue";
import { VueRouter } from "@/components/layout/components/index";
import { isExternal } from "@/util/validate";
import { AppModule } from "@/store/modules/app";

const development = config.development;
interface routerItem {
  children: [];
  path: string;
  meta: {};
}
let url_index = 0;
const urlList: String[] = [];

class Menu {
  constructor() {}
  /**
   * 返回vue-router格式
   * @param menuItem
   */
  private getRouterItem(menuItem) {
    url_index++;
    urlList.push("" + url_index);
    const routerItem: RouteConfig = {
      path: menuItem.Url || "" + url_index,
      name: menuItem.Text,
      component: Layout,
      children: [] as RouteConfig[],
      meta: {
        key: menuItem.Key,
        title: menuItem.Text,
        icon: menuItem.Icon,
        ParentId: menuItem.ParentId,
        Id: menuItem.Id
      }
    };
    if (menuItem.Url) {
      // 判断是否需要 external
      if (
        (isExternal(menuItem.Url) ||
        _.startsWith(menuItem.Url, config.staticPage))
      ) {
        routerItem.component = () => import("@/pages/external/index.vue");
        routerItem.path = `/external_${url_index}`;
        const url = isExternal(menuItem.Url)
          ? menuItem.Url
          : _.replace(
              menuItem.Url,
              config.staticPage,
              `${window.location.origin}`
            );
        routerItem.props = { default: true, url: url };
      } else {
        const languagePage = AppModule.language === 'en' ? '.en' : '';
        routerItem.component = () =>
          import(`@/pages${menuItem.Url}/index${languagePage}.vue`).catch(err => {
            return import(`@/pages${menuItem.Url}/index.vue`);
          });
      }
    } else {
      if (menuItem.ParentId) {
        routerItem.component = VueRouter;
      }
    }
    return routerItem;
  }
  /**
   * 返回同一级菜单（打平一级）
   * @param menu
   */
  getParallelMenu(menu: any[]) {
    if (development) {
      menu = require("@/subMenu.json");
    }
    return _.map(menu, menuItem => {
      return this.getRouterItem(menuItem);
    });
  }
  /**
   * 返回tree格式菜单 async
   */
  getTreeMenu(menu: any[]) {
    if (development) {
      menu = require("@/subMenu.json");
    }
    const trees = this.recursionTree(menu);
    return trees.filter(item => {
      if (
        urlList.includes(item.path) &&
        item.children &&
        item.children.length === 0
      ) {
        return false;
      }
      return true;
    });
  }

  /**
   * 递归 格式化 树
   * @param datalist
   * @param ParentId 父级id
   * @param children
   */
  private recursionTree(
    datalist,
    ParentId = null,
    children: RouteConfig[] = []
  ): RouteConfig[] {
    _.filter(datalist, item => {
      if (ParentId === null) {
        return !item.ParentId;
      } else {
        return item.ParentId === ParentId;
      }
    }).map(menuItem => {
      const routerItem: RouteConfig = this.getRouterItem(menuItem);
      routerItem.children = this.recursionTree(
        datalist,
        menuItem.Id,
        menuItem.children || []
      );
      if (
        ParentId === null &&
        menuItem.Url &&
        routerItem.children.length === 0
      ) {
        routerItem.children.push({
          ...routerItem,
          path: "",
          children: undefined
        });
        routerItem.component = Layout;
      }
      children.push(routerItem);
    });
    return children;
  }

  /**
   * (tree)递归 格式化 打平一级菜单
   */
  private generateRoutesFromMenu(
    menu,
    routes: Array<routerItem> = [],
    parentMenu?: routerItem
  ) {
    for (let i = 0, l = menu.length; i < l; i++) {
      const item: routerItem = menu[i];
      if (item.path) {
        item.meta["parentMenu"] = parentMenu;
        const itemClone = { ...item };
        delete itemClone.children;
        routes.push(itemClone);
      }
      if (item.children) {
        this.generateRoutesFromMenu(item.children, routes, item);
      }
    }
    return routes;
  }
}
export default new Menu();
