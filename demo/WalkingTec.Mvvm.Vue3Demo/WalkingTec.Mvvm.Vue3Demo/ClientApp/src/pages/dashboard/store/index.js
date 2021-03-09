import { __assign } from "tslib";
import Vuex from "vuex";
import api from "./api";
import createStore from "@/store/base/index";
var newStore = createStore(api);
export default new Vuex.Store(__assign({ strict: true, getters: {} }, newStore));
//# sourceMappingURL=index.js.map