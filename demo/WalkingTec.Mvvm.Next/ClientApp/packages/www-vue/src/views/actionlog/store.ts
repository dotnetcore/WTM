import { EntitiesPageStore } from '@leng/public/src';
import lodash from 'lodash';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            Search: { url: '/api/_actionlog/Search', },
            Details: { url: '/api/_actionlog/{ID}' },
            Insert: { url: '/api/_actionlog/Add' },
            Update: { url: '/api/_actionlog/Edit' },
            Delete: { url: '/api/_actionlog/BatchDelete' },
            Export: { url: '/api/_actionlog/ExportExcel' },
            ExportByIds: { url: '/api/_actionlog/ExportExcelByIds' },
            Template: { url: '/api/_actionlog/GetExcelTemplate', },
            Import: { url: '/api/_actionlog/Import', },
        });
        /**
         * 创建订阅事件处理 不使用  EventSubject 处理事件 直接调用函数执行。 EventSubject 只是方便集中处理 逻辑 
         * 默认只处理 内置 事件 'onSearch', 'onDetails', 'onDelete', 'onInsert', 'onUpdate', 'onImport', 'onExport'
         * @memberof PageStore
         */
        this.onCreateSubscribe();
    }
}
export default PageStore