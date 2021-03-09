/**
 * 根据service中 创建store 初始结构
 * 注：store如果没有逻辑可以用
 * 目前只创建 state，actions，mutations 部分
 */
import _request from "@/util/service";
import attributes from "./attributes";
var stoBase = {
    // 接口列表key（service中）
    apiKeys: "actionList",
    // 返回命名
    getKeyName: function (key) {
        var upperKey = _.upperFirst(key);
        var mutationsKey = "set" + upperKey + "_mutations";
        var stateKey = key + "Data";
        return { mutationsKey: mutationsKey, stateKey: stateKey };
    },
    // service接口列表的名称作为state的key，并判断是否包含List，如果包含List定位数据
    stateDef: function (keyName, dataType) {
        if (keyName.indexOf("List") > -1 || dataType === "array") {
            return [];
        }
        else {
            return { obj: {} };
        }
    },
    /**
     *  store > mutations
     *  返回数据，优先使用Entity，如果类型不一样，自定义mutations
     */
    mutationsDef: function (stateKey) {
        return function (state, data) {
            // 接口返回数据结构 如果:{data: {}}
            state[stateKey] = data.Entity || data;
        };
    },
    // store > action
    actionsDef: function (serviceItem, mutationsKey, cb) {
        return function (_a, params) {
            var commit = _a.commit;
            var option = Object.assign({ data: params }, serviceItem);
            return _request(option, null).then(function (result) {
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
export default (function (serviceUnit, callback) {
    var _a;
    var store = {
        state: (_a = {}, _a[stoBase.apiKeys] = {}, _a),
        actions: {},
        mutations: {},
        modules: {}
    };
    for (var key in serviceUnit) {
        var serviceItem = serviceUnit[key];
        var _b = stoBase.getKeyName(key), mutationsKey = _b.mutationsKey, stateKey = _b.stateKey;
        // 接口列表
        store.state[stoBase.apiKeys][key] = serviceItem.url;
        //  get定义（state，mutations），post/put 不定义
        if (serviceItem.method === "get") {
            store.state[stateKey] = stoBase.stateDef(key, serviceItem.dataType);
            // mutations
            store.mutations[mutationsKey] = stoBase.mutationsDef(stateKey);
        }
        // actions instanceof
        store.actions[key] = stoBase.actionsDef(serviceItem, mutationsKey, callback);
    }
    // 公共模块添加
    store.modules = {
        attributes: attributes
    };
    return store;
});
//# sourceMappingURL=index.js.map