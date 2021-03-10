import { message } from 'ant-design-vue';
import { extend } from 'lodash-es';
import moment from 'moment';
import { App } from 'vue';
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
        moment: typeof moment
        $message: typeof message
    }
}

export default {
    install(app: App) {
        app.config.globalProperties = extend(app.config.globalProperties, { moment })
    }
}