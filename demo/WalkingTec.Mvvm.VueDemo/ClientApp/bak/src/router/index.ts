import Vue from "vue";
import Router from "vue-router";
import { state } from "../store/menu";

Vue.use(Router);

function generateRoutesFromMenu(menu: any, parentMenu?: any, routes: any = []) {
    // for (let i = 0, l = menu.length; i < l; i++) {
    //     const item = menu[i];
    //     if (item.path) {
    //         item.meta.parentMenu = parentMenu;
    //         const itemClone = { ...item };
    //         delete itemClone.children;
    //         routes.push(itemClone);
    //     }
    //     if (item.children) {
    //         generateRoutesFromMenu(item.children, item, routes);
    //     }
    // }
    return routes;
}

export default new Router({
    mode: "hash", // 'history',
    routes: [
        {
            name: "user",
            path: "*",
            component: () => import("@/pages/index/user/index.vue")
        },
        ...generateRoutesFromMenu(state.items, null, [])
    ]
});
