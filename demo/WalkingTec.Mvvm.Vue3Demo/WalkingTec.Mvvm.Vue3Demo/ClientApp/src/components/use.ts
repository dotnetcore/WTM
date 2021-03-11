import { App } from 'vue';
import WtmAction from './page/action/index.vue';
import WtmDetails from './page/details.vue';
import WtmGrid from './page/grid/index.vue';
import antd from './plugins/antd';
import icon from './plugins/icon';
export default {
    install(app: App) {
        app.use(antd)
        app.use(icon)
        /**    use antd 组件    */
        app.component('WtmGrid', WtmGrid)
        app.component('WtmDetails', WtmDetails)
        app.component('WtmAction', WtmAction)
    }
}