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
const locale = localStorage.getItem('locale') || 'zh'
const i18n = createI18n({
    locale: locale, // set locale
    fallbackLocale: 'en', // set fallback locale
    messages: locales, // set locale messages
    // If you need to specify other options, you can set other options
    // ...
})

export const $i18n = i18n.global
/**
 * 转换 Rules 信息
 * @param label 
 * @param localesKey 默认 tips.error.required
 * @returns 
 */
$i18n.toRulesMessage = function (label, localesKey = 'tips_error_required') {
    try {
        return $i18n.t(localesKey, { label: $i18n.t(label) })
    } catch (error) {
        console.log("LENG ~ error", error, label)
    }
}
/**
 * 转换 Placeholder 信息
 * @param label 
 * @param localesKey 默认 tips.placeholder.input
 * @returns 
 */
$i18n.toPlaceholder = function (label, localesKey = 'tips_placeholder_input') {
    try {
        return $i18n.t(localesKey, { label: $i18n.t(label) })
    } catch (error) {
        console.log("LENG ~ error", error, label)
    }
}
export declare type Enumlocales = Readonly<typeof defaultZh>
export const $locales = lodash.mapValues(lodash.cloneDeep(defaultZh), (val, key) => key)
export default i18n


declare module 'vue-i18n' {
    interface VueI18n {
        /**
         * 转换 Rules 信息
         * @param label 
         * @param localesKey 默认 tips.error.required
         * @returns 
         */
        toRulesMessage: (label, localesKey?) => string
        /**
         * 转换 Placeholder 信息
         * @param label 
         * @param localesKey 默认 tips.placeholder.input
         * @returns 
         */
        toPlaceholder: (label, localesKey?) => string
    }
}
