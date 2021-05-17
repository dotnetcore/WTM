import { App } from 'vue';
import globalProperties from './globalProperties';
import locales from './locales';
import mixin from './mixin';
export default {
    install(app: App) {
        app.mixin(mixin)
        app.use(locales)
        app.use(globalProperties)
    }
}