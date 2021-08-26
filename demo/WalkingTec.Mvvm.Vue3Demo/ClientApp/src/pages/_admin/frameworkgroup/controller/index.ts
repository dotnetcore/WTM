import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_frameworkgroup/search',
            // 详情
            details: '/api/_frameworkgroup/{ID}',
            // 添加
            insert: '/api/_frameworkgroup/add',
            // 修改
            update: '/api/_frameworkgroup/edit',
            // 删除 单&多
            delete: '/api/_frameworkgroup/BatchDelete',
            // 导入
            import: '/api/_frameworkgroup/import',
            // 导出
            export: '/api/_frameworkgroup/ExportExcel',
            // 筛选导出
            exportIds: '/api/_frameworkgroup/ExportExcelByIds',
            // 数据模板
            template: '/api/_frameworkgroup/GetExcelTemplate'
        })
    }
}
export default new PageController()