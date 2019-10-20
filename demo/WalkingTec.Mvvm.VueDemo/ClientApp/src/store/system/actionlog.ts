import Vuex from "vuex";
import service from "@/service/system/actionlog";
import createStore from "../base/index";
const newStore = createStore(service);
export default new Vuex.Store({
    strict: true,
    getters: {},
    ...newStore
});
