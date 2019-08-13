/**
 * 菜单数据
 * window.serverMenus
 */
import dataMenuItems from "./menu-items";

const serverMenuData = { data: { list: [] } };
const genMenus = (menuItems?: any) => {
    let menus = [];
    if (menuItems && menuItems.length > 0) {
        menus = menuItems.map(menuItem => {
            const ret = {
                name: menuItem.name,
                meta: {
                    icon: menuItem.icon
                },
                children: [],
                path: "",
                component: () => {}
            };
            ret.path = menuItem.url;
            ret.component = () =>
                import("pages/index/" + menuItem.url + ".vue").catch(() => {
                    return import("pages/index/test/index.vue");
                });
            ret.children = genMenus(menuItem.children);
            return ret;
        });
    }
    return menus;
};

const routes = genMenus(
    serverMenuData && serverMenuData.data && serverMenuData.data.list
);
console.log("routes", routes);
export default () => {
    return routes.concat(dataMenuItems);
    // return routes;
};
