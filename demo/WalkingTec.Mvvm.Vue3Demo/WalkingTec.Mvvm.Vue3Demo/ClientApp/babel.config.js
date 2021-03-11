module.exports = {
  presets: [
    '@vue/cli-plugin-babel/preset'
  ],
  plugins: [
    // 'lodash',
    ['import', {
      libraryName: 'ant-design-vue',
      libraryDirectory: 'es',
      style: (name) => {
        return `${name}/style/index.js`
      },
      css: (name) => {
        return `${name}/style/index.js`
      }
    }],
  ]
}
