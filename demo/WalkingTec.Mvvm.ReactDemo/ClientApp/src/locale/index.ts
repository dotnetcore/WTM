import enus from './en-US/default';
import zhcn from './zh-CN/default';
import lodash from 'lodash';
import globalconfig from 'global.config';
import antdZhCN from 'antd/lib/locale-provider/zh_CN';
import antdEnUS from 'antd/lib/locale-provider/en_US';
const locales = {
    'zh-CN': {
        ...zhcn,
    },
    'en-US': {
        ...enus,
    }
};
/**
 * 获取当前配置语言 的 value
 *
 * @export
 * @param {string} key 
 * @param {*} [defaultValue] 默认值
 * @returns
 */
export function getLocalesValue(key: string, defaultValue?) {
    return lodash.get(getLocales(), key, defaultValue)
}
/**
 * 获取当前语言配置
 *
 * @export
 * @returns
 */
export function getLocales(language = globalconfig.language) {
    return lodash.get(locales, language)
}
export function getConfigProvider(language = globalconfig.language) {
    return lodash.get({
        'zh-CN': antdZhCN,
        'en-US': antdEnUS
    }, language, antdZhCN)
}
export default locales