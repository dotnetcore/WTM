import {
    Affix,
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
    Checkbox,
    Pagination,
    Row,
    Radio,
    Select,
    Space,
    Spin,
    Tabs,
    Tag,
    Popconfirm,
    Transfer,
    Dropdown,
    Layout,
    Skeleton,
    Switch,
    Slider,
    Upload,
    DatePicker,
    message,
    Rate,
    Result,
    List,
    Card,
    Collapse,
    Descriptions,
    Tree,
    ConfigProvider
} from 'ant-design-vue';
import ProLayout, { PageContainer } from '@ant-design-vue/pro-layout';
import { App, h } from 'vue';
export default {
    install(app: App) {
        /**    use antd 组件    */
        [
            Affix,
            Avatar,
            Popconfirm,
            Button,
            Col,
            Layout,
            Divider,
            Upload,
            Drawer,
            Empty,
            Form,
            Image,
            Input,
            Checkbox,
            InputNumber,
            Menu,
            Modal,
            Pagination,
            Row,
            Select,
            Radio,
            Space,
            Spin,
            Tabs,
            Tag,
            Transfer,
            Dropdown,
            Skeleton,
            Switch,
            Slider,
            DatePicker,
            Rate,
            Result,
            List,
            Card,
            Collapse,
            Descriptions,
            Tree,
            ConfigProvider
        ].map(x => app.use(x));
        // Spin.setDefaultIndicator({
        //     indicator: h('i', { class: 'anticon anticon-loading anticon-spin ant-spin-dot' }),
        // });
        app.use(ProLayout)
        app.component('PageContainer', PageContainer)
        app.config.globalProperties.$message = message;

    }
}