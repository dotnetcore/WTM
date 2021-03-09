import config from "@/config/index";
import { contentType } from "@/config/enum";
/**
 * 接口
 */
// 登陆
var login = {
    url: config.headerApi + "/_Account/Login",
    method: "post",
    contentType: contentType.form
};
// 发送验证码
var verificationCode = {
    url: "/verification_code/send",
    method: "post"
};
export default {
    login: login,
    verificationCode: verificationCode
};
//# sourceMappingURL=index.js.map