import Vuex from "vuex";
import service from "@/service/system/frameworkrole";
import createStore from "../base/index";
const newStore = createStore(service);
const mutations = {
    ...newStore.mutations,
    'setGetPageActions_mutations': (state, data) => {
        state['getPageActionsData'] = data;
    }
};
export default new Vuex.Store({
    strict: true,
    ...newStore,
    mutations
});
