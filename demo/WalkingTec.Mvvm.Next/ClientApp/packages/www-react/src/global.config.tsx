import lodash from 'lodash';
import { configure, observable, toJS } from 'mobx';
import { create, persist } from 'mobx-persist';
import { BindAll } from 'lodash-decorators';
const hydrate = create({
    storage: window.localStorage,   // 存储的对象
    jsonify: true, // 格式化 json
    debounce: 100,
});
configure({ enforceActions: 'observed' })
// 环境变量 开发 模型
const development = process.env.NODE_ENV === "development";
@BindAll()
class ConfigStore {
    constructor() {
        this.createHydrate('GlobalConfig', this);
    }
    title = 'NoCode'
    /**
    * 开发环境
    */
    @persist
    @observable
    development = development;
    /**
     * 创建 存储
     * @param key 
     * @param pointer 
     */
    createHydrate(key: string, pointer: any) {
        key = `${this.title}-${key}`
        hydrate(key, pointer).then(() => {
            development && console.warn(key, pointer)
        })
    }
}
export default new ConfigStore();
