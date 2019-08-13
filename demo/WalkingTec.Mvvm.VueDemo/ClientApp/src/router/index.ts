import Vue from "vue";
import VueRouter from "vue-router";
import store from "@/store/index";
Vue.use(VueRouter);
// 结构
const fnMenus = menuItems => {
    let menus = [];
    if (menuItems && menuItems.length > 0) {
        menus = menuItems.map(menuItem => {
            const ret = {
                name: menuItem.name,
                meta: {
                    icon: menuItem.icon
                },
                children: [],
                url: menuItem.url,
                path: menuItem.path || menuItem.url,
                //component: () => import(`${vueFile}`)
                component: () => import("@/views" + menuItem.url + ".vue")
            };
            ret.children = fnMenus(menuItem.children);
            return ret;
        });
    }
    return menus;
};
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
            // res = fnMenus(res.data.list);
            store.commit("setMenuItems", res);
            const routers = new VueRouter({
                mode: "hash", // 'history',
                routes: generateRoutesFromMenu(res)
            });
            return routers;
        })
        .catch(error => {
            // 本地调试 可以注释
            location.href = "/login.html";
        });
}
