import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_frameworkuser/search',
            // 详情
            details: '/api/_frameworkuser/{ID}',
            // 添加
            insert: '/api/_frameworkuser/add',
            // 修改
            update: '/api/_frameworkuser/edit',
            batchUpdate: { method: 'post', url: '/api/_frameworkuser/BatchEdit' },
            // 删除 单&多
            delete: '/api/_frameworkuser/BatchDelete',
            // 导入
            import: '/api/_frameworkuser/import',
            // 导出
            export: '/api/_frameworkuser/ExportExcel',
            // 筛选导出
            exportIds: '/api/_frameworkuser/ExportExcelByIds',
            // 数据模板
            template: '/api/_frameworkuser/GetExcelTemplate'
        })
    }
}
export default new PageController()
