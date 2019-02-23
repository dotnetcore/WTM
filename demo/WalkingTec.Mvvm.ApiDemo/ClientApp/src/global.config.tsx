import "./global.less";
import { notification } from 'antd';
import ImgLogo from 'assets/img/logo.png';
import ImgUser from 'assets/img/user.png';
notification.config({
    duration: 3,
    top: 60
});
export default {
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
    infoType: "Modal",//Drawer || Modal
    /** 
    * 详情信息 展示 宽度
    */
    infoTypeWidth: 700,
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
}