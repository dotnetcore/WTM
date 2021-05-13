import lodash from "lodash";
import { action, observable, toJS } from "mobx"
import { create, persist } from "mobx-persist"
/**
 * 用户菜单
 */
export class UserMenus {
    constructor() {
    }
    /** 菜单 */
    private menus: Array<any> = [];
    // 平行数据菜单
    private ParallelMenu: Array<any> = [];
    // @action.bound
    onInit(ParallelMenu) {
        if (ParallelMenu) {
            ParallelMenu = lodash.map(ParallelMenu, item => {
                return lodash.assign({ ParentId: null }, lodash.cloneDeep(item))
            })
            const menus = this.recursionTree(ParallelMenu, null, [])
            this.menus = lodash.clone(menus);
            this.ParallelMenu = lodash.clone(ParallelMenu);
        }
    }
    getMenus() {
        return this.menus
    }
    /**
     * 递归 格式化 树
     * @param datalist 
     * @param ParentId 
     * @param children 
     */
    recursionTree(datalist, ParentId, children = []) {
        lodash.filter(datalist, ['ParentId', ParentId]).map(data => {
            const router = {
                path: data.Url,
                name: data.Url ? undefined : data.Id,
            }
            data = lodash.assign(router, {
                router: { to: router },
                meta: {
                    // icon: data.Icon, 
                    title: data.Text
                },
            }, data)
            data.children = this.recursionTree(datalist, data.Id, data.children || []);
            children.push(data);
        });
        return children;
    }
}