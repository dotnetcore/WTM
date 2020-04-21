import { EntitiesPageStore } from '@leng/public/src';
import lodash from 'lodash';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            // target: 'https://www.baidu.com/',
            Search: { url: '/api/_FrameworkUserBase/Search', },
            Details: { url: '/api/_FrameworkUserBase/{ID}' },
            Insert: { url: '/api/_FrameworkUserBase/Add' },
            Update: { url: '/api/_FrameworkUserBase/Edit' },
            Delete: { url: '/api/_FrameworkUserBase/BatchDelete' },
            Export: { url: '/api/_FrameworkUserBase/ExportExcel' },
            ExportByIds: { url: '/api/_FrameworkUserBase/ExportExcelByIds' },
            Template: { url: '/api/_FrameworkUserBase/GetExcelTemplate', },
            Import: { url: '/api/_FrameworkUserBase/Import', },
        });
        /**
         * 创建订阅事件处理 不使用  EventSubject 处理事件 直接调用函数执行。 EventSubject 只是方便集中处理 逻辑 
         * 默认只处理 内置 事件 'onSearch', 'onDetails', 'onDelete', 'onInsert', 'onUpdate', 'onImport', 'onExport'
         * @memberof PageStore
         */
        // this.onCreateSubscribe();
    }
}
export default PageStore