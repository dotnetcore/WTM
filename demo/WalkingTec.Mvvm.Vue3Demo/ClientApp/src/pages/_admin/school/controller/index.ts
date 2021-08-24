import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/School/search',
            // 详情
            details: '/api/School/{ID}',
            // 添加
            insert: '/api/School/add',
            // 修改
            update: '/api/School/edit',
            // 删除 单&多
            delete: '/api/School/BatchDelete',
            // 导入
            import: '/api/School/import',
            // 导出
            export: '/api/School/ExportExcel',
            // 筛选导出
            exportIds: '/api/School/ExportExcelByIds',
            // 数据模板
            template: '/api/School/GetExcelTemplate'
        })
    }
}
export default new PageController()