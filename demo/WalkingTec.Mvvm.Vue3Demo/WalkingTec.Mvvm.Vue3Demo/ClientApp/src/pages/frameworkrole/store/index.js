import { __assign } from "tslib";
import Vuex from "vuex";
import api from "./api";
import createStore from "@/store/base/index";
var newStore = createStore(api);
var mutations = __assign(__assign({}, newStore.mutations), { setGetPageActions_mutations: function (state, data) {
        state["getPageActionsData"] = data;
    } });
export default new Vuex.Store(__assign(__assign({ strict: true }, newStore), { mutations: mutations }));
//# sourceMappingURL=index.js.map