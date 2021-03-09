import { __decorate, __extends, __metadata } from "tslib";
import { VuexModule, Module, Mutation, Action, getModule } from "vuex-module-decorators";
import { constantRoutes } from "@/router";
import Menu from "@/router/menu";
import store from "@/store/modules/index";
var Routes = /** @class */ (function (_super) {
    __extends(Routes, _super);
    function Routes() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.routes = [];
        _this.dynamicRoutes = [];
        _this.parallelMenus = [];
        _this.pageList = [];
        return _this;
    }
    Routes.prototype.SET_ROUTES = function (routes) {
        this.routes = constantRoutes.concat(routes);
        this.dynamicRoutes = routes;
    };
    Routes.prototype.SET_PARALLEL_MENUS = function (routes) {
        this.parallelMenus = routes;
    };
    Routes.prototype.SET_PAGE_LIST = function (data) {
        this.pageList = data;
    };
    Routes.prototype.GenerateRoutes = function (menu) {
        var treeMenus = Menu.getTreeMenu(menu);
        this.SET_ROUTES(treeMenus);
    };
    Routes.prototype.ParallelMenus = function (menu) {
        var parallelMenus = Menu.getParallelMenu(menu);
        this.SET_PARALLEL_MENUS(parallelMenus);
    };
    Routes.prototype.PageList = function (pages) {
        var pagesList = [];
        _.map(pages, function (item) {
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
    };
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Array]),
        __metadata("design:returntype", void 0)
    ], Routes.prototype, "SET_ROUTES", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Array]),
        __metadata("design:returntype", void 0)
    ], Routes.prototype, "SET_PARALLEL_MENUS", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Array]),
        __metadata("design:returntype", void 0)
    ], Routes.prototype, "SET_PAGE_LIST", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Array]),
        __metadata("design:returntype", void 0)
    ], Routes.prototype, "GenerateRoutes", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Array]),
        __metadata("design:returntype", void 0)
    ], Routes.prototype, "ParallelMenus", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], Routes.prototype, "PageList", null);
    Routes = __decorate([
        Module({ dynamic: true, store: store, name: "routes" })
    ], Routes);
    return Routes;
}(VuexModule));
export var RoutesModule = getModule(Routes);
//# sourceMappingURL=routes.js.map