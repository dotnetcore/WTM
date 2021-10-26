import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_frameworkuserbase/search',
            // 详情
            details: '/api/_frameworkuserbase/{ID}',
            // 添加
            insert: '/api/_frameworkuserbase/add',
            // 修改
            update: '/api/_frameworkuserbase/edit',
            batchUpdate: { method: 'post', url: '/api/_frameworkuserbase/BatchEdit' },
            // 删除 单&多
            delete: '/api/_frameworkuserbase/BatchDelete',
            // 导入
            import: '/api/_frameworkuserbase/import',
            // 导出
            export: '/api/_frameworkuserbase/ExportExcel',
            // 筛选导出
            exportIds: '/api/_frameworkuserbase/ExportExcelByIds',
            // 数据模板
            template: '/api/_frameworkuserbase/GetExcelTemplate'
        })
    }
}
class LogController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_actionlog/search',
            // 详情
            details: '/api/_actionlog/{ID}',
            // 添加
            insert: '/api/_actionlog/add',
            // 修改
            update: '/api/_actionlog/edit',
            // 删除 单&多
            delete: '/api/_actionlog/BatchDelete',
            // 导入
            import: '/api/_actionlog/import',
            // 导出
            export: '/api/_actionlog/ExportExcel',
            // 筛选导出
            exportIds: '/api/_actionlog/ExportExcelByIds',
            // 数据模板
            template: '/api/_actionlog/GetExcelTemplate'
        })
    }
}
export const LogPage = new LogController()
export default new PageController()