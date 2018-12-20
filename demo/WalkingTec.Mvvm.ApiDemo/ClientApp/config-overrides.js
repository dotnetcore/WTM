/**
 * config-overrides.js
 * 重写 react-scripts 默认配置
 */
const configDev = require("./config/webpack.config.dev");
const configProd = require("./config/webpack.config.prod");
module.exports = function override(config, env) {
  if (env == "development") {
    return configDev(config, env)
  } else {
    return configProd(config, env)
  }
}