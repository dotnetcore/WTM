import { RouteConfig } from "vue-router";
import Menu from "./menu/menu";

// interface menuItem {}

class user {
    // 权限
    Actions: string[];
    // 树菜单
    treeMenus: RouteConfig[];
    // 同级菜单
    parallelMenus: RouteConfig[];
    constructor() {
        this.Actions = [];
        this.treeMenus = [];
        this.parallelMenus = [];
    }
    setAction(data: any[]) {
        this.Actions = _.map(data, item => _.toLower(item));
        return this.Actions;
    }
    /**
     * 返回tree格式菜单 async
     * @param data
     */
    setTreeMenus(data: any[]) {
        this.treeMenus = Menu.getTreeMenu(data);
        return this.treeMenus;
    }
    /**
     * 返回同一级别
     * @param data
     */
    setParallelMenus(data: any[]) {
        this.parallelMenus = Menu.getParallelMenu(data);
        return this.parallelMenus;
    }
}

export default new user();
