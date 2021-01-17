import Vuex from "vuex";
import apis from "@/service/modules/upload";
import createStore from "@/store/base/index";

const newStore = createStore(apis);

export default new Vuex.Store({
  strict: true,
  getters: {},
  ...newStore
});
