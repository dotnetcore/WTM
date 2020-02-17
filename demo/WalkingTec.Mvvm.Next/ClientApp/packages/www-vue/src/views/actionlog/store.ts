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
        this.ColumnDefs = [
            {
                headerName: "类型", field: "LogType",
            },
            {
                headerName: "模块", field: "ModuleName"
            },
            {
                headerName: "动作", field: "ActionName"
            },
            {
                headerName: "ITCode", field: "ITCode"
            },
            {
                headerName: "Url", field: "ActionUrl",
            },
            {
                headerName: "操作时间", field: "ActionTime",
            },
            {
                headerName: "时长", field: "Duration"
            },
            {
                headerName: "IP", field: "IP",
            },
            {
                headerName: "备注", field: "Remark", enableRowGroup: false
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