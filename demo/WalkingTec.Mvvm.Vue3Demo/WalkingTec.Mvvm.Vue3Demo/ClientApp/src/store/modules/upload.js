import { __assign } from "tslib";
import Vuex from "vuex";
import apis from "@/service/modules/upload";
import createStore from "@/store/base/index";
var newStore = createStore(apis);
export default new Vuex.Store(__assign({ strict: true, getters: {} }, newStore));
//# sourceMappingURL=upload.js.map