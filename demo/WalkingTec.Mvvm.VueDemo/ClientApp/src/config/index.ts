/**
 * 配置文件
 */
const config = {
  serverHost: "/wtm", // /api
  headerApi: "/api",
  /**
   * token 名称
   */
  tokenKey: "token",
  /**
   * global cookies
   */
  globalKey: "global",
  /**
   * 组件大小 medium / small / mini
   */
  elSize: "small",
  /**
   * iframe 嵌入页面标示
   */
  staticPage: "@StaticPage",

  /**
   * debugger调试
   */
  development: true
};
export default config;

/**
 * 默认样式配置
 */
export const style = {
  menuBg: "#304156",
  menuText: "#bfcbd9",
  menuActiveText: "#409eff",
  theme: "#1890ff"
};
