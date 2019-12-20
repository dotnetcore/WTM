import { EntitiesTimeStore, EntitiesUserStore, EntitiesPageStore } from '@leng/public/src';
import router from './router';
import lodash from 'lodash';
const RootStore = {
    UserStore: new EntitiesUserStore(),
    TimeStore: new EntitiesTimeStore(),
}
RootStore.UserStore.onCheckLogin();
// 监控用户登录状态通知
RootStore.UserStore.UserObservable.subscribe((entitie) => {
    const isLogin = lodash.eq(router.currentRoute.path, '/login');
    if ((!entitie.Loading && !entitie.OnlineState)) {
        !isLogin && router.replace({ path: "/login" })
    } else {
        isLogin && router.replace({ path: "/" })
    }
});
export default RootStore