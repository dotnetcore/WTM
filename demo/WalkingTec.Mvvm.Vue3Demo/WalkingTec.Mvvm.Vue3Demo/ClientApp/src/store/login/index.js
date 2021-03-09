import { __assign } from "tslib";
import Vue from "vue";
import Vuex from "vuex";
import reqLogin from "@/service/login";
import createStore from "@/store/base/index";
Vue.use(Vuex);
var newStore = createStore(reqLogin);
export default new Vuex.Store(__assign({ strict: true, getters: {} }, newStore));
//# sourceMappingURL=index.js.map