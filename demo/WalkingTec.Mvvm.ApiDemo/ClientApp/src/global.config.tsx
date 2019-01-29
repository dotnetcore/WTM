import "./global.less";
import { notification } from 'antd';
notification.config({
    duration: 3,
    top: 60
});
export default {
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
     * 表单 item lable 占比
     * doc:https://ant.design/components/form-cn/
     */
    formItemLayout: {
        labelCol: {
            xs: { span: 24 },
            sm: { span: 6 },
        },
        wrapperCol: {
            xs: { span: 24 },
            sm: { span: 16 },
        },
    }

}