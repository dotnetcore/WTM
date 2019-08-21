import Vue from "vue";
import VueRouter from "vue-router";
import store from "@/store/index";
Vue.use(VueRouter);
interface routerItem {
    children: [];
    path: string;
    meta: {};
}
// 打平一级
function generateRoutesFromMenu(
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
            generateRoutesFromMenu(item.children, routes, item);
        }
    }
    return routes;
}

// 等待接口
export default function createRouter() {
    // 接口路由
    return store
        .dispatch("localMenus")
        .then(res => {
            // 数据结构不同，此处重新维护
            store.commit("setMenuItems", res);
            const routers = new VueRouter({
                mode: "hash", // 'history',
                routes: generateRoutesFromMenu(res)
            });
            console.log("routers", routers);
            return routers;
        })
        .catch(error => {
            // 本地调试 可以注释
            location.href = "/login.html";
        });
}
