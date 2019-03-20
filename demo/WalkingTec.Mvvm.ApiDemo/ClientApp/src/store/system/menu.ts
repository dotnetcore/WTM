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
                if (Regular.url.test(data.Path)) {
                    data.Path = "/external/" + encodeURIComponent(data.Path);
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