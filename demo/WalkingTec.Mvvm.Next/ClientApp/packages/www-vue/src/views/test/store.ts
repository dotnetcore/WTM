import { EntitiesPageStore } from '@leng/public/src';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            target: '/api',
            Search: { url: '/_actionlog/Search', },
            Details: { url: '/_FrameworkUserBase/{id}' },
            Insert: { url: '/_FrameworkGroup/Add' },
            Update: { url: '/_FrameworkGroup/Edit' },
            Delete: { url: '/_FrameworkGroup/BatchDelete' },
            Export: { url: '/_FrameworkGroup/ExportExcel', body: {} },
        });
        this.ColumnDefs = [
            {
                headerName: "类型",
                field: "LogType"
            },
            {
                headerName: "模块",
                field: "ModuleName"
            },
            {
                headerName: "动作",
                field: "ActionName"
            },
            {
                headerName: "ITCode",
                field: "ITCode"
            },
            {
                headerName: "Url",
                field: "ActionUrl"
            },
            {
                headerName: "操作时间",
                field: "ActionTime"
            },
            {
                headerName: "时长",
                field: "Duration"
            },
            {
                headerName: "IP",
                field: "IP"
            },
            {
                headerName: "备注",
                field: "Remark",
                enableRowGroup: false
            }
        ];
        // 订阅事件处理
        this.onSubscribe();
    }
}
export default PageStore