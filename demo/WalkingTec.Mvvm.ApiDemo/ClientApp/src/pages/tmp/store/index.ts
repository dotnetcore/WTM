import { BindAll } from 'lodash-decorators';
import DataSource from 'store/dataSource';
@BindAll()
export class Store extends DataSource {
    constructor() {
        super({
            // IdKey: "ID", 默认 ID
            // Target: "/api", 默认 /api
            Apis: {
                search: {
                    url: "/frameworkuser/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/frameworkuser/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/frameworkuser/add",
                    method: "post"
                },
                update: {
                    url: "/frameworkuser/edit",
                    method: "put"
                },
                delete: {
                    url: "/frameworkuser/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/frameworkuser/import",
                    method: "post"
                },
                export: {
                    url: "/frameworkuser/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/frameworkuser/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/frameworkuser/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
    // 具体方法可重写
    // /** 搜索 */
    // async onSearch(params?: ISearchParams) {
    //     const res = await this.Observable.onSearch(params);
    //     this.DataSource.tableList = res;
    //     return res;
    // }
    // /** 详情 */
    // async onDetails(params) {
    //     const res = await this.Observable.onDetails(params);
    //     this.DataSource.details = res;
    //     return res;
    // }
    // /** 添加 */
    // async onInsert(params) {
    //     const res = await this.Observable.onInsert(params);
    //     notification.success({ message: "操作成功" });
    //     this.onSearch();
    //     return res;
    // }
    // /** 修改 */
    // async onUpdate(params) {
    //     const res = await this.Observable.onUpdate(this.DataSource.details, params);
    //     notification.success({ message: "操作成功" });
    //     this.onSearch();
    //     return res;
    // }
}
export default new Store();