import { EntitiesPageStore } from '@leng/public/src';
import lodash from 'lodash';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            // target: 'https://www.baidu.com/',
            Search: { url: '/api/_frameworkgroup/Search', },
            Details: { url: '/api/_frameworkgroup/{ID}' },
            Insert: { url: '/api/_frameworkgroup/Add' },
            Update: { url: '/api/_frameworkgroup/Edit' },
            Delete: { url: '/api/_frameworkgroup/BatchDelete' },
            Export: { url: '/api/_frameworkgroup/ExportExcel' },
            ExportByIds: { url: '/api/_frameworkgroup/ExportExcelByIds' },
            Template: { url: '/api/_frameworkgroup/GetExcelTemplate', },
            Import: { url: '/api/_frameworkgroup/Import', },
        });
        this.ColumnDefs = [
            {
                headerName: "用户组编码", field: "GroupCode",
                // 自定义 多语言 
                // headerValueGetter: (params) => ({ 'zh-CN': '姓名', 'en-US': "Name" }[lodash.get(params, 'context.locale')])
            },
            {
                headerName: "用户组名称", field: "GroupName",
            },
            {
                headerName: "备注", field: "GroupRemark",
            },
        ];
        /**
         * 创建订阅事件处理 不使用  EventSubject 处理事件 直接调用函数执行。 EventSubject 只是方便集中处理 逻辑 
         * 默认只处理 内置 事件 'onSearch', 'onDetails', 'onDelete', 'onInsert', 'onUpdate', 'onImport', 'onExport'
         * @memberof PageStore
         */
        this.onCreateSubscribe();
    }
}
export default PageStore