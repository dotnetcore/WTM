/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import Regular from 'utils/Regular';
import { action, observable, runInAction } from "mobx";
import lodash from 'lodash';
import User from './user';
import globalConfig from 'global.config';
interface subMenu {
    Key?: string,
    Name?: string,
    Icon?: string,
    Path?: string,
    Component?: string,
    Children?: any[],
    // [key: string]: any
}
class Store {
    constructor() {
        this.getMenu()
    }
    /** 菜单展开 收起 */
    @observable collapsed = false;
    /** 菜单 */
    @observable subMenu: subMenu[] = [];
    /**
     * 获取菜单
     */
    async getMenu() {
        if (globalConfig.development) {
            const res: any[] = await import("../../subMenu.json").then(x => x.default);
            this.setSubMenu(lodash.map(res, data => {
                // 跨域页面
                if (Regular.url.test(data.Path)) {
                    data.Path = "/external/" + encodeURIComponent(data.Path);
                }
                // public 下的 pages 页面
                if (lodash.includes(data.Path, globalConfig.staticPage)) {
                    data.Path = "/external/" + encodeURIComponent(lodash.replace(data.Path, globalConfig.staticPage, `${window.location.origin}/pages`));
                }
                // public 下的 pages 页面
                if (lodash.includes(data.Path, globalConfig.dynamicPage)) {
                    data.Path = "/external/" + encodeURIComponent(lodash.replace(data.Path, globalConfig.dynamicPage, `${window.location.origin}`));
                }
                return data;
            }));
        }
    }

    /**  设置菜单 */
    @action.bound
    setSubMenu(subMenu) {
        this.subMenu = subMenu;
    }
    /** 菜单收起 展开 */
    @action.bound
    toggleCollapsed() {
        this.collapsed = !this.collapsed;
        dispatchEvent(new CustomEvent('resize'));
    }

}
export default new Store();