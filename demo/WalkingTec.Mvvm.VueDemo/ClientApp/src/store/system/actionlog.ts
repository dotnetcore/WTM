import Vue from "vue";
import Vuex from "vuex";
import service from "@/service/system/actionlog";
import createStore from "../base/index";
Vue.use(Vuex);
const newStore = createStore(service);
export default new Vuex.Store({
    strict: true,
    state: { ...newStore.state },
    actions: { ...newStore.actions },
    getters: {},
    mutations: { ...newStore.mutations },
    modules: newStore.modules
});
