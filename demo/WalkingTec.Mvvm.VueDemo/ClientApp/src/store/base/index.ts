/**
 * 根据service 创建store 注：store如果没有逻辑可以用
 *
 * 目前创建 state，actions，mutations 部分
 */
import service from "@/service/service";
import { firstUpperCase } from "@/util/string";

interface StoreType {
    state: {};
    actions: {};
    mutations: {};
}
// interface serviceItemInter {
//     url: string;
//     method: string;
//     isBuffer?: boolean;
//     contentType?: string;
//     dataType?: string;
// }
/**
 * 根据service 创建store
 * @param {*} serviceUnit: service接口列表
 */
export default (serviceUnit, callback?: Function) => {
    const store: StoreType = {
        state: {},
        actions: {},
        mutations: {}
    };
    for (const key in serviceUnit) {
        const serviceItem = serviceUnit[key];
        const upperKey = firstUpperCase(key);
        const mutationsKey = `set${upperKey}_mutations`;
        const stateKey = upperKey + "Data";
        // const actionsKey = serviceItem.method + upperKey;
        //  get定义（state，mutations），post/put 不定义
        if (serviceItem.method === "get") {
            // service接口列表的名称作为state的key，并判断是否包含List，如果包含List定位数据
            if (key.indexOf("List") > -1 || serviceItem.dataType === "array") {
                store.state[stateKey] = [];
            } else {
                store.state[stateKey] = { obj: {} };
            }
            // mutations
            store.mutations[mutationsKey] = (state, data) => {
                // 接口返回数据结构 如果:{data: {}}
                console.log("----", key, data.Entity || data);
                state[stateKey] = data.Entity || data;
            };
        }

        // actions instanceof
        store.actions[key] = ({ commit }, params) => {
            const option = Object.assign({ data: params }, serviceItem);
            return service(option, null).then(result => {
                if (serviceItem.method === "get") {
                    commit(mutationsKey, result || {});
                    // 判断是否回调方法
                    callback && callback(result, commit);
                }
                return result || {};
            });
        };
    }
    return store;
};
