/**
 * config-overrides.js
 * 重写 react-scripts 默认配置
 */
const configDev = require("./config/webpack.config.dev");
const configProd = require("./config/webpack.config.prod");
const moment = require('moment');
const lodash = require('lodash');
const path = require('path');
const fs = require('fs');
const ForkTsCheckerWebpackPlugin = require('react-dev-utils/ForkTsCheckerWebpackPlugin');
const appDirectory = fs.realpathSync(process.cwd());
process.env.REACT_APP_TIME = moment().format('YYYY-MM-DD HH:mm:ss');
// 魔改 react-scripts 默认配置
magicChange();
module.exports = (config, env) => {
  // 删除 typescript-eslint
  lodash.remove(config.module.rules, data => String(data.test) === String(/\.(js|mjs|jsx|ts|tsx)$/) && data.enforce === "pre");
  // 删除 ForkTsCheckerWebpackPlugin
  lodash.remove(config.plugins, data => data instanceof ForkTsCheckerWebpackPlugin);
  // 暂时 修复 antd Antd Webpack build failing (Can't resolve 'css-animation/es/Event') https://github.com/ant-design/ant-design/issues/17928
  // lodash.update(config, 'resolve.alias', value => {
  //   return { ...value, "css-animation/es/Event": "css-animation/dist-src/Event" }
  // })
  // 修改 ts 编译器
  lodash.update(config, 'module.rules[1].oneOf[1]', value => {
    return {
      test: /\.(tsx|ts|js|jsx)$/,
      include: value.include,
      use: [
        // 'cache-loader',
        {
          loader: 'awesome-typescript-loader',
          options: {
            useCache: true,
            configFileName: "tsconfig.compile.json",
            cacheDirectory: "node_modules/.cache/awcache",
            // transpileOnly: true,
            errorsAsWarnings: true,
            // reportFiles: [
            //   `${value.include}/**/*.{ts,tsx}`,
            //   ...packages.map(data => `${data}/**/*.{ts,tsx}`)
            // ]
            // usePrecompiledFiles: true,
          }
        }
      ]

    }
  });
  if (env == "development") {
    return configDev(config, env)
  } else {
    return configProd(config, env)
  }
}
/**
 * 魔改 react-scripts 默认配置
 */
function magicChange() {
  try {
    // 获取 脚手架启动目录
    const startPath = path.resolve(appDirectory, 'node_modules', 'react-scripts', 'scripts', 'start.js');
    // 文件存在
    if (fs.existsSync(startPath)) {
      console.log('------------------------------------ modify default setting ------------------------------------')
      const startjs = fs.readFileSync(startPath).toString();
      // 替换 代码
      const newStartjs = startjs.replace('const useTypeScript = fs.existsSync(paths.appTsConfig);', 'const useTypeScript = false ;// config-overrides.js 魔改 fs.existsSync(paths.appTsConfig);');
      fs.writeFileSync(startPath, newStartjs);
    }
  } catch (error) {
    console.log("modify failed", error);
  }
}