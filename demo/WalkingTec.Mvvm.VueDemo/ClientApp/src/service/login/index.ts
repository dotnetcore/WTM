import config from "@/config/index";
/**
 * 接口
 */
// 登陆
const login = {
    url: config.headApi + "/_login/login",
    method: "post"
};
// 发送验证码
const verificationCode = {
    url: "/verification_code/send",
    method: "post"
};

export default {
    login,
    verificationCode
};
