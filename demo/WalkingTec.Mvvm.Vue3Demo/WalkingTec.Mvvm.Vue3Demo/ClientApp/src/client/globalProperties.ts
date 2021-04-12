import { message } from 'ant-design-vue';
import lodash from 'lodash';
import moment from 'moment';
import { AjaxRequest } from 'rxjs/ajax';
import { map } from 'rxjs/operators';
import { App } from 'vue';
import $WtmConfig, { WtmConfig } from './config';
import { AjaxBasics } from './helpers';
import { EnumActionType } from './enum';
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
        $WtmConfig: WtmConfig
        $Ajax: AjaxBasics
        moment: typeof moment
        $message: typeof message
        lodash: typeof lodash
        EnumActionType: typeof EnumActionType
    }
}
export const $Ajax = new AjaxBasics({ target: $WtmConfig.target })
export function FieldRequest(request: string | AjaxRequest) {
    return $Ajax.request<any>(request).pipe(map(value => {
        return lodash.map(value, item => {
            return {
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
    moment,
    lodash,
    EnumActionType,
}
export default {
    install(app: App) {
        app.config.globalProperties = lodash.assign(app.config.globalProperties, globalProperties)
    }
}