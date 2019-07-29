import { BindAll } from 'lodash-decorators';
import lodash from 'lodash';
import DataSource, { ISearchParams } from 'store/dataSource';
@BindAll()
export class Store extends DataSource {
    constructor() {
        super({
            // IdKey: "ID", 默认 ID
            // Target: "/api", 默认 /api
            Apis: {
                search: {
                    url: "/_frameworkmenu/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkmenu/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_frameworkmenu/add",
                    method: "post"
                },
                update: {
                    url: "/_frameworkmenu/edit",
                    method: "put"
                },
                delete: {
                    url: "/_frameworkmenu/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_frameworkmenu/import",
                    method: "post"
                },
                export: {
                    url: "/_frameworkmenu/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_frameworkmenu/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_frameworkmenu/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
    /** 搜索 */
    async onSearch(params?: ISearchParams) {
        try {
            this.PageState.tableLoading = true;
            const res = await this.Observable.onSearch(params);
            // 格式化树结构数据
            res.Data = res.Data.map(value => {
                value.treePath = [value.PageName];
                if (value.ParentID) {
                    value.treePath = this.recursionTree(res.Data, value.ParentID, value.treePath);
                }
                return value;
            });
            console.log("TCL: Store -> onSearch -> res.Data", res.Data)
            
            this.DataSource.tableList = res;
            return res;
        } catch (error) {
            console.warn(error)
        }
        finally {
            this.PageState.tableLoading = false;
        }
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
            if (findData.ParentID) {
                this.recursionTree(datalist, findData.ParentID, children);
            }
        }
        return children;
    }
}
export default new Store();