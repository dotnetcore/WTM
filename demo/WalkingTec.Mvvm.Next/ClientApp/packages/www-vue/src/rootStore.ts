import { EntitiesTimeStore, EntitiesUserStore } from '@leng/public/src';
import { runInAction } from 'mobx';
import Ajax from '@leng/public/src/utils/request';
import lodash from 'lodash';
import { map } from 'rxjs/operators';
import Globalconfig from './global.config';

const RootStore = {
    UserStore: new EntitiesUserStore(),
    // TimeStore: new EntitiesTimeStore(),
}
/**    def 获取 测试菜单    */
if (Globalconfig.development) {
    const devSubMenu = Ajax.ajax('/subMenu.json').pipe(map((data: any[]) => {
        return data.map(x => ({
            ...x,
            key: x.Id,
            path: x.Url || '',
            name: x.Text,
            icon: x.Icon || "pic-right",
        }))
    })).toPromise();
    const onAnalysisMenus = RootStore.UserStore.onAnalysisMenus;
    RootStore.UserStore.onAnalysisMenus = async function () {
        const subMenu = await devSubMenu;
        onAnalysisMenus(subMenu)
    }
    // RootStore.UserStore.UserObservable.subscribe(async (entitie) => {
    //     if (entitie.OnlineState) {
    //         const subMenu = await devSubMenu
    //         RootStore.UserStore.onAnalysisMenus(subMenu);
    //     }
    // });
}
/**    def 获取 测试菜单    */


RootStore.UserStore.onCheckLogin();

export default RootStore