import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            key: "ID",
            search: '/api/_frameworkuserbase/search',
            details: '/api/_frameworkuserbase/{ID}',
            insert: '/api/_frameworkuserbase/add',
            update: '/api/_frameworkuserbase/edit',
            delete: '/api/_frameworkuserbase/BatchDelete',
            import: '/api/_frameworkuserbase/import',
            export: '/api/_frameworkuserbase/ExportExcel',
            exportIds: '/api/_frameworkuserbase/ExportExcelByIds',
            template: '/api/_frameworkuserbase/GetExcelTemplate'
        })
    }
}
export default new PageController()