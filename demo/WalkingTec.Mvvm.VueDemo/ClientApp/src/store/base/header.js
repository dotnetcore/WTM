import Vue from 'vue';
import Vuex from 'vuex';
import service from '@/service/service';
import requestUrls from '@/service/header';
Vue.use(Vuex);

const state = {};
const actions = {
    getLogout({ commit }, params) {
        const option = {
            ...requestUrls.logout,
            data: params
        };
        return service(option).then(result => {
            console.log('result', result, commit);
            return result;
        });
    },
};

const getters = {};

const mutations = {};

const modules = {};

const store = new Vuex.Store({
    actions,
    getters,
    modules,
    state,
    mutations
});

export default store;
