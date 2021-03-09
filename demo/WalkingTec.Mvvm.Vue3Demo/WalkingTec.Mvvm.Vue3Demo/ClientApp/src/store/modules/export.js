import { __assign } from "tslib";
import Vue from "vue";
import Vuex from "vuex";
import { getExportInfoService, getProgressService, getDownloadInfo } from "@/service/modules/export";
import service from "@/util/service";
Vue.use(Vuex);
var state = {
    session: "",
    isFinish: false,
    progress: 0,
    position: 0,
    exceed: false,
    exceedMsg: "",
    downloadUrl: getDownloadInfo.url
};
var mutations = {
    setSession: function (state, value) {
        state.session = value;
    },
    setProgress: function (state, value) {
        state.progress = value;
    },
    setIsFinish: function (state, value) {
        state.isFinish = value;
    },
    setExceed: function (state, value) {
        state.exceed = value;
    },
    setExceedMsg: function (state, value) {
        state.exceedMsg = value;
    },
    setPosition: function (state, value) {
        state.position = value;
    }
};
var actions = {
    // eslint-disable-next-line
    getExportInfo: function (_a, params) {
        var commit = _a.commit;
        var sendData = __assign({}, getExportInfoService);
        var paramsCopy = __assign({}, params);
        sendData.data = paramsCopy;
        console.log("sendData", sendData);
        return service(sendData).then(function (data) {
            commit("setSession", data.data.session);
            return data;
        });
    },
    // eslint-disable-next-line
    getProgress: function (_a) {
        var commit = _a.commit, state = _a.state;
        var sendData = __assign({}, getProgressService);
        console.log("state.session", state.session);
        sendData.url = sendData.url.replace("{session}", state.session);
        sendData.data = { session: state.session };
        return service(sendData).then(function (data) {
            commit("setIsFinish", data.data.status === 2);
            commit("setExceed", data.data.status === 3);
            commit("setExceedMsg", data.data.err_msg);
            if (data.data.total !== 0) {
                commit("setProgress", Math.ceil(100 * (data.data.position / data.data.total)));
            }
            commit("setPosition", data.data.position);
            return data;
        });
    }
};
var store = new Vuex.Store({
    actions: actions,
    state: state,
    mutations: mutations
});
export default store;
//# sourceMappingURL=export.js.map