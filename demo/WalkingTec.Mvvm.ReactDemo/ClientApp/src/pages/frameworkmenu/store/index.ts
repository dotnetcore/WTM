import { BindAll } from 'lodash-decorators';
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
            // res.Data = res.Data.map(value => {
            //     console.log("TCL: Store -> onSearch -> value", value)
            //     return value;
            // })
            this.DataSource.tableList = res;
            return res;
        } catch (error) {
            console.warn(error)
        }
        finally {
            this.PageState.tableLoading = false;
        }
    }
}
export default new Store();