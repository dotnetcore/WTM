import { __assign } from "tslib";
import Vuex from "vuex";
import service from "./api";
import createStore from "@/store/base/index";
var newStore = createStore(service);
export default new Vuex.Store(__assign({ strict: true, getters: {} }, newStore));
//# sourceMappingURL=index.js.map