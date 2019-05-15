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
                    url: "/_frameworkgroup/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkgroup/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_frameworkgroup/add",
                    method: "post"
                },
                update: {
                    url: "/_frameworkgroup/edit",
                    method: "put"
                },
                delete: {
                    url: "/_frameworkgroup/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_frameworkgroup/import",
                    method: "post"
                },
                export: {
                    url: "/_frameworkgroup/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_frameworkgroup/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_frameworkgroup/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();