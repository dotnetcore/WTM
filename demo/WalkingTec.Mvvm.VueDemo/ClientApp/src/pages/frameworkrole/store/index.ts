import Vuex from "vuex";
import api from "./api";
import createStore from "@/store/base/index";
const newStore = createStore(api);
const mutations = {
  ...newStore.mutations,
  setGetPageActions_mutations: (state, data) => {
    state["getPageActionsData"] = data;
  }
};
export default new Vuex.Store({
  strict: true,
  ...newStore,
  mutations
});
