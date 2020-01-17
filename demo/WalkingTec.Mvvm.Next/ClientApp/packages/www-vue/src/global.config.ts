import { configure, observable, action } from 'mobx';
import { create, persist } from 'mobx-persist';
import { notification } from 'ant-design-vue';
import lodash from 'lodash';
import { Request } from '@leng/public/src';
import 'moment/locale/zh-cn';
import moment from 'moment';
moment.locale('zh-cn');
// 设置 请求出错 通知
Request.Error = (error) => {
    notification.error({
        key: 'RequestError' + error.status,
        description: error.name,
        message: error.message
    })
}
// mobx 严格模式 https://cn.mobx.js.org/refguide/api.html
configure({ enforceActions: "observed" });
// 是否被 Iframe 嵌套
const isIframe = window.self !== window.top;
class ConfigStore {
    constructor() {
       
    }
    hydrate = create({
        storage: window.localStorage,   // 存储的对象
        jsonify: true, // 格式化 json
        // debounce: 1000,
    });
    /**
    * 开发环境
    */
    development = process.env.NODE_ENV === "development";
    /**
     * ant Pro 布局 设置  https://github.com/ant-design/ant-design-pro-layout/blob/master/README.zh-CN.md#MenuDataItem
     * @type {Settings}
     * @memberof ConfigStore
     */
    @persist("object")
    @observable
    settings = {
        language: lodash.get(window, 'navigator.language', 'zh-CN'),
        // layout 的 左上角 的 title
        title: 'WalkingTec MVVM',
        // 使用 IconFont 的图标配置
        iconfontUrl: '',
        // 弹框类型
        infoType: "Modal",
        /**
        * AgGrid 主题
        * ag-theme-balham
        * ag-theme-material
        */
        agGridTheme: "ag-theme-material",
        /**
         * 页签 页面
         */
        tabsPage: true,
    }
    /**
     * 分页默认 条数
     */
    defaultPageSize = 20;
    @action
    onSetLanguage(language) {
        this.settings.language = language;
        // 不清楚 为啥改了 没效果 执行页面刷新
        window.location.reload()
    }
}
const GlobalConfig = new ConfigStore();

export default GlobalConfig