import { EntitiesPageStore } from '@leng/public/src';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            target: '/api',
            Search: { url: '/FrameworkUser/Search', },
            Details: { url: '/FrameworkUser/{id}' },
            Insert: { url: '/FrameworkUser/Add' },
            Update: { url: '/FrameworkUser/Edit' },
            Delete: { url: '/FrameworkUser/BatchDelete' },
            Export: { url: '/FrameworkUser/ExportExcel' },
        });
        this.DeBugLog = true;
        this.ColumnDefs = [
            {
                headerName: "账号", field: "ITCode",
            },
            {
                headerName: "姓名", field: "Name",
            },
            {
                headerName: "性别", field: "Sex",
            },
            {
                headerName: "照片", field: "PhotoId", cellRenderer: "avatar"
            },
            {
                headerName: "是否有效", field: "IsValid", cellRenderer: "switch"
            },
            {
                headerName: "角色", field: "RoleName_view",
            },
            {
                headerName: "用户组", field: "GroupName_view",
            }
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