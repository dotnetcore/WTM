import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_dataprivilege/search',
            // 详情
            details: '/api/_dataprivilege/get',
            // 添加
            insert: '/api/_dataprivilege/add',
            // 修改
            update: '/api/_dataprivilege/edit',
            // 删除 单&多
            delete: '/api/_dataprivilege/BatchDelete',
            // 导入
            import: '/api/_dataprivilege/import',
            // 导出
            export: '/api/_dataprivilege/ExportExcel',
            // 筛选导出
            exportIds: '/api/_dataprivilege/ExportExcelByIds',
            // 数据模板
            template: '/api/_dataprivilege/GetExcelTemplate'
        })
    }
}
export default new PageController()