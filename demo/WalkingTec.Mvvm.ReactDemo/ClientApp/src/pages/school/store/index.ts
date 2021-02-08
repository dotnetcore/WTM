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
                    url: "/school/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/school/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/school/add",
                    method: "post"
                },
                update: {
                    url: "/school/edit",
                    method: "put"
                },
                delete: {
                    url: "/school/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/school/import",
                    method: "post"
                },
                export: {
                    url: "/school/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/school/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/school/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();