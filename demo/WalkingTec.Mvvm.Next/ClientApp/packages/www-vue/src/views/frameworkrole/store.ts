import { EntitiesPageStore } from '@leng/public/src';
import lodash from 'lodash';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            // target: 'https://www.baidu.com/',
            Search: { url: '/api/_frameworkrole/Search', },
            Details: { url: '/api/_frameworkrole/{ID}' },
            Insert: { url: '/api/_frameworkrole/Add' },
            Update: { url: '/api/_frameworkrole/Edit' },
            Delete: { url: '/api/_frameworkrole/BatchDelete' },
            Export: { url: '/api/_frameworkrole/ExportExcel' },
            ExportByIds: { url: '/api/_frameworkrole/ExportExcelByIds' },
            Template: { url: '/api/_frameworkrole/GetExcelTemplate', },
            Import: { url: '/api/_frameworkrole/Import', },
        });
        this.ColumnDefs = [
            {
                headerName: "角色编号", field: "RoleCode",
                 // 自定义 多语言 
                // headerValueGetter: (params) => ({ 'zh-CN': '姓名', 'en-US': "Name" }[lodash.get(params, 'context.locale')])
            },
            {
                headerName: "角色名称", field: "RoleName",
            },
            {
                headerName: "备注", field: "RoleRemark",
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