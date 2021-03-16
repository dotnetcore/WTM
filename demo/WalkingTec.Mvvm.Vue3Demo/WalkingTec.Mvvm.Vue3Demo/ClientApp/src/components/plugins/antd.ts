import {
    Avatar,
    Button,
    Col,
    Divider,
    Drawer,
    Empty,
    Form,
    Image,
    Input,
    InputNumber,
    Menu,
    Modal,
    Pagination,
    Row,
    Select,
    Space,
    Spin,
    Tabs,
    Tag,
    Transfer,
    Dropdown,
    Skeleton,
    DatePicker,
    ConfigProvider
} from 'ant-design-vue';
import ProLayout, { PageContainer } from '@ant-design-vue/pro-layout';
import { App } from 'vue';
export default {
    install(app: App) {
        /**    use antd 组件    */
        [
            Avatar,
            Button,
            Col,
            Divider,
            Drawer,
            Empty,
            Form,
            Image,
            Input,
            InputNumber,
            Menu,
            Modal,
            Pagination,
            Row,
            Select,
            Space,
            Spin,
            Tabs,
            Tag,
            Transfer,
            Dropdown,
            Skeleton,
            DatePicker,
            ConfigProvider
        ].map(x => app.use(x));
        app.use(ProLayout)
        app.component('PageContainer',PageContainer)
    }
}