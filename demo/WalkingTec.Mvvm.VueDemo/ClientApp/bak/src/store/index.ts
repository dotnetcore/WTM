import Vue from "vue";
import Vuex from "vuex";
import menus from "./menu";
Vue.use(Vuex);

export default new Vuex.Store({
    state: {},
    actions: {},
    getters: {
        menuItems: (state: any) => state.menus.items
    },
    modules: { menus },
    mutations: {}
});
