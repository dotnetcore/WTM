import {
    DownloadOutlined,
    DownOutlined,
    EditOutlined,
    SaveOutlined,
    SearchOutlined,
    RedoOutlined,
    UserOutlined,
    DeleteOutlined,
    CloudUploadOutlined,
    CloudDownloadOutlined,
    SettingOutlined,
    FormOutlined,
    InboxOutlined,
    CheckOutlined,
    CloseOutlined,
    PlusOutlined,
    LockOutlined
} from '@ant-design/icons-vue';
import lodash from 'lodash';
import { App } from 'vue';
export default {
    install(app: App) {
        lodash.map({
            DownloadOutlined,
            LockOutlined,
            DownOutlined,
            EditOutlined,
            SaveOutlined,
            SearchOutlined,
            RedoOutlined,
            UserOutlined,
            DeleteOutlined,
            CloudUploadOutlined,
            CloudDownloadOutlined,
            SettingOutlined,
            InboxOutlined,
            CheckOutlined,
            CloseOutlined,
            PlusOutlined,
            FormOutlined
        }, (icon, key) => app.component(key, icon))
    }
}