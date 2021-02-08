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
                    url: "/student/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/student/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/student/add",
                    method: "post"
                },
                update: {
                    url: "/student/edit",
                    method: "put"
                },
                delete: {
                    url: "/student/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/student/import",
                    method: "post"
                },
                export: {
                    url: "/student/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/student/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/student/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();