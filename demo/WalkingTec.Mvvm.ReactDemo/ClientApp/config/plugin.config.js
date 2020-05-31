// Change theme plugin
// eslint-disable-next-line eslint-comments/abdeils - enable - pair;
/* eslint-disable import/no-extraneous-dependencies */
const ThemeColorReplacer = require('webpack-theme-color-replacer')
const generate = require('@ant-design/colors/lib/generate').default
const path = require('path');

// interface SelectorUtil {
//   changeEach(selector: string, surfix: string, prefix?: string): string;
// }

function getModulePackageName(module) {
  if (!module.context) return null;

  const nodeModulesPath = path.join(__dirname, '../node_modules/');
  if (module.context.substring(0, nodeModulesPath.length) !== nodeModulesPath) {
    return null;
  }

  const moduleRelativePath = module.context.substring(nodeModulesPath.length);
  const [moduleDirName] = moduleRelativePath.split(path.sep);
  let packageName = moduleDirName;
  // handle tree shaking
  if (packageName && packageName.match('^_')) {
    // eslint-disable-next-line prefer-destructuring
    packageName = packageName.match(/^_(@?[^@]+)/)[1];
  }
  return packageName;
}

const getAntdSerials = (color) => {
  const lightNum = 9;
  const devide10 = 10;
  // 淡化（即less的tint）
  const lightens = new Array(lightNum).fill(undefined).map((_, i) => {
    return ThemeColorReplacer.varyColor.lighten(color, i / devide10);
  });
  const colorPalettes = generate(color);
  const rgb = ThemeColorReplacer.varyColor.toNum3(color.replace('#', '')).join(',');
  return lightens.concat(colorPalettes).concat(rgb);
};
module.exports = new ThemeColorReplacer({
  fileName: 'static/css/theme-colors.css',
  matchColors: getAntdSerials('#1890ff'), // 主色系列
  injectCss: true,
  // isJsUgly: true,
  // 改变样式选择器，解决样式覆盖问题(最好能通过修改antd解决)
  changeSelector(selector, util) {
    switch (selector) {
      case '.ant-calendar-today .ant-calendar-date':
        return ':not(.ant-calendar-selected-date):not(.ant-calendar-selected-day)' + selector;
      case '.ant-btn:focus,.ant-btn:hover':
        return util.changeEach(selector, ':not(.ant-btn-primary):not(.ant-btn-danger)');
      case '.ant-btn.active,.ant-btn:active':
        return util.changeEach(selector, ':not(.ant-btn-primary):not(.ant-btn-danger)');
      case '.ant-steps-item-process .ant-steps-item-icon>.ant-steps-icon':
        return ':not(.ant-steps-item-process)' + selector;
      case '.ant-menu-horizontal>.ant-menu-item-active,.ant-menu-horizontal>.ant-menu-item-open,' +
        '.ant-menu-horizontal>.ant-menu-item-selected,.ant-menu-horizontal>.ant-menu-item:hover,' +
        '.ant-menu-horizontal>.ant-menu-submenu-active,.ant-menu-horizontal>.ant-menu-submenu-open,' +
        '.ant-menu-horizontal>.ant-menu-submenu-selected,.ant-menu-horizontal>.ant-menu-submenu:hover':
        return (
          '.ant-menu-horizontal>.ant-menu-item-active,' +
          '.ant-menu-horizontal>.ant-menu-item-open,' +
          '.ant-menu-horizontal>.ant-menu-item-selected,' +
          '.ant-menu-horizontal:not(.ant-menu-dark)>.ant-menu-item:hover,' +
          '.ant-menu-horizontal>.ant-menu-submenu-active,' +
          '.ant-menu-horizontal>.ant-menu-submenu-open,' +
          '.ant-menu-horizontal:not(.ant-menu-dark)>.ant-menu-submenu-selected,' +
          '.ant-menu-horizontal:not(.ant-menu-dark)>.ant-menu-submenu:hover'
        );
      case '.ant-menu-horizontal>.ant-menu-item-selected>a':
        return '.ant-menu-horizontal:not(ant-menu-light):not(.ant-menu-dark)>.ant-menu-item-selected>a';
      case '.ant-menu-horizontal>.ant-menu-item>a:hover':
        return '.ant-menu-horizontal:not(ant-menu-light):not(.ant-menu-dark)>.ant-menu-item>a:hover';
      default:
        return selector;
    }
  }
});
