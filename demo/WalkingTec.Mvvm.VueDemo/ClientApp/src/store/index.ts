import Vue from "vue";
import Vuex from "vuex";
import dataMenuItems from "./menu/menu-items";
import reqIndex from "../service/index";
import createStore from "./base/index";
import config from "@/config/index";
import cache from "@/util/cache";

Vue.use(Vuex);
const newStore = createStore(reqIndex);

const state = {
    isFold: false,
    // 菜单
    menuItems: []
};
const mutations = {
    toggleIsFold(states) {
        states.isFold = !states.isFold;
    },
    setMenuItems(states, data) {
        states.menuItems = data;
    }
};
const actions = {
    // 返回菜单结构 menuItems: Array<RouteConfig>
    genMenus({ commit }, menuItems: any) {
        // 递归结构
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
                        path: "",
                        component: () => {}
                    };
                    ret.path = menuItem.path;
                    ret.component = () => import("@/views" + ret.path + ".vue");
                    ret.children = fnMenus(menuItem.children);
                    return ret;
                });
            }
            return menus;
        };
        return fnMenus(menuItems);
    },
    // 本地菜单配置
    localMenus({ dispatch, commit }) {
        return dispatch("genMenus", dataMenuItems).then(res => {
            const menus = cache.getStorage(config.tokenKey, true);
            let systemMenus = [];
            if (menus.Attributes && menus.Attributes.Menus) {
                // 优化
                systemMenus = menus.Attributes.Menus.map(item => {
                    return {
                        name: item.Text,
                        path: item.Url,
                        meta: {
                            icon: item.Icon
                        },
                        component: () =>
                            import("@/views/system" + item.Url + "/index.vue")
                    };
                });
            }
            res.forEach(item => {
                if (item.path === "system") {
                    item.children = systemMenus;
                }
            });
            commit("setMenuItems", res);
            return res;
        });
    }
};
const getters = {
    menuItems: (state: any) => state.menuItems,
    isCollapse: (state: any) => state.isFold
};
export default new Vuex.Store({
    state: { ...state, ...newStore.state },
    actions: { ...newStore.actions, ...actions },
    getters,
    mutations: { ...mutations, ...newStore.mutations }
});
