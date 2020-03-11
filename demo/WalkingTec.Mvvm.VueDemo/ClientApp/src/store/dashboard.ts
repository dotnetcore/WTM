import Vuex from "vuex";
import service from "@/service/dashboard";
import createStore from "./base/index";
const newStore = createStore(service);
export default new Vuex.Store({
  strict: true,
  getters: {},
  ...newStore
});
