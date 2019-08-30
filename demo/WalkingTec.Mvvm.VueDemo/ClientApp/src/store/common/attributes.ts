import Vue from "vue";
import Vuex from "vuex";
Vue.use(Vuex);

const state: Object = {
    Actions: [],
    Menus: []
};

const mutations = {
    setActions(states, data) {
        states.Actions = _.map(data, _.toLower);
    },
    setMenus(states, data) {
        states.Menus = data;
    }
};

const actions = {};

const store = new Vuex.Store({
    actions,
    state,
    mutations
});

export default store;
