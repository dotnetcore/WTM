import { App } from 'vue';
import locales from './locales';
import mixin from './mixin';
export default {
    install(app: App) {
        app.mixin(mixin)
        app.use(locales)
    }
}