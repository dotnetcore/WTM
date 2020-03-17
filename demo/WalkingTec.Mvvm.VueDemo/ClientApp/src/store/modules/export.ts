import Vue from "vue";
import Vuex from "vuex";
import {
  getExportInfoService,
  getProgressService,
  getDownloadInfo
} from "@/service/modules/export";

import service from "@/util/service";
Vue.use(Vuex);
type serviceType = {
  method: string;
  url: string;
  data?: Object;
};

const state: Object = {
  session: "",
  isFinish: false,
  progress: 0,
  position: 0,
  exceed: false,
  exceedMsg: "",
  downloadUrl: getDownloadInfo.url
};

const mutations = {
  setSession(state, value) {
    state.session = value;
  },
  setProgress(state, value) {
    state.progress = value;
  },
  setIsFinish(state, value) {
    state.isFinish = value;
  },
  setExceed(state, value) {
    state.exceed = value;
  },
  setExceedMsg(state, value) {
    state.exceedMsg = value;
  },
  setPosition(state, value) {
    state.position = value;
  }
};

const actions = {
  // eslint-disable-next-line
  getExportInfo({ commit }, params) {
    const sendData: serviceType = {
      ...getExportInfoService
    };
    const paramsCopy = {
      ...params
    };
    sendData.data = paramsCopy;
    console.log("sendData", sendData);
    return service(sendData).then(data => {
      commit("setSession", data.data.session);
      return data;
    });
  },
  // eslint-disable-next-line
  getProgress({ commit, state }) {
    const sendData: serviceType = {
      ...getProgressService
    };
    console.log("state.session", state.session);
    sendData.url = sendData.url.replace("{session}", state.session);
    sendData.data = { session: state.session };
    return service(sendData).then(data => {
      commit("setIsFinish", data.data.status === 2);
      commit("setExceed", data.data.status === 3);
      commit("setExceedMsg", data.data.err_msg);
      if (data.data.total !== 0) {
        commit(
          "setProgress",
          Math.ceil(100 * (data.data.position / data.data.total))
        );
      }

      commit("setPosition", data.data.position);
      return data;
    });
  }
};

const store = new Vuex.Store({
  actions,
  state,
  mutations
});

export default store;
