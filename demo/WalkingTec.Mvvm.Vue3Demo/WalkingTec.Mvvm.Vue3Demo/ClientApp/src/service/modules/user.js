import config from "@/config/index";
var reqPath = config.headerApi + "/_Account/";
// 验证登陆
var loginCheckLogin = {
    url: reqPath + "CheckUserInfo",
    method: "get"
};
// 验证登出
var loginLogout = {
    url: reqPath + "Logout",
    method: "get"
};
var ChangePassword = {
    url: reqPath + "ChangePassword",
    method: "post"
};
export default {
    loginCheckLogin: loginCheckLogin,
    loginLogout: loginLogout,
    ChangePassword: ChangePassword
};
//# sourceMappingURL=user.js.map