import Vuex from "vuex";
import api from "./api";
import createStore from "@/store/base/index";
const newStore = createStore(api);
export default new Vuex.Store({
  strict: true,
  getters: {},
  ...newStore
});
