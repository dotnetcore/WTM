/**
 * config-overrides.js
 * 重写 react-scripts 默认配置
 */
const moment = require('moment');
const lodash = require('lodash');
const path = require('path');

const webpack = require('webpack');

const fs = require('fs');
const { override, fixBabelImports, addLessLoader, disableEsLint, addBundleVisualizer, removeModuleScopePlugin, addWebpackPlugin } = require('customize-cra');
const appDirectory = fs.realpathSync(process.cwd());
const ThemeColorReplacer = require('./config/plugin.config');
const createFonts = require('./config/createFonts');
process.argv.map(val => {
  if (lodash.endsWith(val, 'scripts/build.js') && lodash.isNil(process.env.GENERATE_SOURCEMAP)) {
    process.env.GENERATE_SOURCEMAP = 'false';
  }
})
process.env.REACT_APP_TIME = moment().format('YYYY-MM-DD HH:mm:ss') + ' @冷（lengyingxin8966@gmail.com）';
// process.env.GENERATE_SOURCEMAP = 'false';
// process.env.TSC_COMPILE_ON_ERROR = 'true';
// process.stdout.isTTY = false;
createFonts();
module.exports = (...params) => {
  const config = override(
    disableEsLint(),

    addWebpackPlugin(
      ThemeColorReplacer,
      new webpack.ContextReplacementPlugin(/moment[/\\]locale$/, /en|zh-cn/),
    ),

    fixBabelImports('import', {
      libraryName: 'antd',
      libraryDirectory: 'es',
      style: true,
    }),
    fixBabelImports("lodash", {
      libraryDirectory: "",
      camel2DashComponentName: false
    }),
    addLessLoader({
      javascriptEnabled: true,
      // modifyVars: { '@primary-color': '#1DA57A' },
      localIdentName: "app-[local]-[hash:base64:5]"
    }),
    // 查看 构建文件 大小 分布地图
    addBundleVisualizer({
      "analyzerMode": "static",
      "reportFilename": "report.html"
    }, true),
    removeModuleScopePlugin()
  )(...params);
  [
    // {
    //   name: 'app',
    //   // test: /\.(tsx|ts)$/,
    //   // test: /[\\/]src[\\/](pages)[\\/]/,
    //   test: /([\\/]src\/pages[\\/])/,
    //   chunks: 'all',
    //   // enforce: true // 强制忽略minChunks等设置
    // },
    {
      name: 'grid',
      test: /[\\/]node_modules[\\/](ag-grid-.*)[\\/]/,
      chunks: 'all',
    },
    {
      name: 'echarts',
      test: /[\\/]node_modules[\\/](echarts|echarts-for-react|zrender)[\\/]/,
      chunks: 'all',
    },
    {
      name: 'antd',
      test: /[\\/]node_modules[\\/](antd|@ant-design|rc-.*|rmc-.*|size-sensor|fast-deep-equal)[\\/]/,
      chunks: 'all',
    }
  ].map(item => {
    // 随机一个名字
    const { name } = item;
    if (config.mode === "production") {
      item.name = lodash.uniqueId()
    }
    lodash.set(config, `optimization.splitChunks.cacheGroups.${name}`, item);
  })
  // 清理  drop_console 日志
  lodash.set(config, 'optimization.minimizer[0].options.terserOptions.compress.drop_console', config.mode === "production");
  return config;
}
/**
 * 魔改 react-scripts 默认配置
 */
// function magicChange() {
//   try {
//     // 获取 脚手架启动目录
//     const startPath = path.resolve(appDirectory, 'node_modules', 'react-scripts', 'scripts', 'start.js');
//     // 文件存在
//     if (fs.existsSync(startPath)) {
//       console.log('------------------------------------ modify default setting ------------------------------------')
//       const startjs = fs.readFileSync(startPath).toString();
//       // 替换 代码
//       const newStartjs = startjs.replace('const useTypeScript = fs.existsSync(paths.appTsConfig);', 'const useTypeScript = false ;// config-overrides.js 魔改 fs.existsSync(paths.appTsConfig);');
//       fs.writeFileSync(startPath, newStartjs);
//     }
//   } catch (error) {
//     console.log("modify failed", error);
//   }
// }