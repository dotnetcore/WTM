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
                    url: "/city/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/city/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/city/add",
                    method: "post"
                },
                update: {
                    url: "/city/edit",
                    method: "put"
                },
                delete: {
                    url: "/city/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/city/import",
                    method: "post"
                },
                export: {
                    url: "/city/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/city/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/city/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();