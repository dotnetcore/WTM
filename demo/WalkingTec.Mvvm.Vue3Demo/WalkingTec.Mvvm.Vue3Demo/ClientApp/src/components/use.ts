import ProLayout from '@ant-design-vue/pro-layout';
import { Button, Divider, Spin, Space, Form, Row, Col, Menu, Image, Tabs, Drawer, Modal, Empty, Pagination } from 'ant-design-vue';
import 'ant-design-vue/dist/antd.less';
import { App, defineAsyncComponent } from 'vue';
import WtmGrid from './grid/index.vue';
export default {
    install(app: App) {
        /**    use antd 组件    */
        [Button, Divider, Spin, Space, Form, Row, Col, Menu, Image, Tabs, Drawer, Modal, Empty, Pagination, ProLayout].map(x => app.use(x))
        /**    use antd 组件    */
        app.component('WtmGrid', WtmGrid)
    }
}