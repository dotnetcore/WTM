import { EntitiesPageStore } from '@leng/public/src';
import lodash from 'lodash';
import { Observable } from 'rxjs';
export function onGetController() {
    return new Observable<any[]>((sub) => {
        import("../index").then(pages => {
            const PagesList = [];
            lodash.map(pages.default, (item) => {
                if (item.controller) {
                    PagesList.push({ Text: item.name, Value: item.controller, Url: item.path })
                }
            })
            sub.next(PagesList);
            sub.complete();
        })
    })
}
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            // target: 'https://www.baidu.com/',
            Search: { url: '/api/_frameworkmenu/Search', },
            Details: { url: '/api/_frameworkmenu/{ID}' },
            Insert: { url: '/api/_frameworkmenu/Add' },
            Update: { url: '/api/_frameworkmenu/Edit' },
            Delete: { url: '/api/_frameworkmenu/BatchDelete' },
            Export: { url: '/api/_frameworkmenu/ExportExcel' },
            ExportByIds: { url: '/api/_frameworkmenu/ExportExcelByIds' },
            Template: { url: '/api/_frameworkmenu/GetExcelTemplate', },
            Import: { url: '/api/_frameworkmenu/Import', },
        });
        /**
         * 创建订阅事件处理 不使用  EventSubject 处理事件 直接调用函数执行。 EventSubject 只是方便集中处理 逻辑 
         * 默认只处理 内置 事件 'onSearch', 'onDetails', 'onDelete', 'onInsert', 'onUpdate', 'onImport', 'onExport'
         * @memberof PageStore
         */
        // this.onCreateSubscribe();
    }
    /**
     * 刷新菜单
     */
    async onRefreshMenu() {
        const res = await this.Request.ajax('/api/_FrameworkMenu/RefreshMenu').toPromise();
        // console.log("TCL: Store -> onRefreshMenu -> res", res)
        // message.info(res.Value)
    }
    /**
    * 递归 格式化 树
    * @param datalist 
    * @param ParentId 
    * @param children 
    */
    recursionTree(datalist, ParentId, children = []) {
        const findData = lodash.find(datalist, ['ID', ParentId]);
        if (findData) {
            children.unshift(findData.PageName);
            if (findData.ParentID) {
                this.recursionTree(datalist, findData.ParentID, children);
            }
        }
        return children;
    }
    /**
     * 解析 onSearch数据
     * @param response 
     */
    onAnalysisSearch(response: any) {
        response.RowData = response.RowData.map(value => {
            value.treePath = [value.PageName];
            if (value.ParentID) {
                value.treePath = this.recursionTree(response.RowData, value.ParentID, value.treePath);
            }
            return value;
        });
        super.onAnalysisSearch(response)
    }
}
export default PageStore
