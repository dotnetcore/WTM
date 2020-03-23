import config from "@/config/index";
import { contentType } from "@/config/enum";
/**
 * 接口
 */
// 登陆
const login = {
    url: config.headerApi + "/_login/login",
    method: "post",
    contentType: contentType.form
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
