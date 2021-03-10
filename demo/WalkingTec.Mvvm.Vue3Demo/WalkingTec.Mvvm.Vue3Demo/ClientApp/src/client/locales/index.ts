import defaultEn from './en/default';
import defaultZh from './zh/default';
import { assign } from 'lodash-es';
import { createI18n } from 'vue-i18n'

const messages = {
    en: assign({}, defaultEn),
    zh: assign({}, defaultZh),
}
const i18n = createI18n({
    locale: 'zh', // set locale
    fallbackLocale: 'en', // set fallback locale
    messages, // set locale messages
    // If you need to specify other options, you can set other options
    // ...
})
export default i18n

