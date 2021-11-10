import * as WTM from "@/client";
import lodash from "lodash";
import { BindAll } from "lodash-decorators";
export * from './entity';
@BindAll()
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super()
        this.onReset({
            // 唯一标识
            key: "ID",
            // 搜索
            search: '/api/_frameworkmenu/search',
            // 详情
            details: '/api/_frameworkmenu/{ID}',
            // 添加
            insert: '/api/_frameworkmenu/add',
            // 修改
            update: '/api/_frameworkmenu/edit',
            // 删除 单&多
            delete: '/api/_frameworkmenu/BatchDelete',
            // 导入
            import: '/api/_frameworkmenu/import',
            // 导出
            export: '/api/_frameworkmenu/ExportExcel',
            // 筛选导出
            exportIds: '/api/_frameworkmenu/ExportExcelByIds',
            // 数据模板
            template: '/api/_frameworkmenu/GetExcelTemplate',
            // 分页数据配置
            PaginationOptions: { onMapValues: this.onMapValues }
        })
    }
    /**
     * 分页数据过滤重组
     * @param res 
     * @returns 
     */
    onMapValues(res) {
        res.dataSource = lodash.map(res.dataSource, value => {
            value.treePath = [value.PageName];
            if (value.ParentId) {
                value.treePath = this.recursionTree(res.dataSource, value.ParentId, value.treePath);
            }
            return value
        })
        return res
    }
    /**
     * 递归 格式化 树
     * @param datalist 
     * @param ParentId 
     * @param children 
     */
    recursionTree(datalist, ParentId, children = []) {
        const findData = lodash.find(datalist, ['ID', ParentId]);
        if (findData) {
            children.unshift(findData.PageName);
            if (findData.ParentId) {
                this.recursionTree(datalist, findData.ParentId, children);
            }
        }
        return children;
    }
}
export default new PageController()