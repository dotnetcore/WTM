/**
 * 根据service中 创建store 初始结构
 * 注：store如果没有逻辑可以用
 * 目前只创建 state，actions，mutations 部分
 */
import _request from "@/util/service";
import attributes from "./attributes";

interface StoreType {
  state: {};
  actions: {};
  mutations: {};
  modules?: {};
  getters?: {};
}

const stoBase = {
  // 接口列表key（service中）
  apiKeys: "actionList",
  // 返回命名
  getKeyName: key => {
    const upperKey = _.upperFirst(key);
    const mutationsKey = `set${upperKey}_mutations`;
    const stateKey = `${key}Data`;
    return { mutationsKey, stateKey };
  },
  // service接口列表的名称作为state的key，并判断是否包含List，如果包含List定位数据
  stateDef: (keyName, dataType) => {
    if (keyName.indexOf("List") > -1 || dataType === "array") {
      return [];
    } else {
      return { obj: {} };
    }
  },
  /**
   *  store > mutations
   *  返回数据，优先使用Entity，如果类型不一样，自定义mutations
   */
  mutationsDef: stateKey => {
    return (state, data) => {
      // 接口返回数据结构 如果:{data: {}}
      state[stateKey] = data.Entity || data;
    };
  },
  // store > action
  actionsDef: (serviceItem, mutationsKey: string, cb?: Function) => {
    return ({ commit }, params) => {
      const option = Object.assign({ data: params }, serviceItem);
      return _request(option, null).then(result => {
        if (serviceItem.method === "get") {
          commit(mutationsKey, result || {});
          // 判断是否回调方法
          cb && cb(result, commit);
        }
        return result || {};
      });
    };
  }
};

/**
 * 根据service 创建store
 * @param {*} serviceUnit: service接口列表
 * stoBase.apiKeys [propName: string]: string;
 */
export default (serviceUnit, callback?: Function) => {
  const store: StoreType = {
    state: { [stoBase.apiKeys]: {} }, // actionList 接口列表
    actions: {},
    mutations: {},
    modules: {}
  };
  for (const key in serviceUnit) {
    const serviceItem = serviceUnit[key];
    const { mutationsKey, stateKey } = stoBase.getKeyName(key);
    // 接口列表
    store.state[stoBase.apiKeys][key] = serviceItem.url;
    //  get定义（state，mutations），post/put 不定义
    if (serviceItem.method === "get") {
      store.state[stateKey] = stoBase.stateDef(key, serviceItem.dataType);
      // mutations
      store.mutations[mutationsKey] = stoBase.mutationsDef(stateKey);
    }
    // actions instanceof
    store.actions[key] = stoBase.actionsDef(
      serviceItem,
      mutationsKey,
      callback
    );
  }
  // 公共模块添加
  store.modules = {
    attributes
  };
  return store;
};
