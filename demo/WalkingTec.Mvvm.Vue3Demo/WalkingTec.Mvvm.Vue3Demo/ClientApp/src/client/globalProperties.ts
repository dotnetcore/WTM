import { message } from 'ant-design-vue';
import lodash from 'lodash';
import moment from 'moment';
import NProgress from 'nprogress';
import { AjaxRequest } from 'rxjs/ajax';
import { map } from 'rxjs/operators';
import { App } from 'vue';
import $WtmConfig, { WtmConfig } from './config';
import { WTM_ValueType } from './declare';
import { EnumActionType } from './enum';
import { AjaxBasics } from './helpers';
import i18n, { $locales, Enumlocales } from './locales';
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
        $WtmConfig: WtmConfig
        $Ajax: AjaxBasics
        $locales: Enumlocales
        moment: typeof moment
        $message: typeof message
        lodash: typeof lodash
        EnumActionType: typeof EnumActionType
        EnumValueType: typeof WTM_ValueType
        FieldRequest: typeof FieldRequest
    }
}
/**
 * 设置 Ajax Headers
 * @returns 
 */
AjaxBasics.onMergeHeaders = function () {
    return {
        'Accept-Language': i18n.global.locale,
    }
}
AjaxBasics.onNProgress = function (type) {
    if (type == "done") {
        NProgress.done();
    } else {
        NProgress.start();
    }
}
AjaxBasics.onError = function (error) {
    if (error.response) {
        // message.error('')
    }
}
export const $Ajax = new AjaxBasics({ target: $WtmConfig.target })
export function FieldRequest(request: string | AjaxRequest) {
    return $Ajax.request<any>(request).pipe(map(value => {
        return lodash.map(value, item => {
            return {
                key: lodash.get(item, 'Value'),
                label: lodash.get(item, 'Text'),
                value: lodash.get(item, 'Value'),
                ...item
            }
        })
    })).toPromise()
}
export const globalProperties = {
    $WtmConfig,
    $Ajax,
    $locales,
    moment,
    lodash,
    EnumActionType,
    EnumValueType: WTM_ValueType,
    FieldRequest
}
export default {
    install(app: App) {
        app.config.globalProperties = lodash.assign(app.config.globalProperties, globalProperties)
    }
}