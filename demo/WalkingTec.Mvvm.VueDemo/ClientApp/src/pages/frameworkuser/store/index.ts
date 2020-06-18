import Vuex from "vuex";
import service from "./api";
import createStore from "@/store/base/index";
const newStore = createStore(service);

export default new Vuex.Store({
  strict: true,
  getters: {},
  ...newStore
});
