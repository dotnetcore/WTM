import { Regulars } from "@/client/helpers";
import lodash from "lodash";
import queryString from 'query-string';
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
        // console.log("LENG ~ UserMenus ~ getMenus ~ this.menus", this.menus)
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
            let router: any = {
                path: data.Url,
                name: data.Url ? undefined : data.Id,
            }
            // 外部链接地址
            if (Regulars.url.test(data.Url)) {
                router = queryString.stringifyUrl({ url: '/webview', query: { src: data.Url, name: data.Text } })
            }
            data = lodash.assign({}, {
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