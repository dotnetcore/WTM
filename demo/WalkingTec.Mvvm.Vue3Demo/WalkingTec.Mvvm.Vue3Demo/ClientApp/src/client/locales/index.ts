import defaultEn from './en/default';
import defaultZh from './zh/default';
import lodash from 'lodash';
import { createI18n } from 'vue-i18n'

const locales = {
    en: lodash.assign({}, defaultEn),
    zh: lodash.assign({}, defaultZh),
}
// 获取 page 中所有的 语言文件
const files = require.context('@/pages', true, /locales.ts$/) // 根据目录结构去搜索文件
files.keys().map(x => {
    const locale = files(x).default
    lodash.assign(locales.en, locale.en)
    lodash.assign(locales.zh, locale.zh)
})
const i18n = createI18n({
    locale: 'zh', // set locale
    fallbackLocale: 'en', // set fallback locale
    messages: locales, // set locale messages
    // If you need to specify other options, you can set other options
    // ...
})
export default i18n

