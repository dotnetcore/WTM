import { App } from 'vue';
import WtmAction from './page/action/index.vue';
import WtmDetails from './page/details.vue';
import WtmView from './page/view.vue';
import WtmGrid from './page/grid/index.vue';
import WtmFilter from './page/filter.vue';
import WtmField from './page/field/index.vue';
import antd from './plugins/antd';
import icon from './plugins/icon';
export default {
    install(app: App) {
        app.use(antd)
        app.use(icon)
        /**    use antd 组件    */
        app.component('WtmGrid', WtmGrid)
        app.component('WtmView', WtmView)
        app.component('WtmDetails', WtmDetails)
        app.component('WtmAction', WtmAction)
        app.component('WtmFilter', WtmFilter)
        app.component('WtmField', WtmField)
    }
}