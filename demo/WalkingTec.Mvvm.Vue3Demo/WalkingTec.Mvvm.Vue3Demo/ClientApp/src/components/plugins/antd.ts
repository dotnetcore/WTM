import ProLayout from '@ant-design-vue/pro-layout';
import { Button, Col, Divider, Drawer, Empty, Form, Image, Menu, Modal, Pagination, Row, Space, Spin, Tabs } from 'ant-design-vue';
import { App } from 'vue';
export default {
    install(app: App) {
        /**    use antd 组件    */
        [Button, Divider, Spin, Space, Form, Row, Col, Menu, Image, Tabs, Drawer, Modal, Empty, Pagination, ProLayout].map(x => app.use(x));
    }
}