// 由于 TypeScript 默认并不支持 *.vue 后缀的文件，ts识别vue
declare module '*.vue' {
  import Vue from 'vue'
  export default Vue
}
