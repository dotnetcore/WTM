/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import { Request } from 'utils/Request';
import { action, observable, runInAction } from "mobx";
import lodash from 'lodash';
import User from './user';
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
        let menu = [];
        if (User.User.role == "administrator") {
            const res = await import("../../subMenu.json");
            menu = res.subMenu;
            menu.push({
                "Key": "system",
                "Name": "系统设置",
                "Icon": "setting",
                "Path": "/system",
                "Component": "",
                "Children": []
            })
            
            console.log(lodash.flattenDeep(menu));
        }
        this.setSubMenu(menu);
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