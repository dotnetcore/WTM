import { Regulars } from "@/client/helpers";
import lodash from "lodash";
import queryString from 'query-string';
import { Subject } from "rxjs";
import { createApp, createVNode } from 'vue';
/**
 * 用户菜单
 */
export class UserMenus {
    constructor() {
    }
    /**
     * 用户异步数据加载订阅
     * @type {Promise<any>}
     * @memberof ControllerUser
     */
    MenusAsync = new Subject()
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
            this.MenusAsync.complete()
        }
    }
    getMenus() {
        // console.log("LENG ~ UserMenus ~ getMenus ~ this.menus", this.menus)
        return this.menus
    }
    findMenus(path) {
        return lodash.find(this.ParallelMenu, ['Url', path])
    }
    /**
     * 递归 格式化 树
     * @param datalist 
     * @param ParentId 
     * @param children 
     */
    recursionTree(datalist, ParentId, children = []) {
        lodash.filter(datalist, ['ParentId', ParentId]).map(data => {
            let path = data.Url
            // 外部链接地址
            if (Regulars.url.test(data.Url)) {
                path = queryString.stringifyUrl({ url: '/webview', query: { src: data.Url, name: data.Text } })
            }
            data = lodash.assign({ path }, {
                router: { to: path },
                meta: {
                    icon: createVNode('span', { class: data.Icon, style: 'margin-right:10px' }),//createApp({ template: `<span class="${data.Icon}">` }),
                    title: data.Text
                },
            }, data)
            data.children = this.recursionTree(datalist, data.Id, data.children || []);
            children.push(data);
        });
        return children;
    }
}