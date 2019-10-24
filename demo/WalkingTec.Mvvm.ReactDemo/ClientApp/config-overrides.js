/**
 * config-overrides.js
 * 重写 react-scripts 默认配置
 */
const moment = require('moment');
const lodash = require('lodash');
const path = require('path');
const fs = require('fs');
const ForkTsCheckerWebpackPlugin = require('react-dev-utils/ForkTsCheckerWebpackPlugin');
const { override, fixBabelImports, addLessLoader } = require('customize-cra');
const appDirectory = fs.realpathSync(process.cwd());
process.env.REACT_APP_TIME = moment().format('YYYY-MM-DD HH:mm:ss');
// process.stdout.isTTY = false;
// 魔改 react-scripts 默认配置
magicChange();
getFont();
module.exports = (...params) => {
  const config = override(
    fixBabelImports('import', {
      libraryName: 'antd',
      libraryDirectory: 'es',
      style: true,
    }),
    addLessLoader({
      javascriptEnabled: true,
      // modifyVars: { '@primary-color': '#1DA57A' },
      localIdentName: "app-[local]-[hash:base64:5]"
    }),
  )(...params);
  // 删除 typescript-eslint
  lodash.remove(config.module.rules, data => String(data.test) === String(/\.(js|mjs|jsx|ts|tsx)$/) && data.enforce === "pre");
  // 删除 ForkTsCheckerWebpackPlugin
  lodash.remove(config.plugins, data => data instanceof ForkTsCheckerWebpackPlugin);
  // 暂时 修复 antd Antd Webpack build failing (Can't resolve 'css-animation/es/Event') https://github.com/ant-design/ant-design/issues/17928
  // lodash.update(config, 'resolve.alias', value => {
  //   return { ...value, "css-animation/es/Event": "css-animation/dist-src/Event" }
  // })
  // 修改 ts 编译器
  // lodash.update(config, 'module.rules[1].oneOf[1]', value => {
  //   return {
  //     test: /\.(tsx|ts|js|jsx)$/,
  //     include: value.include,
  //     use: [
  //       // 'cache-loader',
  //       {
  //         loader: 'awesome-typescript-loader',
  //         options: {
  //           useCache: true,
  //           configFileName: "tsconfig.app.json",
  //           cacheDirectory: "node_modules/.cache/awcache",
  //           // transpileOnly: true,
  //           errorsAsWarnings: true,
  //           // reportFiles: [
  //           //   `${value.include}/**/*.{ts,tsx}`,
  //           //   ...packages.map(data => `${data}/**/*.{ts,tsx}`)
  //           // ]
  //           // usePrecompiledFiles: true,
  //         }
  //       }
  //     ]
  //   }
  // });
  return config;
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
/**
 * 读取配置 字体文件信息
 */
function getFont() {
  try {
    // 获取 字体文件根路径
    const fontRootDir = path.resolve(appDirectory, 'src', 'assets', 'font');
    const fontjs = path.resolve(appDirectory, 'src', 'assets', 'font', 'font.ts');
    const fontArray = [];
    console.log('------------------------------------ create font ------------------------------------')
    // 读取目录结构，获取字体
    fs.readdirSync(fontRootDir, { withFileTypes: true }).map(dirent => {
      // 是否为目录
      if (dirent.isDirectory()) {
        const font = { name: dirent.name, class: '', icons: [] };
        const fontPath = path.resolve(fontRootDir, dirent.name, 'iconfont.css');
        const fileCssStr = fs.readFileSync(fontPath, { encoding: 'utf-8' });
        /@font-face\s{0,}{\s{0,}('|"|)font-family(\1)\s{0,}:\s{0,}('|"|)([a-zA-Z0-9-_.#]{1,})(\3)\s{0,};/.exec(fileCssStr)
        font.class = RegExp.$4;
        const classReg = new RegExp(`.(${font.class}-([a-zA-Z0-9-_.#]{1,}))\\s{0,}:before\\s{0,}{`, "g");
        let regArray;
        while (regArray = classReg.exec(fileCssStr)) {
          font.icons.push(RegExp.$1)
        };
        fontArray.push(font);
      }
    });
    const fontjsStr = `${fontArray.map(x => `import './${x.name}/iconfont.css'`).join(";\n")};\nexport default ${JSON.stringify(fontArray, null, 4)} `;
    fs.writeFileSync(fontjs, fontjsStr, { encoding: 'utf-8' });
  } catch (error) {
    console.log("create font", error);
  }
  // throw "抛个错误"
}