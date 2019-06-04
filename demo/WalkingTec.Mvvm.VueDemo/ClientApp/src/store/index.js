import Vue from "vue";
import Vuex from "vuex";
import menus from "./menu";
Vue.use(Vuex);

const state = {};
const actions = {};

const getters = {
    menuItems: state => state.menus.items
};

const mutations = {};

const modules = { menus };

const store = new Vuex.Store({
    actions,
    getters,
    modules,
    state,
    mutations
});

export default store;
