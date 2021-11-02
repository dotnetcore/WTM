

import * as WTM from "@/client";
export * from './entity';

export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_Admin/FrameworkUser/search',
            // 详情
            details: '/api/_Admin/FrameworkUser/{ID}',
            // 添加
            insert: '/api/_Admin/FrameworkUser/add',
            // 修改
            update: '/api/_Admin/FrameworkUser/edit',
            batchUpdate: { method: 'post', url: '/api/_Admin/FrameworkUser/BatchEdit' },
            // 删除 单&多
            delete: '/api/_Admin/FrameworkUser/BatchDelete',
            // 导入
            import: '/api/_Admin/FrameworkUser/import',
            // 导出
            export: '/api/_Admin/FrameworkUser/ExportExcel',
            // 筛选导出
            exportIds: '/api/_Admin/FrameworkUser/ExportExcelByIds',
            // 数据模板
            template: '/api/_Admin/FrameworkUser/GetExcelTemplate'
        })
    }
}

export default new PageController()
