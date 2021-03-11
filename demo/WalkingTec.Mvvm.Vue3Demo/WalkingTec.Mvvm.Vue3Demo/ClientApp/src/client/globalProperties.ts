import { message } from 'ant-design-vue';
import lodash from 'lodash';
import moment from 'moment';
import { App } from 'vue';
import $WtmConfig, { WtmConfig } from './config';
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
        $WtmConfig: WtmConfig
        moment: typeof moment
        $message: typeof message
        lodash: typeof lodash
    }
}
const globalProperties = {
    $WtmConfig,
    moment,
    lodash,
}
export default {
    install(app: App) {
        app.config.globalProperties = lodash.assign(app.config.globalProperties, globalProperties)
    }
}