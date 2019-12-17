import { EntitiesTimeStore, EntitiesUserStore, EntitiesPageStore } from '@leng/public/src';
const RootStore = {
    UserStore: new EntitiesUserStore(),
    TimeStore: new EntitiesTimeStore(),
    PageStore: new EntitiesPageStore({
        target: '/api',
        Search: { url: '/_FrameworkUserBase/Search', },
        Details: { url: '/_FrameworkUserBase/{id}' },
        Insert: { url: '/_FrameworkGroup/Add' },
        Update: { url: '/_FrameworkGroup/Edit' },
        Delete: { url: '/_FrameworkGroup/BatchDelete' },
        Export: { url: '/_FrameworkGroup/ExportExcel', body: {} },
    }),
}
RootStore.UserStore.onCheckLogin();
export default RootStore