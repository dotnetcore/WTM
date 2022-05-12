

import * as WTM from "@/client";
export * from './entity';

export class StudentPageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/Student/Search',
            // 详情
            details: '/api/Student/{ID}',
            // 添加
            insert: '/api/Student/Add',
            // 修改
            update: '/api/Student/Edit',
            batchUpdate: { method: 'post', url: '/api/Student/BatchEdit' },
            // 删除 单&多
            delete: '/api/Student/BatchDelete',
            // 导入
            import: '/api/Student/Import',
            // 导出
            export: '/api/Student/StudentExportExcel',
            // 筛选导出
            exportIds: '/api/Student/StudentExportExcelByIds',
            // 数据模板
            template: '/api/Student/GetExcelTemplate'
        })
    }
}

export const ExStudentPageController = new StudentPageController()
