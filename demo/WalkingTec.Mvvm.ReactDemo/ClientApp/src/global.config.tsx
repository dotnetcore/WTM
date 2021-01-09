import { notification } from 'antd';
import { Settings } from 'app/layout/antdPro/SettingDrawer';
import ImgLogo from 'assets/img/logo.png';
import ImgUser from 'assets/img/user.png';
import lodash from 'lodash';
import { configure, observable, toJS } from 'mobx';
import { create, persist } from 'mobx-persist';
import themeColor from 'utils/themeColor';
import Help from 'utils/Help';
import moment from 'moment';
// import 'moment/locale/zh-cn';
import "./global.less";
// const language = lodash.get(window, 'navigator.language', 'zh-CN');
// mobx 严格模式 https://cn.mobx.js.org/refguide/api.html
configure({ enforceActions: "observed" });
notification.config({
    duration: 3,
    top: 60
});
const hydrate = create({
    storage: window.localStorage,   // 存储的对象
    jsonify: true, // 格式化 json
    // debounce: 1000,
});
// 环境变量 开发 模型
const development = process.env.NODE_ENV === "development"
class ConfigStore {
    constructor() {
        hydrate('WTM_GlobalConfig', this)
            // post hydration
            .then(() => {
                console.log('WTM_GlobalConfig', toJS(this));
                // 主题色
                themeColor.changeColor(this.settings.primaryColor);
                window['g_locale'] = this.language;
                // 日期中文
                moment.locale(lodash.toLower(this.language));
            })
    }
    buildTime = process.env.REACT_APP_TIME;
    /**
    * 开发环境
    */
    development = development;
    /**
     * 默认配置
     */
    default = {
        // title: "WalkingTec MVVM",
        logo: ImgLogo,
        avatar: ImgUser,
    };
    /**
     * 语言
     * @memberof ConfigStore
     */
    @persist
    @observable
    language: "zh-CN" | "en-US" = Help.GetNavigatorLanguage();
    /**
     * ant Pro 布局 设置  https://github.com/ant-design/ant-design-pro-layout/blob/master/README.zh-CN.md#MenuDataItem
     * @type {Settings}
     * @memberof ConfigStore
     */
    @persist("object")
    @observable
    settings: Settings = {
        // 导航的主题 'light' | 'dark'
        navTheme: 'dark',
        // layout 的菜单模式,sidemenu：右侧导航，topmenu：顶部导航 'sidemenu' | 'topmenu'
        layout: 'sidemenu',
        // layout 的内容模式,Fluid：定宽 1200px，Fixed：自适应
        contentWidth: 'Fluid',
        primaryColor: "#1890FF",
        // 是否固定 header 到顶部
        fixedHeader: true,
        // 是否下滑时自动隐藏 header
        autoHideHeader: false,
        // 是否固定导航
        fixSiderbar: true,
        // 关于 menu 的配置，暂时只有 locale,locale 可以关闭 menu 的自带的全球化
        menu: {
            locale: true,
        },
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
        agGridTheme: "ag-theme-balham",
        /**
         * 页签 页面
         */
        tabsPage: true,
    }
    /**
     * 服务器地址 前缀
     * process.env.NODE_ENV === "development" 根据 环境替换
     */
    target = "/api";
    /**
     * 请求头
     */
    getHeaders(headers?) {
        headers = lodash.merge({
            credentials: 'include',
            accept: "*/*",
            "Content-Type": "application/json",
            'Accept-Language': this.language,
            token: this.token.get()
        }, headers);
        console.log("ConfigStore -> getHeaders -> headers", headers)
        return headers
    };
    /**
     * token
     */
    token = {
        set(token) {
            window.localStorage.setItem('__token', token);
            return token
        },
        get() {
            return window.localStorage.getItem('__token') || null;
        },
        clear() {
            window.localStorage.removeItem("__User");
            //window.localStorage.clear();
            window.location.pathname = "/"
        }
    };
    /** 列表 分页 可选 行数 以下是默认值 */
    // pageSizeOptions: ['10', '20', '30', '40', '50', '100', '200'],
    /** 列表 行  */
    // @persist
    @observable
    Limit = 50;
    /** 
     * 详情信息 展示类型 
     */
    // @persist
    // @observable
    // infoType = "Modal";//Drawer || Modal
    /** 
    * 详情信息 展示 宽度
    */
    // @persist
    @observable
    infoTypeWidth = '900px';
    /**
     * 表单 item lable 占比
     * doc:https://ant.design/components/form-cn/
     */
    // @persist("object")
    @observable
    formItemLayout = {
        labelCol: {
            span: 6
        },
        wrapperCol: {
            span: 16
        },
    };
    /**
     * 详情信息 列 数 24 的除数
     */
    // @persist
    @observable
    infoColumnCount = 2;
    /**
    * 搜索 列 数 24 的除数
    */
    // @persist
    @observable
    searchColumnCount = 3;
    /**
     * 锁定表格滚动
     */
    // @persist
    @observable
    lockingTableRoll = true;
    /**
     * 静态页面 标记
     */
    // @persist
    @observable
    staticPage = "@StaticPage";
}
const GlobalConfig = new ConfigStore();

export default GlobalConfig
