import { notification } from 'antd';
import ImgLogo from 'assets/img/logo.png';
import ImgUser from 'assets/img/user.png';
import { configure, observable } from 'mobx';
import { create, persist } from 'mobx-persist';
import moment from 'moment';
import 'moment/locale/zh-cn';
import "./global.less";
// 日期中文
moment.locale('zh-cn');
// mobx 严格模式 https://cn.mobx.js.org/refguide/api.html
configure({ enforceActions: "observed" });
notification.config({
    duration: 3,
    top: 60
});
const hydrate = create({
    storage: window.localStorage,   // 存储的对象
    jsonify: true, // 格式化 json
    debounce: 1000,
});
// 环境变量 开发 模型
const development = process.env.NODE_ENV === "development"
class ConfigStore {
    constructor() {
        hydrate('WTM_GlobalConfig', this)
            // post hydration
            .then(() => console.log('some WTM_GlobalConfig', { ...this }))
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
        title: "WalkingTec MVVM",
        logo: ImgLogo,
        avatar: ImgUser,
    };
    /**
     * 服务器地址 前缀
     * process.env.NODE_ENV === "development" 根据 环境替换
     */
    target = "/api";
    /**
     * 请求头
     */
    headers = {
        credentials: 'include',
        accept: "*/*",
        "Content-Type": "application/json",
        "token": null
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
            window.localStorage.clear();
            window.location.reload()
        }
    };
    /** 列表 分页 可选 行数 以下是默认值 */
    // pageSizeOptions: ['10', '20', '30', '40', '50', '100', '200'],
    /** 列表 行  */
    @persist
    @observable
    Limit = 50;
    /** 
     * 详情信息 展示类型 
     */
    @persist
    @observable
    infoType = "Drawer";//Drawer || Modal
    /** 
    * 详情信息 展示 宽度
    */
    @persist
    @observable
    infoTypeWidth = '800px';
    /**
     * 表单 item lable 占比
     * doc:https://ant.design/components/form-cn/
     */
    @persist("object")
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
    @persist
    @observable
    infoColumnCount = 2;
    /**
    * 搜索 列 数 24 的除数
    */
    @persist
    @observable
    searchColumnCount = 3;
    /**
     * 锁定表格滚动
     */
    @persist
    @observable
    lockingTableRoll = true;
    /**
     * 菜单默认展开 true=收起
     */
    @persist
    @observable
    collapsed = false;
    /**
     * tabs 页面
     */
    @persist
    @observable
    tabsPage = true;
    /**
     * tabs 切换动画
     */
    tabsAnimated = false;
    /**
     * tabs 页签位置，可选值有 top right bottom left
     */
    @persist
    @observable
    tabPosition: 'top' | 'right' | 'bottom' | 'left' = "top";
    /**
     * 菜单类型  horizontal 头部  inline 左侧
     */
    @persist
    @observable
    menuMode: "horizontal" | "inline" = "inline";
    /**
     * 静态页面 标记
     */
    @persist
    @observable
    staticPage = "@StaticPage";
    /**
     * AgGrid 主题
     * ag-theme-balham
     * ag-theme-material
     */
    @persist
    @observable
    agGridTheme = "ag-theme-balham";
}
const GlobalConfig = new ConfigStore();

export default GlobalConfig