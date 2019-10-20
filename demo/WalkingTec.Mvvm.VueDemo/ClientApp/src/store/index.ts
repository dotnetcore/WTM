import Vue from "vue";
import Vuex from "vuex";
import reqIndex from "../service/index";
import createStore from "./base/index";

Vue.use(Vuex);
const newStore = createStore(reqIndex);

const state = {
    isFold: false,
    // 菜单
    menuItems: [],
    Actions: [],
    Menus: []
};
const mutations = {
    toggleIsFold(states) {
        states.isFold = !states.isFold;
    },
    setMenuItems(states, data) {
        states.menuItems = data;
    }
};
const getters = {
    menuItems: (state: any) => state.menuItems,
    isCollapse: (state: any) => state.isFold
};
export default new Vuex.Store({
    state: { ...state, ...newStore.state },
    actions: newStore.actions,
    getters,
    mutations: { ...mutations, ...newStore.mutations }
});
