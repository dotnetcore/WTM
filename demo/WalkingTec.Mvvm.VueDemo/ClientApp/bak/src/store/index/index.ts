import Vue from "vue";
import Vuex from "vuex";
import reqLogin from "../../service/login";
import createStore from "../base/index";
Vue.use(Vuex);
// 回调统一判断登陆
function callback(result, commit) {
    if (result && result.code === 3) {
        commit("setIsLogin", false);
    }
}
const newStore = createStore(reqLogin, callback);
const getters = {
    isLogin: state => state.isLogin
};
const state = { isLogin: false };
const mutations = {
    setIsLogin(state, data) {
        console.log(state, data);
        state.isLogin = data;
    }
};

export default new Vuex.Store({
    strict: true,
    state: { ...newStore.state, ...state },
    actions: { ...newStore.actions },
    getters: { ...getters },
    mutations: { ...newStore.mutations, ...mutations },
    modules: {}
});
