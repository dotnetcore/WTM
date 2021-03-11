import { DownloadOutlined, EditOutlined } from '@ant-design/icons-vue';
import lodash from 'lodash';
import { App } from 'vue';
export default {
    install(app: App) {
        lodash.map({
            EditOutlined,
            DownloadOutlined
        }, (icon, key) => app.component(key, icon))
    }
}