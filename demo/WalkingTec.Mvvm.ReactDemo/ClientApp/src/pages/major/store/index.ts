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
                    url: "/major/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/major/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/major/add",
                    method: "post"
                },
                update: {
                    url: "/major/edit",
                    method: "put"
                },
                delete: {
                    url: "/major/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/major/import",
                    method: "post"
                },
                export: {
                    url: "/major/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/major/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/major/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();