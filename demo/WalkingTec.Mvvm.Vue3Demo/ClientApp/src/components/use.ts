import { App, defineAsyncComponent } from 'vue';
import WtmAction from './page/action/index.vue';
import WtmDetails from './page/details/index.vue';
import WtmView from './page/view.vue';
import WtmGrid from './page/grid/index.vue';
import WtmGridLoading from './page/grid/loading.vue';
import { frameworkComponents } from './page/grid/framework';
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
        app.component('WtmAgGrid', defineAsyncComponent({
            loader: () => import("./page/grid/grid.vue"),
            loadingComponent: WtmGridLoading,
            delay: 0,
        }))
        app.component('WtmEcharts', defineAsyncComponent({
            loader: () => import("./page/echarts.vue"),
            delay: 0,
        }))
        app.component('WtmView', WtmView)
        app.component('WtmDetails', WtmDetails)
        app.component('WtmAction', WtmAction)
        app.component('WtmFilter', WtmFilter)
        app.component('WtmField', WtmField)
        app.config.globalProperties.$FrameworkComponents = frameworkComponents
    }
}