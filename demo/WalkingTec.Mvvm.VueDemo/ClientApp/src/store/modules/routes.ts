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
import store from "@/store/modules";

export interface IRoutesModule {
    routes: RouteConfig[];
    dynamicRoutes: RouteConfig[];
}

@Module({ dynamic: true, store, name: "routes" })
class Routes extends VuexModule implements IRoutesModule {
    public routes: RouteConfig[] = [];
    public dynamicRoutes: RouteConfig[] = [];

    @Mutation
    private SET_ROUTES(routes: RouteConfig[]) {
        this.routes = constantRoutes.concat(routes);
        this.dynamicRoutes = routes;
    }

    @Action
    public GenerateRoutes(menu: any[]) {
        const treeMenus = Menu.getTreeMenu(menu);
        this.SET_ROUTES(treeMenus);
    }
}

export const RoutesModule = getModule(Routes);
