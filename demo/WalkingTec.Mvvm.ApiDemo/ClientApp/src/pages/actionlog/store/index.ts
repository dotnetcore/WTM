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
                    url: "/_actionlog/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_actionlog/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_actionlog/add",
                    method: "post"
                },
                update: {
                    url: "/_actionlog/edit",
                    method: "put"
                },
                delete: {
                    url: "/_actionlog/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_actionlog/import",
                    method: "post"
                },
                export: {
                    url: "/_actionlog/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_actionlog/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_actionlog/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();