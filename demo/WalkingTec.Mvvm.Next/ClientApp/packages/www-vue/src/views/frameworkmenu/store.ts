import { EntitiesPageStore } from '@leng/public/src';
import lodash from 'lodash';
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
        this.ColumnDefs = [
            {
                headerName: "顺序", field: "DisplayOrder",
                // 自定义 多语言 
                // headerValueGetter: (params) => ({ 'zh-CN': '姓名', 'en-US': "Name" }[lodash.get(params, 'context.locale')])
            },
            {
                headerName: "图标", field: "ICon",
            },
        ];
        /**
         * 创建订阅事件处理 不使用  EventSubject 处理事件 直接调用函数执行。 EventSubject 只是方便集中处理 逻辑 
         * 默认只处理 内置 事件 'onSearch', 'onDetails', 'onDelete', 'onInsert', 'onUpdate', 'onImport', 'onExport'
         * @memberof PageStore
         */
        this.onCreateSubscribe();
    }
    /**
     * 刷新菜单
     */
    async onRefreshMenu() {
        const res = await this.Request.ajax('/api_FrameworkMenu/RefreshMenu').toPromise();
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
                value.treePath = this.recursionTree(response.RowData.Data, value.ParentID, value.treePath);
            }
            return value;
        });
        super.onAnalysisSearch(response)
    }
}
export default PageStore