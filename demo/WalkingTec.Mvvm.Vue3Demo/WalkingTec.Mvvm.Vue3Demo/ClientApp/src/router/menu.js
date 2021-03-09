import { __assign } from "tslib";
import config from "@/config/index";
import Layout from "@/components/layout/index.vue";
import { VueRouter } from "@/components/layout/components/index";
import { isExternal } from "@/util/validate";
import { AppModule } from "@/store/modules/app";
var development = config.development;
var url_index = 0;
var urlList = [];
var Menu = /** @class */ (function () {
    function Menu() {
    }
    /**
     * 返回vue-router格式
     * @param menuItem
     */
    Menu.prototype.getRouterItem = function (menuItem) {
        url_index++;
        urlList.push("" + url_index);
        var routerItem = {
            path: menuItem.Url || "" + url_index,
            name: menuItem.Text,
            component: Layout,
            children: [],
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
            if ((isExternal(menuItem.Url) ||
                _.startsWith(menuItem.Url, config.staticPage))) {
                routerItem.component = function () { return import("@/pages/external/index.vue"); };
                routerItem.path = "/external_" + url_index;
                var url = isExternal(menuItem.Url)
                    ? menuItem.Url
                    : _.replace(menuItem.Url, config.staticPage, "" + window.location.origin);
                routerItem.props = { default: true, url: url };
            }
            else {
                var languagePage_1 = AppModule.language === 'en' ? '.en' : '';
                routerItem.component = function () {
                    return import("@/pages" + menuItem.Url + "/index" + languagePage_1 + ".vue").catch(function (err) {
                        return import("@/pages" + menuItem.Url + "/index.vue");
                    });
                };
            }
        }
        else {
            if (menuItem.ParentId) {
                routerItem.component = VueRouter;
            }
        }
        return routerItem;
    };
    /**
     * 返回同一级菜单（打平一级）
     * @param menu
     */
    Menu.prototype.getParallelMenu = function (menu) {
        var _this = this;
        if (development) {
            menu = require("@/subMenu.json");
        }
        return _.map(menu, function (menuItem) {
            return _this.getRouterItem(menuItem);
        });
    };
    /**
     * 返回tree格式菜单 async
     */
    Menu.prototype.getTreeMenu = function (menu) {
        if (development) {
            menu = require("@/subMenu.json");
        }
        var trees = this.recursionTree(menu);
        return trees.filter(function (item) {
            if (urlList.includes(item.path) &&
                item.children &&
                item.children.length === 0) {
                return false;
            }
            return true;
        });
    };
    /**
     * 递归 格式化 树
     * @param datalist
     * @param ParentId 父级id
     * @param children
     */
    Menu.prototype.recursionTree = function (datalist, ParentId, children) {
        var _this = this;
        if (ParentId === void 0) { ParentId = null; }
        if (children === void 0) { children = []; }
        _.filter(datalist, function (item) {
            if (ParentId === null) {
                return !item.ParentId;
            }
            else {
                return item.ParentId === ParentId;
            }
        }).map(function (menuItem) {
            var routerItem = _this.getRouterItem(menuItem);
            routerItem.children = _this.recursionTree(datalist, menuItem.Id, menuItem.children || []);
            if (ParentId === null &&
                menuItem.Url &&
                routerItem.children.length === 0) {
                routerItem.children.push(__assign(__assign({}, routerItem), { path: "", children: undefined }));
                routerItem.component = Layout;
            }
            children.push(routerItem);
        });
        return children;
    };
    /**
     * (tree)递归 格式化 打平一级菜单
     */
    Menu.prototype.generateRoutesFromMenu = function (menu, routes, parentMenu) {
        if (routes === void 0) { routes = []; }
        for (var i = 0, l = menu.length; i < l; i++) {
            var item = menu[i];
            if (item.path) {
                item.meta["parentMenu"] = parentMenu;
                var itemClone = __assign({}, item);
                delete itemClone.children;
                routes.push(itemClone);
            }
            if (item.children) {
                this.generateRoutesFromMenu(item.children, routes, item);
            }
        }
        return routes;
    };
    return Menu;
}());
export default new Menu();
//# sourceMappingURL=menu.js.map