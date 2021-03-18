import {
    DownloadOutlined,
    DownOutlined,
    EditOutlined,
    SaveOutlined,
    SearchOutlined,
    RedoOutlined,
    UserOutlined
} from '@ant-design/icons-vue';
import lodash from 'lodash';
import { App } from 'vue';
export default {
    install(app: App) {
        lodash.map({
            DownloadOutlined,
            DownOutlined,
            EditOutlined,
            SaveOutlined,
            SearchOutlined,
            RedoOutlined,
            UserOutlined
        }, (icon, key) => app.component(key, icon))
    }
}