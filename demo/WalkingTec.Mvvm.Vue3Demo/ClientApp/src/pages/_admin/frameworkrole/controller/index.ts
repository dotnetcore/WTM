import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_frameworkrole/search',
            // 详情
            details: '/api/_frameworkrole/{ID}',
            // 添加
            insert: '/api/_frameworkrole/add',
            // 修改
            update: '/api/_frameworkrole/edit',
            // 删除 单&多
            delete: '/api/_frameworkrole/BatchDelete',
            // 导入
            import: '/api/_frameworkrole/import',
            // 导出
            export: '/api/_frameworkrole/ExportExcel',
            // 筛选导出
            exportIds: '/api/_frameworkrole/ExportExcelByIds',
            // 数据模板
            template: '/api/_frameworkrole/GetExcelTemplate',
            // 获取权限
            GetPageActions: '/api/_frameworkrole/GetPageActions/{ID}',
            // 权限
            EditPrivilege: '/api/_frameworkrole/EditPrivilege'
        })
    }
    /**
     * 获取权限
     * @param body 
     * @returns 
     */
    onGetPrivilege(ID) {
        return this.$ajax.get<{ Pages: Array<any>, Entity: any }>(this.getAjaxRequest('GetPageActions').url, { ID })
    }
    /**
     * 保存权限
     * @param params 
     * @returns 
     */
    onSavePrivilege(params) {
        return this.$ajax.put(this.getAjaxRequest('EditPrivilege').url, params)
    }
}
export default new PageController()