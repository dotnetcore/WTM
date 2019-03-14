import { notification, Modal } from 'antd';
import ImgLogo from 'assets/img/logo.png';
import ImgUser from 'assets/img/user.png';
import { configure } from 'mobx';
import "./global.less";
// mobx 严格模式 https://cn.mobx.js.org/refguide/api.html
configure({ enforceActions: "observed" });
notification.config({
    duration: 3,
    top: 60
});
const development = process.env.NODE_ENV === "development"
if (development && 'ActiveXObject' in window) {
    Modal.confirm({
        title: "求求您，别用IE了~"
    })
}
export default {
    /**
     * 开发环境
     */
    development: development,
    /**
     * 默认配置
     */
    default: {
        title: "WalkingTec MVVM",
        logo: ImgLogo,
        avatar: ImgUser,
    },
    /**
     * 服务器地址 前缀
     * process.env.NODE_ENV === "development" 根据 环境替换
     */
    target: "/api",
    /**
     * 请求头
     */
    headers: {
        credentials: 'include',
        accept: "*/*",
        "Content-Type": "application/json",
        "token": null
    },
    /**
     * token
     */
    token: {
        set(token) {
            window.localStorage.setItem('__token', token);
            return token
        },
        get() {
            return window.localStorage.getItem('__token') || null;
        },
    },

    /** 
     * 详情信息 展示类型 
     */
    infoType: "Drawer",//Drawer || Modal
    /** 
    * 详情信息 展示 宽度
    */
    infoTypeWidth: '800px',
    /**
     * 表单 item lable 占比
     * doc:https://ant.design/components/form-cn/
     */
    formItemLayout: {
        labelCol: {
            span: 6
        },
        wrapperCol: {
            span: 16
        },
    },
    /**
     * 详情信息 列 数 24 的除数
     */
    infoColumnCount: 2,
    /**
    * 搜索 列 数 24 的除数
    */
    searchColumnCount: 3,
    /**
     * 锁定表格滚动
     */
    lockingTableRoll: true
}