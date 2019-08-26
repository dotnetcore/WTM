import config from "@/config/index";
// import { contentType } from "@/config/enum";
const reqPath = config.headerApi + "/_login/";
// 验证登陆
const loginCheckLogin = {
    url: reqPath + "CheckLogin/{ID}",
    method: "get"
};
// 验证登出
const loginLogout = {
    url: reqPath + "Logout/{ID}",
    method: "get"
};

export default {
    loginCheckLogin,
    loginLogout
};
