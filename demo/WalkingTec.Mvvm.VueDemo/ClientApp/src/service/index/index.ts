import config from "@/config/index";
// import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_login/";
// 验证登陆
const loginCheckLogin = ({ ID }) => {
    return {
        url: reqPath + "CheckLogin/" + ID,
        method: "get"
    };
};
// 验证登出
const loginLogout = ({ ID }) => {
    return {
        url: reqPath + "Logout/" + ID,
        method: "get"
    };
};

export default {
    loginCheckLogin,
    loginLogout
};
