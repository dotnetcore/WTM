import {
    DownloadOutlined,
    UpOutlined,
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
    LockOutlined,
    UploadOutlined,
    ArrowsAltOutlined,
    GlobalOutlined
} from '@ant-design/icons-vue';
import lodash from 'lodash';
import { App } from 'vue';
export default {
    install(app: App) {
        lodash.map({
            DownloadOutlined,
            LockOutlined,
            UpOutlined,
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
            UploadOutlined,
            FormOutlined,
            ArrowsAltOutlined,
            GlobalOutlined
        }, (icon, key) => app.component(key, icon))
    }
}