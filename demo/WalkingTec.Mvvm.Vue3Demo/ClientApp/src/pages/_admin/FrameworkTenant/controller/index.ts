import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_FrameworkTenant/search',
            // 详情
            details: '/api/_FrameworkTenant/{ID}',
            // 添加
            insert: '/api/_FrameworkTenant/add',
            // 修改
            update: '/api/_FrameworkTenant/edit',
            // 删除 单&多
            delete: '/api/_FrameworkTenant/BatchDelete',
            // 导入
            import: '/api/_FrameworkTenant/import',
            // 导出
            export: '/api/_FrameworkTenant/ExportExcel',
            // 筛选导出
            exportIds: '/api/_FrameworkTenant/ExportExcelByIds',
            // 数据模板
            template: '/api/_FrameworkTenant/GetExcelTemplate'
        })
    }
}
export default new PageController()