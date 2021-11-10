import * as WTM from "@/client";
import { observable } from "mobx";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/City/search',
            // 详情
            details: '/api/City/{ID}',
            // 添加
            insert: '/api/City/add',
            // 修改
            update: '/api/City/edit',
            // 删除 单&多
            delete: '/api/City/BatchDelete',
            // 导入
            import: '/api/City/import',
            // 导出
            export: '/api/City/ExportExcel',
            // 筛选导出
            exportIds: '/api/City/ExportExcelByIds',
            // 数据模板
            template: '/api/City/GetExcelTemplate'
        })
    }
    onGetTree() {
        return this.$ajax.get<any>('/api/City/GetCitysTree')
    }
}
export default new PageController()