import { EntitiesTimeStore, EntitiesUserStore } from '@leng/public/src';
const RootStore = {
    UserStore: new EntitiesUserStore(),
    TimeStore: new EntitiesTimeStore(),
}
RootStore.UserStore.onCheckLogin();

export default RootStore