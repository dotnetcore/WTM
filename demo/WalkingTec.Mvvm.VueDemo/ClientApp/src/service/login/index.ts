import config from "@/config/index";
import { contentType } from "@/config/enum";
/**
 * 接口
 */
// 登陆
const login = {
  url: config.headerApi + "/_Account/Login",
  method: "post",
  contentType: contentType.form
};
// 发送验证码
const verificationCode = {
  url: "/verification_code/send",
  method: "post"
};

// 验证登陆
const loginRemote = {
  url: "/api/_Account/LoginRemote",
  method: "get"
};

export default {
  login,
  verificationCode,
  loginRemote
};
